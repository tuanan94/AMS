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
using Newtonsoft.Json;

namespace AMS.Controllers
{
    public class HomeController : Controller
    {
        TestService testService = new TestService();
        PostService postService = new PostService();
        public ActionResult Test()
        {
            List<House> allHouse = testService.getAllHouse();
            ViewBag.allHouse = allHouse;
            return View(allHouse);
        }
        
        [HttpPost]
        public void AddByAjax(string Title, int PostId)
        {
            postService.createPost(Title, PostId);
         
        }
         [HttpPost]
        public ActionResult Index(string Title, int PostId)
        {

            postService.createPost(Title, PostId);
            return RedirectToAction("TimeLine");
        }
        [HttpPost]
         public ActionResult Indexx(ListPostViewModel model, int postId)
        {

            postService.createPost(model.Title, model.Id);
            //return PartialView("TimeLine");
            return RedirectToAction("TimeLine");
        }
        
        public ActionResult TimeLine()
        {
            // get all post
            IEnumerable<Post> allPost = postService.getAllPost();
            IEnumerable<Post> listComment = new List<Post>();
            ListPostViewModel listPostViewModel = new ListPostViewModel();
            listPostViewModel.listPost = new List<PostViewModel>();
            foreach (var item in allPost)
            {
                 PostViewModel  postViewModel = new PostViewModel();
                postViewModel.ImgUrl = item.ImgUrl;
                postViewModel.Id = item.Id;
                postViewModel.Title = item.Title;
                postViewModel.CountComment = postService.CountComment(postViewModel.Id);
                if (item.CreateDate.HasValue)
                {
                    postViewModel.CreateDate = item.CreateDate.Value;
                }
             
                //get list comment belong post
                listComment = postService.getCommentBelongPost(postViewModel.Id);
                if (listComment !=null)
                {
                    postViewModel.Post = listComment;
                }
                listPostViewModel.listPost.Add(postViewModel);
              
            }
           
            
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
            post.CreateDate = DateTime.Now;
            postService.createPost(post);
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

            postService.CreatePosts(p);
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
       
    }
}