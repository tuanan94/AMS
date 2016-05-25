using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;

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
        public List<User> findByHouseId(int houseId)
        {
            return userRepository.findByHouseID(houseId);
           
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