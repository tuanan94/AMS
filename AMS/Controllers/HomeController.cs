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
namespace AMS.Controllers
{
    public class HomeController : Controller
    {
        TestService testService = new TestService();
        UserInHouseService userInHouseService = new UserInHouseService();
        PostService postService = new PostService();
        public ActionResult Test()
        {
            List<House> allHouse = testService.getAllHouse();
            ViewBag.allHouse = allHouse;
            return View(allHouse);
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string Title, int PostId)
        {
            postService.createPost(Title, PostId);
            return RedirectToAction("TimeLine");
        }
        public ActionResult TimeLine()
        {
            // get all post
            IEnumerable<Post> allPost = postService.getAllPostNotDe();
            IEnumerable<Post> listComment = new List<Post>();
            ListPostViewModel listPostViewModel = new ListPostViewModel();
            listPostViewModel.listPost = new List<PostViewModel>();
            foreach (var item in allPost)
            {
                 PostViewModel  postViewModel = new PostViewModel();
                postViewModel.ImgUrl = item.ImgUrl;
                postViewModel.Id = item.Id;
                postViewModel.Title = item.Title;
                //get list comment belong post
                listComment = postService.getCommentBelongPost(postViewModel.Id);
                if (listComment !=null)
                {
                    postViewModel.Post = listComment;
                }
                listPostViewModel.listPost.Add(postViewModel);
              
            }
           
            //ViewBag.allPost = allPost;
            //ViewBag.listComment = listComment;
            return View(listPostViewModel);
        }

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
            return View();
        }
        [HttpGet]
        public ActionResult ManageMember()
        {
            return View();
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