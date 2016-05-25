using AMS.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Helper;
using AMS.ViewModel;
namespace AMS.Controllers
{
    public class HomeController : Controller
    {
        TestService testService = new TestService();
        UserInHouseService userInHouseService = new UserInHouseService();
        PostService  postService = new PostService();
        public ActionResult Test()
        {
            List<House> allHouse = testService.getAllHouse();
            ViewBag.allHouse = allHouse;
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TimeLine()
        {
            List<Post> allPost = postService.getAllPost();
            ViewBag.allPost = allPost;
            return View();
        }
        [HttpPost]
        public ActionResult TimeLine(PostViewModel post)
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
                            ext = post.Media.FileName.Substring(post.Media.FileName.LastIndexOf(".",
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
                post.ImgUrl = mediaUrl;
                //var postnew = new Post()
                //{
                //    Body = "aaaaaaaa",
                //    Title = "aaaaaa",
                //    ImgUrl = mediaUrl

                //};
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
        public ActionResult ManageMember()
        {
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
        public ActionResult ManageMember(MemberViewModel member)
        {
            if (ModelState.IsValid)
            {
                userInHouseService.addMemberRequest(member);

            }
            return View();
            //pendingMemberService.addMemberRequest(member);

        }
    }
}