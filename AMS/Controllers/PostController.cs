using AMS.Service;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using AMS;
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
        public String Create(List<String> images, List<String> thumbnailImages, String content, string embeded)
        {
            User curUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            if (curUser == null)
            {
                return "error";
            }
            Post p = new Post();
            p.CreateDate = DateTime.Now;
            p.UserId = curUser.Id;
            p.Body = content;
            p.PostStatus = SLIM_CONFIG.POST_STATUS_PUBLIC;
            p.EmbedCode = embeded;
            int postId =  postService.addPost(p);
            if(images == null)
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
            if (targetPost == null||targetPost.UserId==null)
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
            notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_POST, targetPost.UserId.Value, SLIM_CONFIG.NOTIC_VERB_COMMENT, curUser.Id,targetPost.Id);
            return "success";

        }


        [HttpGet]
        [Authorize]
        public Object getPost(int? idToken, int? houseId)
        {
            
            List<PostMapping> all = postService.getAllPostMapping(idToken,houseId);
            
            // Serializer settings
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomResolver(typeof(PostMapping));
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            // Do the serialization and output to the console
            string json = JsonConvert.SerializeObject(all,settings);
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
            if(postId!=null && postId.HasValue == true)
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
        public Object getCommentsForPost(int? postId)
        {
            // Serializer settings
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomResolver(typeof(CommentMapping));
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            // Do the serialization and output to the console
            string json = JsonConvert.SerializeObject(postService.comments(postId.Value), settings);
            return json;
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
    }
}
public class CustomResolver : DefaultContractResolver
{
    Type c;
    public CustomResolver(Type c){
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