using AMS.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Helper;
using AMS.ViewModel;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using AMS.Constant;
using AMS.Enum;
using AMS.Models;
using AMS.ViewModel;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using AMS.ObjectMapping;

namespace AMS.Controllers
{
    public class HomeController : Controller
    {
        TestService testService = new TestService();
        PostService postService = new PostService();
        UserService userService = new UserService();
        HouseServices houseService = new HouseServices();
        NotificationService notificationService = new NotificationService();
        AroundProviderService _aroundProviderService = new AroundProviderService();
        AroundProviderProductService _aroundProviderProductService = new AroundProviderProductService();
        readonly string parternTime = "dd-MM-yyyy HH:mm";
        private const int foodId = 1;
        private const int entertainId = 2;
        private const int kidCornerId = 3;


        public ActionResult Test()
        {
            List<House> allHouse = testService.getAllHouse();
            ViewBag.allHouse = allHouse;
            return View(allHouse);
        }
        [Authorize]

        public ActionResult Index()
        {
            User curUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            
            if(curUser == null)
            {
                return View("error");
            }
            ViewBag.curUser = curUser;
            ViewBag.curHouse = curUser.House;
            weatherResult weatherResult = WeatherUtil.getWeatherResult();
            ViewBag.weather = weatherResult;
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
        public ActionResult ViewProfile()
        {
            User currentUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            ViewBag.user = currentUser;

            return View();
        }
        [HttpPost]
        [Authorize]
        public String saveProfileImage(int userid, String url)
        {
            User user = userService.findById(userid);
            if(user == null)
            {
                return "error";
            }
            else{
                user.ProfileImage = url;
            }
            userService.updateUser(user);
            return "success";
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
            List<User> members = userService.findByHouseId((user.HouseId.HasValue==true?user.HouseId.Value:-1));
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
        public ActionResult Setting()
        {
            User curUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            if(curUser == null)
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
        [HttpPost]
        [Authorize]
        public String getHintUsername(String fullname,int? startNumber)
        {
            
            String hintResult;
            User testUser = null;
            hintResult = StringUtil.RemoveSign4VietnameseString(fullname).ToLower().Replace(" ","");
            testUser = userService.findByUsername(hintResult+(startNumber==null?"":(startNumber+"")));
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

            return userService.findByUsername(username)==null;

        }

        [HttpGet]
        [Authorize]
        public Object addMember(String fullname, String username, String profileImage,int gender, DateTime birthDate, String IDNumber, DateTime idDate, int relationShipLevel)
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
            u.Gender = null;
            if (!IDNumber.Equals(""))
            {
                u.IDNumber = IDNumber;
                u.IDCreatedDate = idDate;
            }
            
            u.FamilyLevel = relationShipLevel;
            u.CreateDate = DateTime.Now;
            u.LastModified = DateTime.Now;
            u.Password = "123123";
            u.ProfileImage = profileImage;
            userService.addUser(u);
            return JsonConvert.SerializeObject(u);
        }
        [HttpPost]
        [Authorize]
        public bool deleteRequest(int id)
        {
            User u = userService.findById(id);

            if(u==null || u.Status == 1)
            {
                return false;
            }
            userService.deleteUser(u);
            return true;
        }
            public ActionResult ViewAroundProvider()
        {
            ViewBag.AllProviders = _aroundProviderService.GetAllProviders();
            return View();
        }

        public ActionResult ViewAroundProviderDetail()
        {
            ViewBag.AllProviders = _aroundProviderService.GetAllProviders();
            return View();
        }

        public ActionResult SingleProviderDetail(int id)
        {
            List<AroundProviderProduct> products = _aroundProviderProductService.GetAroundProviderProduct(id);
            AroundProvider curProvider = _aroundProviderService.GetProvider(id);
            string address = curProvider.Address;
            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", 
                                                Uri.EscapeDataString(address));
            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            var locationElement = result.Element("geometry").Element("location");
            var lat = locationElement.Element("lat");
            var lng = locationElement.Element("lng");
            //Double.Parse(lng.Value);
            ViewBag.Products = products;
            ViewBag.CurProvider = curProvider;
            ViewBag.Lat = Double.Parse(lat.Value);
            ViewBag.Lng = Double.Parse(lng.Value);

            return View();
        }
    }
}