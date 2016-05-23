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
        PostService  postService = new PostService();
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
            return View("TimeLine");
        }
        public ActionResult TimeLine()
        {
            // get all post
            IEnumerable<Post> allPost = postService.getAllPost();
            // list of list comment
            List<IEnumerable<Post>> listCommentList = new List<IEnumerable<Post>>();
            IEnumerable<Post> commentsLists = new List<Post>();
            List<int> listPostId = new List<int>();
            foreach (var item in allPost)
            {
                if (item.PostId.ToString().Length > 0)
                {
                    // get postId of post
                    int i = item.PostId.Value;
                    //create list of postId
                    listPostId.Add(i);
                }
            }
            foreach (var id in listPostId)
            {
                // get all comment belong each post ???
                commentsLists = postService.getCommentBelongPost(id);
            }
            //for (int j = 0; j < listPostId.Count; j++)
            //{
            //    for (int k = j+1; k < listPostId.Count; k++)
            //    {
            //        if (listPostId[j].Equals(listPostId[k]))
            //        {
            //            //listPostId.Remove(k);
            //            listCommentList.Add(postService.getCommentBelongPost(listPostId[j]));
            //        }
            //        if (!listPostId[j].Equals(listPostId[k]))
            //        {
            //            commentsLists = postService.getCommentBelongPost(listPostId[j]);
            //        }  
            //    }
              
              
            //}
            ViewBag.listCommentList = listCommentList;
            ViewBag.commentsLists = commentsLists;
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