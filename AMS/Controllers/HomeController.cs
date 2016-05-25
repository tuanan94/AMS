using AMS.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Helper;
using AMS.ViewModel;
using Microsoft.AspNet.Identity;
namespace AMS.Controllers
{
    public class HomeController : Controller
    {
        TestService testService = new TestService();
        UserInHouseService userInHouseService = new UserInHouseService();
        PostService  postService = new PostService();
        UserService userService = new UserService();
        public ActionResult Test()
        {
            List<House> allHouse = testService.getAllHouse();
            ViewBag.allHouse = allHouse;
            return View(allHouse);
        }
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TimeLine()
        {
            IEnumerable<Post> allPost = postService.getAllPost();
            ViewBag.allPost = allPost;
            return View();
        }
        [HttpPost]
        public ActionResult TimeLine(PostViewModel post, string Title, HttpPostedFileBase Media)
        {
            string mediaUrl = null;
          
            bool imageFlag = false;
            //if (ModelState.IsValid)
            //{
                if (post.Media != null && post.Media.ContentLength > 0)
                {
                    {
                      
                        // Save dir
                        var uploadDir = "~/images/Post/Binh";
                        // File extentions
                        var ext = "";
                        try
                        {
                            ext = Media.FileName.Substring(post.Media.FileName.LastIndexOf(".",
                                StringComparison.Ordinal));
                        }
                        catch (Exception)
                        {
                            ext = ".jpg";
                        }
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
                        if (Util.ConvertImageToJpg(uploadDir, tmpFileName, Constant.DefaultImageQuality, uploadDir,
                            fileName))
                        {
                            mediaUrl = Path.Combine(uploadDir, fileName);
                        
                            imageFlag = true;
                        }
                       
                        Util.DeleteFile(imagePath);
                    }


                }
                Post p = new Post();
               // post.Body = p.Body;
                post.Title = Title;
                post.ImgUrl = mediaUrl;
               
                postService.createPost(post);
            //}
            return View();
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
        public ActionResult ViewProfile()
        {
            return View();
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
            List<User> members = userService.findByHouseId(user.Id);
            ViewBag.currentHouse = user.House;
            ViewBag.members = members;
            return View();
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/Create")]
        public ActionResult CreateNewHdRequest()
        {
            return View("CreateHdRequest");
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/ViewHistory")]
        public ActionResult ViewHistoryHdRequest()
        {
            return View("ViewHistoryHdRequests");
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/AdminView")]
        public ActionResult ViewRequestingHdRequestToManager()
        {
            return View("ViewRequestingHdRequestsOfManager");
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
                    User newUser = new AMS.User();
                    newUser.Creator = curUser.Id;
                    newUser.Fullname = member.Fullname;
                    newUser.Username = curUser.House.HouseName + "_" + member.Username;
                    newUser.Password = member.Password;
                    newUser.Gender = member.Gender;
                    newUser.ProfileImage = member.ImageURL;
                    newUser.RoleId = SLIM_CONFIG.Role_RESIDENT;
                    newUser.HouseId = curUser.HouseId;
                    newUser.IsApproved = SLIM_CONFIG.USER_APPROVE_WAITING;
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
    }
}