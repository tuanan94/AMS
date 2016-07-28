using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;
using System.Security.Claims;
using AMS.Constant;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using AMS.ObjectMapping;

namespace AMS.Service
{

    public class UserService
    {
        private UserRepository userRepository;
        public UserService()
        {
            userRepository = new UserRepository();
        }
        public UserService(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public void addUser(User user)
        {
            this.userRepository.Add(user);
        }
        public User findByUsername(String username)
        {
            return userRepository.findByUsername(username);
        }

        public User findById(int id)
        {
            User user = userRepository.FindById(id);

            return user;
        }
        public UserProfileMapping findUserProfileMappingById(int id)
        {
            User user = findById(id);
            UserProfileMapping userProfileMapping = new UserProfileMapping();
            userProfileMapping.Id = user.Id;
            userProfileMapping.DateOfBirth = user.DateOfBirth.GetValueOrDefault();
            userProfileMapping.Email = user.Email;
            userProfileMapping.FullName = user.Fullname;
            userProfileMapping.Gender = user.Gender.GetValueOrDefault() ;
            userProfileMapping.ProfileImage = user.ProfileImage;
            userProfileMapping.HouseId = user.HouseId.GetValueOrDefault();
            userProfileMapping.HouseName = user.House == null ? "Không xác định" : user.House.HouseName;
            userProfileMapping.CreatedDate = user.CreateDate.Value.ToString(AmsConstants.DateFormat);
            userProfileMapping.Age = CommonUtil.CalculateAge(user.DateOfBirth.Value);
            userProfileMapping.HouseProfile = user.House == null||user.House.ProfileImage==null||user.House.ProfileImage.Equals("") ? "/Content/Images/home_default.jpg" : user.House.ProfileImage;
            List<Post> rawPost = user.Posts.OrderByDescending(p => p.CreateDate).Take(5).ToList();
            List<MoreInfo> moreInfos = new List<MoreInfo>();
            foreach(Post p in rawPost)
            {
                MoreInfo pInfo = new MoreInfo();
                pInfo.Id = p.Id +"";
                pInfo.createdDate = p.CreateDate.Value.ToString("s");
                pInfo.PostText = CommonUtil.TruncateLongString(p.Body, 100); 
                moreInfos.Add(pInfo);
            }
            userProfileMapping.moreInfos = moreInfos;
            return userProfileMapping;
        }
        public List<User> findByHouseId(int houseId)
        {
            return userRepository.findByHouseID(houseId);

        }
        public void updateUser(User u)
        {
            userRepository.Update(u);

        }
        public void deleteUser(User u)
        {
            userRepository.Delete(u);
        }

        public SLIM_CONFIG.LoginResult login(string username, string password)
        {
            User user = userRepository.findByUsername(username);
            if (user == null)
            {
                return SLIM_CONFIG.LoginResult.NoUser;
            }
            else
            {
                if (password.Equals(user.Password))
                {
                    var ident = new ClaimsIdentity(
             new[] { 
              // adding following 2 claim just for supporting default antiforgery provider
              new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
              new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

              new Claim(ClaimTypes.Name,username),

              // optionally you could add roles if any
              new Claim(ClaimTypes.Role, user.Role.RoleName)

        },
        DefaultAuthenticationTypes.ApplicationCookie);
                    HttpContext.Current.GetOwinContext().Authentication.SignIn(
            new AuthenticationProperties { IsPersistent = false }, ident);

                    return SLIM_CONFIG.LoginResult.Success;
                }
                else
                {
                    return SLIM_CONFIG.LoginResult.WrongPassword;
                }
            }
        }

    }
}