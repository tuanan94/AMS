using AMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using AMS;
using AMS.Models;
using AMS.ObjectMapping;

namespace AMS.Controllers
{
    public class PostController : Controller
    {
        UserService userService = new UserService();
        PostService postService = new PostService();
        ImageService imageService = new ImageService();
        NotificationService notificationService = new NotificationService();
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public String Create(List<String> images, List<String> thumbnailImages, String content, string embeded, int? oldPostId)
        {
            User curUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            if (curUser == null)
            {
                return "error";
            }
            if (oldPostId != null)
            {
                Post oldPost = postService.findPostById(oldPostId.Value);
                if (oldPost != null)
                {
                    oldPost.Body = content;
                    postService.UpdatePost(oldPost);
                }
                return "success";
            }
            Post p = new Post();
            p.CreateDate = DateTime.Now;
            p.UserId = curUser.Id;
            p.Body = content;
            p.PostStatus = SLIM_CONFIG.POST_STATUS_PUBLIC;
            p.EmbedCode = embeded;
            int postId = postService.addPost(p);
            if (images == null)
            {
                images = new List<string>();
            }
            imageService.saveListImage(images, thumbnailImages, postId);
            return "success";
        }
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public String CreateComment(String detail, int postId)
        {
            User curUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            Post targetPost = postService.findPostById(postId);
            if (targetPost == null || targetPost.UserId == null)
            {
                return "error";
            }
            if (curUser == null)
            {
                return "error";
            }
            Comment c = new Comment();
            c.postId = postId;
            c.userId = curUser.Id;
            c.detail = detail;
            c.createdDate = DateTime.Now;
            postService.addComment(c);
            notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_POST, targetPost.UserId.Value, SLIM_CONFIG.NOTIC_VERB_COMMENT, curUser.Id, targetPost.Id);
            return "success";

        }


        [HttpGet]
        [Authorize]
        public Object getPost(int? idToken, int? houseId)
        {

            List<PostMapping> all = postService.getAllPostMapping(idToken, houseId);

            // Serializer settings
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomResolver(typeof(PostMapping));
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            // Do the serialization and output to the console
            string json = JsonConvert.SerializeObject(all, settings);
            return json;
        }

        [HttpGet]
        [Authorize]
        public Object getSinglePost(int postId)
        {
            PostMapping singlePost = postService.getSiglePost(postId);

            // Serializer settings
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomResolver(typeof(PostMapping));
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            // Do the serialization and output to the console
            string json = JsonConvert.SerializeObject(singlePost, settings);
            return json;


        }
        [HttpGet]
        [Authorize]
        public String getUserProfileForPost(int? postId)
        {
            String profileImage = "";
            if (postId != null && postId.HasValue == true)
            {
                Post p = postService.findPostById(postId.Value);
                profileImage = p.User.ProfileImage;
            }
            if (profileImage == null || profileImage.Equals(""))
            {
                profileImage = "/Content/images/defaultProfile.png";
            }
            return profileImage;
        }
        [HttpGet]
        [Authorize]
        public Object getUserForPost(int? postId)
        {
            if (postId == null)
            {
                return null;
            }
            Post p = postService.findPostById(postId.Value);
            if (p == null)
            {
                return null;
            }
            // Serializer settings
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomResolver(typeof(User));
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            // Do the serialization and output to the console
            string json = JsonConvert.SerializeObject(p.User, settings);
            return json;
        }
        [HttpGet]
        [Authorize]
        public Object getUserForComment(int? commentId)
        {
            if (commentId == null)
            {
                return null;
            }
            Comment c = postService.findCommentById(commentId.Value);
            if (c == null)
            {
                return null;
            }
            // Serializer settings
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomResolver(typeof(User));
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            // Do the serialization and output to the console
            string json = JsonConvert.SerializeObject(c.User, settings);
            return json;
        }
        [HttpGet]
        [Authorize]
        public ActionResult getCommentsForPost(int postId, int lastId)
        {
            // Serializer settings

            // Do the serialization and output to the console
            object data = null;

            List<Comment> allComment = null;
            List<Comment> lastFiveComment = null;
            if (lastId == 0)
            {
                allComment = postService.GetCommentByPostId(postId);
                lastFiveComment = allComment.Skip(allComment.Count - 5).ToList();
            }
            else
            {
                allComment = postService.GetCommentByPostIdHasSmallerId(postId, lastId);
                lastFiveComment = allComment.Skip(allComment.Count - 5).Reverse().ToList();
            }
            List<CommentMapping> result = new List<CommentMapping>();
            long lastGetComment = DateTime.Now.Ticks;
            foreach (Comment c in lastFiveComment)
            {
                CommentMapping cMapping = parseCommentToModel(c);
                cMapping.lastGetComment = lastGetComment;
                result.Add(cMapping);
            }
            if (result.Count == 0)
            {
                data = new { lastGetComment = DateTime.Now.Ticks };
            }
            else
            {
                int totalComment = 0;
                if (lastId == 0)
                {
                    totalComment = allComment.Count;
                }
                data = new { data = result, lastGetComment = DateTime.Now.Ticks, totalComment = totalComment };
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetNewCommentsForPost(int postId, int newestCommentId)
        {
            MessageViewModels reponse = new MessageViewModels();
            // Do the serialization and output to the console
            var listComment = postService.GetNewComment(postId, newestCommentId);
            int newestCommentIdUpdate = newestCommentId;
            if (listComment.Count != 0)
            {
                newestCommentIdUpdate = listComment[listComment.Count - 1].id;
            }
            object obj = new
            {
                listComment = listComment,
                lastGetComment = DateTime.Now.Ticks,
                newestCommentId = newestCommentIdUpdate
            };
            reponse.Data = obj;
            return Json(reponse, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public Object getImagesForPost(int? postId)
        {
            return JsonConvert.SerializeObject(imageService.findImagesByPostId(postId.Value), Formatting.Indented, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
        }


        // GET: Post
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public void deletePost(int postId)
        {
            Post p = postService.findPostById(postId);
            if (p != null)
            {
                p.Status = SLIM_CONFIG.POST_STATUS_HIDE;
            }
            postService.UpdatePost(p);
        }

        private CommentMapping parseCommentToModel(Comment c)
        {
            CommentMapping cMapping = new CommentMapping();
            cMapping.id = c.id;
            cMapping.detail = c.detail;
            cMapping.createdDate = c.createdDate.GetValueOrDefault().ToString("s");
            cMapping.username = c.User.Username;
            cMapping.fullName = c.User.Fullname;
            cMapping.userProfile = c.User.ProfileImage;
            cMapping.userId = c.userId.GetValueOrDefault();
            return cMapping;
        }
    }
}
public class CustomResolver : DefaultContractResolver
{
    Type c;
    public CustomResolver(Type c)
    {
        this.c = c;
    }
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty prop = base.CreateProperty(member, memberSerialization);

        if (prop.DeclaringType != c &&
            prop.PropertyType.IsClass &&
            prop.PropertyType != typeof(string))
        {
            prop.ShouldSerialize = obj => false;
        }

        return prop;
    }
}