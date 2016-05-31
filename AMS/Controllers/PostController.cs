using AMS.Service;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using AMS;

namespace AMS.Controllers
{
    public class PostController : Controller
    {
        UserService userService = new UserService();
        PostService postService = new PostService();
        ImageService imageService = new ImageService();
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public String Create(List<String> images, String content, string embeded)
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
            imageService.saveListImage(images,postId);
            return "success";
        }
        [HttpGet]
        [Authorize]
        public Object getPost(int? idToken)
        {
            List<Post> all = postService.getAllPost();
            
            // Serializer settings
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomResolver(typeof(Post));
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            // Do the serialization and output to the console
            string json = JsonConvert.SerializeObject(all,settings);
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
class CustomResolver : DefaultContractResolver
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