using AMS.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Helper;
using AMS.ViewModel;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Xml.Linq;
using AMS.Constant;
using Newtonsoft.Json;
using AMS.ObjectMapping;
using AMS.Filter;
using AMS.Models;
using Microsoft.Ajax.Utilities;

namespace AMS.Controllers
{
    [AuthorizationPrivilegeFilter_RequestHouse]
    public class HomeController : Controller
    {
        TestService testService = new TestService();
        PostService postService = new PostService();
        UserService userService = new UserService();
        HouseServices houseService = new HouseServices();
        NotificationService notificationService = new NotificationService();
        readonly string parternTime = "dd-MM-yyyy HH:mm";


        public ActionResult Test()
        {
            List<House> allHouse = testService.getAllHouse();
            ViewBag.allHouse = allHouse;
            return View(allHouse);
        }

        [Authorize]
        [AutoRedirect.MandatorySurveyRedirect]
        public ActionResult Index()
        {
            User curUser = userService.findById(int.Parse(User.Identity.GetUserId()));

            if (curUser == null)
            {
                return View("error");
            }
            ViewBag.curUser = curUser;
            ViewBag.curHouse = curUser.House;
            weatherResult weatherResult = WeatherUtil.getWeatherResult();
            ViewBag.weather = weatherResult;
            ViewBag.notifiations = notificationService.getAllNotificationChange(curUser.Id);
            return View();
        }
        [HttpPost]
        public void AddByAjax(string Title, int PostId)
        {
            //            postService.createPost(Title, PostId);

        }
        [HttpPost]
        public ActionResult Index(string Title, int PostId)
        {

            //            postService.createPost(Title, PostId);
            return RedirectToAction("TimeLine");
        }
        [HttpPost]
        public ActionResult Indexx(ListPostViewModel model, int postId)
        {

            //            postService.createPost(model.Title, model.Id);
            //return PartialView("TimeLine");
            return RedirectToAction("TimeLine");
        }

        //        public ActionResult TimeLine()
        //        {
        //            // get all post
        ////            IEnumerable<Post> allPost = postService.getAllPost();
        ////            IEnumerable<Post> listComment = new List<Post>();
        ////            ListPostViewModel listPostViewModel = new ListPostViewModel();
        ////            listPostViewModel.listPost = new List<PostViewModel>();
        ////            foreach (var item in allPost)
        ////            {
        ////                PostViewModel postViewModel = new PostViewModel();
        ////                postViewModel.ImgUrl = item.ImgUrl;
        ////                postViewModel.Id = item.Id;
        ////                postViewModel.Title = item.Title;
        ////                postViewModel.CountComment = postService.CountComment(postViewModel.Id);
        ////                if (item.CreateDate.HasValue)
        ////                {
        ////                    postViewModel.CreateDate = item.CreateDate.Value;
        ////                }
        ////
        ////                //get list comment belong post
        ////                listComment = postService.getCommentBelongPost(postViewModel.Id);
        ////                if (listComment != null)
        ////                {
        ////                    postViewModel.Post = listComment;
        ////                }
        ////                listPostViewModel.listPost.Add(postViewModel);
        ////
        ////            }
        //
        //
        ////            return View(listPostViewModel);
        //        }

        [HttpPost]
        public ActionResult TimeLine(PostViewModel post, string Title, HttpPostedFileBase Media)
        {
            string mediaUrl = null;


            //if (ModelState.IsValid)
            //{
            if (post.Media != null && post.Media.ContentLength > 0)
            {
                {

                    // Save dir
                    var uploadDir = "~/images/Post/Binh";
                    // File extentions
                    var ext = "";
                    //Modify ANTT
                    ext = Media.FileName.Substring(post.Media.FileName.LastIndexOf(".",
                            StringComparison.Ordinal));

                    // Force it to be jpg
                    var fileName = Util.GetUnixTime() + ".jpg";
                    // Tmp file name
                    var tmpFileName = "tmp" + Util.GetUnixTime() + ext;

                    string serverPath = Server.MapPath(uploadDir);
                    // tmp image path
                    var imagePath = Path.Combine(serverPath, tmpFileName);
                    if (!Directory.Exists(serverPath))
                    {
                        Directory.CreateDirectory(serverPath);
                    }
                    // Save as tmp file
                    post.Media.SaveAs(imagePath);
                    if (Util.ConvertImageToJpg(uploadDir, tmpFileName, ConstantB.DefaultImageQuality, uploadDir,
                        fileName))
                    {
                        mediaUrl = Path.Combine(uploadDir, fileName);


                    }

                    Util.DeleteFile(imagePath);
                }


            }
            Post p = new Post();
            // post.Body = p.Body;
            post.Title = Title;
            post.ImgUrl = mediaUrl;
            post.CreateDate = DateTime.Now;
            //            postService.createPost(post);
            //}
            return RedirectToAction("TimeLine");
        }

        [HttpPost]
        public ActionResult TimeLinex(ListPostViewModel post)
        {
            string mediaUrl = null;


            //if (ModelState.IsValid)
            //{

            Post p = new Post();
            // post.Body = p.Body;
            p.Title = post.Title;
            p.ImgUrl = post.ImgUrl;

            //            postService.CreatePosts(p);
            //}
            return RedirectToAction("TimeLine");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        //BinhHT
        [HttpGet]
        [Authorize]
        public ActionResult ViewProfile()
        {
            User currentUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            ViewBag.user = currentUser;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateProfile(UserInfoViewModel user)
        {
            try
            {
                User u = userService.findById(int.Parse(User.Identity.GetUserId()));
                if (null != u)
                {
                    u.Fullname = user.Name;
                    u.LastModified = DateTime.Now;
                    u.IDNumber = user.Idenity;
                    u.Gender = user.Gender;
                    u.DateOfBirth = DateTime.ParseExact(user.Dob, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                    u.LastModified = DateTime.Now;
                    u.FamilyLevel = user.RelationLevel;
                    u.SendPasswordTo = user.CellNumb;
                    if (user.IdCreateDate != null)
                    {
                        u.IDCreatedDate = DateTime.ParseExact(user.IdCreateDate, AmsConstants.DateFormat,
                            CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        u.IDCreatedDate = null;
                    }

                    userService.updateUser(u);
                }
            }
            catch (Exception e)
            {
                RedirectToAction("ViewProfile");
            }
            return RedirectToAction("ViewProfile");
        }
        [HttpPost]
        [Authorize]
        public String saveProfileImage(int userid, String url)
        {
            User user = userService.findById(userid);
            if (user == null)
            {
                return "error";
            }
            else
            {
                user.ProfileImage = url;
            }
            userService.updateUser(user);
            return "success";
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetCurrentPassword(int userId)
        {
            MessageViewModels response = new MessageViewModels();
            User user = userService.findById(userId);
            if (user != null)
            {
                response.Data = user.Password;
            }
            else
            {
                response.StatusCode = -1;
            }
            userService.updateUser(user);
            return Json(response);
        }

        [HttpPost]
        public ActionResult CheckPass(int userId, string curPass)
        {
            User user = userService.findById(userId);
            if (user != null && (!curPass.IsNullOrWhiteSpace()))
            {
                if (curPass.Equals(user.Password))
                {
                    return Json(true);
                }
            }
            return Json(false);
        }

        [HttpPost]
        public ActionResult UpdatePassword(int userId, string ConfirmOldPass, string NewPass)
        {
            MessageViewModels response = new MessageViewModels();
            User user = userService.findById(userId);
            if (user != null && (!ConfirmOldPass.IsNullOrWhiteSpace()) && (!NewPass.IsNullOrWhiteSpace()))
            {
                if (ConfirmOldPass.Equals(user.Password))
                {
                    try
                    {
                        user.Password = NewPass;
                        user.LastModified = DateTime.Now;
                        userService.updateUser(user);

                        response.StatusCode = 0;
                        return Json(response);
                    }
                    catch (Exception)
                    {
                        response.StatusCode = -1;
                        return Json(response);
                    }
                }
            }
            response.StatusCode = -1;
            return Json(response);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ManageMember()
        {
            var currentUser = User.Identity;
            int userId;
            bool isValidID = int.TryParse(currentUser.GetUserId(), out userId);
            if (!isValidID)
            {
                return View("error");
            }
            User user = userService.findById(userId);
            List<User> members = userService.findByHouseId((user.HouseId.HasValue == true ? user.HouseId.Value : -1));
            ViewBag.currentHouse = user.House;
            ViewBag.members = members;
            return View();
        }





        [HttpPost]
        public ActionResult ManageMember(AddMemberViewModel member)
        {
            if (ModelState.IsValid)
            {
                User existUser = userService.findByUsername(member.Username);
                if (existUser != null)
                {
                    ModelState.AddModelError("Username", "Tên người dùng này đã được sử dụng");
                }
                else
                {
                    var curUser = userService.findById(int.Parse(User.Identity.GetUserId()));
                    User newUser = new User();
                    newUser.Creator = curUser.Id;
                    newUser.Fullname = member.Fullname;
                    newUser.Username = curUser.House.HouseName + "_" + member.Username;
                    newUser.Password = member.Password;
                    newUser.Gender = member.Gender;
                    newUser.ProfileImage = member.ImageURL;
                    newUser.RoleId = SLIM_CONFIG.Role_RESIDENT;
                    newUser.HouseId = curUser.HouseId;
                    newUser.Status = SLIM_CONFIG.USER_APPROVE_WAITING;
                    newUser.DateOfBirth = member.DateOfBirth;
                    newUser.CreateDate = DateTime.Now;
                    newUser.LastModified = DateTime.Now;
                    userService.addUser(newUser);
                }
            }
            else
            {
                ViewBag.adding = true;
            }
            var currentUser = User.Identity;
            int userId;
            bool isValidID = int.TryParse(currentUser.GetUserId(), out userId);
            if (!isValidID)
            {
                return View("error");
            }
            User user = userService.findById(userId);
            List<User> members = userService.findByHouseId(user.Id);
            ViewBag.currentHouse = user.House;
            ViewBag.members = members;
            return View();
            //pendingMemberService.addMemberRequest(member);

        }

        [HttpGet]
        [Authorize]
        [AutoRedirect.MandatorySurveyRedirect]
        public ActionResult Setting()
        {
            User curUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            if (curUser == null)
            {
                return View("error");
            }
            House curHouse = curUser.House;
            ViewBag.curHouse = curHouse;
            return View();
        }
        [HttpGet]
        [Authorize]
        public Object getUserByHouseId(int? houseId)
        {
            if (houseId == null)
            {
                return null;
            }
            House curHouse = houseService.FindById(houseId.Value);
            if (curHouse == null)
            {
                return "error";
            }
            User curUser = userService.findById(int.Parse(User.Identity.GetUserId()));

            if (curHouse.DisplayMember == false && curUser.HouseId != curHouse.Id)
            {
                return "NOT_PERMISSION";
            }
            List<User> result = userService.findByHouseId(houseId.Value);
            // Serializer settings
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomResolver(typeof(User));
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            // Do the serialization and output to the console
            string json = JsonConvert.SerializeObject(result, settings);
            return json;
        }


        [HttpPost]
        [Authorize]
        public String ChangeProfile(int houseId, String dir)
        {
            House h = houseService.FindById(houseId);
            if (h == null)
            {
                return "error";
            }
            else
            {
                h.ProfileImage = dir;
            }
            houseService.Update(h);
            return "success";

        }

        [HttpPost]
        [Authorize]
        public String UpdateDisplayMember(int houseId, bool display)
        {
            House h = houseService.FindById(houseId);
            if (h == null)
            {
                return "error";
            }
            else
            {
                h.DisplayMember = display;
            }
            houseService.Update(h);
            return "success";

        }

        [HttpPost]
        [Authorize]
        public String UpdateAllowOtherView(int houseId, bool display)
        {
            House h = houseService.FindById(houseId);
            if (h == null)
            {
                return "error";
            }
            else
            {
                h.AllowOtherView = display;
            }
            houseService.Update(h);
            return "success";

        }
        [HttpGet]
        [Authorize]
        public String getUser(int UserId)
        {
            UserProfileMapping user = userService.findUserProfileMappingById(UserId);
            // Serializer settings
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomResolver(typeof(UserProfileMapping));
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            // Do the serialization and output to the console
            string json = JsonConvert.SerializeObject(user, settings);
            return json;
        }
        [HttpGet]
        [Authorize]
        public Object getNotification()
        {
            List<Notification> notis = notificationService.getNotification(int.Parse(User.Identity.GetUserId()));
            // Serializer settings
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomResolver(typeof(Notification));
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            // Do the serialization and output to the console
            string json = JsonConvert.SerializeObject(notis, settings);
            return json;
        }
        [HttpGet]
        [Authorize]
        public String getHintUsername(String fullname, int? startNumber)
        {

            String hintResult;
            User testUser = null;
            hintResult = StringUtil.RemoveSign4VietnameseString(fullname).ToLower().Replace(" ", "");
            testUser = userService.findByUsername(hintResult + (startNumber == null ? "" : (startNumber + "")));
            if (testUser == null)
            {
                return hintResult + startNumber;
            }
            else
            {
                return getHintUsername(hintResult, (startNumber == null ? 1 : startNumber + 1));
            }

        }
        [HttpGet]
        [Authorize]
        public bool checkAvailableUsername(String username)
        {

            return userService.findByUsername(username) == null;

        }

        [HttpPost]
        [Authorize]
        public Object addMember(String fullname, String username, String profileImage, int gender, DateTime birthDate, String IDNumber, DateTime idDate, int relationShipLevel, String sendPasswordTo)
        {
            User curUser = userService.findById(int.Parse(User.Identity.GetUserId()));

            //DateTime bdate = DateTime.Parse(birthDate);
            User u = userService.findByUsername(username);
            if (u != null)
            {
                return false;
            }
            u = new User();
            u.Fullname = fullname;
            u.Username = username;
            u.DateOfBirth = birthDate;
            u.RoleId = SLIM_CONFIG.USER_ROLE_RESIDENT;
            u.Status = 0;
            u.HouseId = curUser.HouseId;
            u.Gender = gender;
            if (!IDNumber.Equals(""))
            {
                u.IDNumber = IDNumber;
                u.IDCreatedDate = idDate;
            }

            u.FamilyLevel = relationShipLevel;
            u.CreateDate = DateTime.Now;
            u.LastModified = DateTime.Now;
            //u.Password = "123123"; AnTT 4/7/2016
            u.Password = StringUtil.genPassword(); //AnTT 4/7/2016
            u.ProfileImage = profileImage;
            u.SendPasswordTo = sendPasswordTo;
            userService.addUser(u);
            return JsonConvert.SerializeObject(u);
        }
        [HttpPost]
        [Authorize]
        public bool deleteRequest(int id)
        {
            User u = userService.findById(id);

            if (u == null || u.Status == 1)
            {
                return false;
            }
            userService.deleteUser(u);
            return true;
        }

        [HttpPost]
        [Authorize]
        public void deleteNotification(String data, String type)
        {
            if (SLIM_CONFIG.NOTIC_DELETE_TYPE_CHANGEID.Equals(type))
            {
                notificationService.deleteNoticByNchangeID(int.Parse(data));
            }
        }

        [HttpPost]
        public ActionResult DeleteListNotification(List<int> notiIdList, string type)
        {
            MessageViewModels response = new  MessageViewModels();
            try
            {
                if (SLIM_CONFIG.NOTIC_DELETE_TYPE_CHANGEID.Equals(type))
                {
                    foreach (var id in notiIdList)
                    {
                        notificationService.deleteNoticByNchangeID(id);
                    }
                }
            }
            catch (Exception e)
            {
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult HouseHolderDeleteUser(int houseId, int houseHolderId, int deleteUserId)
        {
            User user = userService.findById(houseHolderId);
            User deleteUser = userService.findById(deleteUserId);
            House house = houseService.FindById(houseId);
            MessageViewModels response = new MessageViewModels();
            if (house != null && null != user && deleteUser != null && house.OwnerID != null && user.HouseId != null)
            {
                if (user.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER && user.HouseId == house.Id && house.OwnerID == user.Id)
                {
                    deleteUser.Status = SLIM_CONFIG.USER_STATUS_DELETE;
                    deleteUser.RoleId = SLIM_CONFIG.USER_ROLE_RESIDENT;
                    deleteUser.LastModified = DateTime.Now;
                    deleteUser.HouseId = null;
                    userService.updateUser(deleteUser);
                }
                else
                {
                    response.StatusCode = 2;
                }
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response);
        }
    }
}