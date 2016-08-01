using AMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Text;
using AMS;
using AMS.Constant;
using AMS.Filter;
using AMS.Models;
using AMS.ObjectMapping;
using Microsoft.Ajax.Utilities;

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
        public String Create(List<String> images, List<String> thumbnailImages, List<String> originImages, String content, string embeded, int? oldPostId)
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
            p.EmbedCode = embeded;
            int postId = postService.addPost(p);
            if (images == null || thumbnailImages == null || originImages == null)
            {
                images = new List<string>();
                thumbnailImages = new List<string>();
                originImages = new List<string>();
            }
            imageService.saveListImage(images, thumbnailImages, originImages, postId);
            return "success";
        }
        [HttpPost]
        [Authorize]
        public ActionResult CreateComment(String detail, int postId)
        {
            User curUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            Post targetPost = postService.findPostById(postId);
            MessageViewModels response = new MessageViewModels();
            if (targetPost == null || targetPost.UserId == null)
            {
                response.StatusCode = -1;
                return Json(response);
            }
            if (curUser == null || curUser.Status != SLIM_CONFIG.USER_STATUS_ENABLE)
            {
                response.StatusCode = -1;
                return Json(response);
            }
            if (targetPost.Status == SLIM_CONFIG.POST_STATUS_HIDE)
            {
                response.StatusCode = 2;
                response.Data = postId;
                return Json(response);
            }
            Comment c = new Comment();
            c.postId = postId;
            c.userId = curUser.Id;
            c.detail = detail;
            c.createdDate = DateTime.Now;
            postService.addComment(c);
            notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_POST, targetPost.UserId.Value, SLIM_CONFIG.NOTIC_VERB_COMMENT, curUser.Id, targetPost.Id);
            return Json(response);
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
            MessageViewModels response = new MessageViewModels();
            object data = null;
            List<Comment> allComment = null;
            List<Comment> lastFiveComment = null;
            Post post = postService.findPostById(postId);
            if (null == post || post.Status == SLIM_CONFIG.POST_STATUS_HIDE)
            {
                response.StatusCode = 2;
                response.Data = postId;
                return Json(response, JsonRequestBehavior.AllowGet);
            }
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
                data = new { listComment = result, lastGetComment = DateTime.Now.Ticks, totalComment = totalComment };
            }
            response.Data = data;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetNewCommentsForPost(int postId, int newestCommentId)
        {
            MessageViewModels response = new MessageViewModels();
            Post post = postService.findPostById(postId);
            if (null == post || post.Status == SLIM_CONFIG.POST_STATUS_HIDE)
            {
                response.StatusCode = 2;
                response.Data = postId;
                return Json(response, JsonRequestBehavior.AllowGet);
            }

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
            response.Data = obj;
            return Json(response, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        [Authorize]
        [AuthorizationPrivilegeFilter_RequestHouse]
        [AutoRedirect.MandatorySurveyRedirect]
        public ActionResult Detail(int postId)
        {
            //Convert to post Mapping
            Post p = postService.findPostById(postId);
            List<string> imgUrl = new List<string>();

            PostMapping pMapping = new PostMapping();
            int imageCount = 0;

            if (p != null)
            {
                pMapping.Id = p.Id;
                pMapping.Status = p.Status;
                pMapping.Body = p.Body != null ? p.Body.Replace("\n", "<br/>") : "";
                pMapping.CreateDate = p.CreateDate.GetValueOrDefault();
                pMapping.EmbedCode = p.EmbedCode;
                pMapping.UserId = p.UserId;
                pMapping.username = p.User == null ? "Không xác định sở hữu" : p.User.Username;
                pMapping.userProfile = p.User == null || p.User.ProfileImage == null || p.User.ProfileImage.Equals("") ? "/Content/Images/defaultProfile.png" : p.User.ProfileImage;
                pMapping.userFullName = p.User == null ? "Không xác định" : p.User.Fullname;
                pMapping.houseName = p.User.House == null ? "Không xác định" : p.User.House.HouseName;
                pMapping.houseId = p.User.HouseId == null ? 0 : p.User.HouseId.Value;

                List<Image> listImages = imageService.GetImagesByPostId(p.Id);
                imageCount = listImages.Count;
                //                if (listImages.Count == 1)
                //                {
                //                    frameImageHeight = 476;
                //                    System.Drawing.Image img = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + listImages[0].thumbnailUrl);
                //                    imageCount = img.Height;
                //                }
                //                else if (listImages.Count > 1)
                //                {
                //                    frameImageHeight = ((int)listImages.Count / 2) * 237;
                //                }
            }
            else
            {
                ViewBag.ErrorMsg = "Rất tiếc! Bài viết này đã không còn tồn tại trong hệ thống.";
                return View("ErrorMsg");
            }
            ViewBag.Post = pMapping;
            ViewBag.ImageCount = imageCount;
            ViewBag.CurrentUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            return View("ViewSinglePost");
        }



        [HttpGet]
        public ActionResult GetPostDetail(int postId)
        {
            //Convert to post Mapping
            Post p = postService.findPostById(postId);
            MessageViewModels response = new MessageViewModels();
            PostMapping pMapping = new PostMapping();
            List<PostImageModel> listImage = new List<PostImageModel>();
            if (p != null)
            {
                pMapping.Id = p.Id;
                pMapping.Body = p.Body != null ? p.Body.Replace("\n", "<br/>") : "";
                pMapping.CreateDate = p.CreateDate.GetValueOrDefault();
                pMapping.EmbedCode = p.EmbedCode;
                pMapping.UserId = p.UserId;
                pMapping.username = p.User == null ? "Không xác định sở hữu" : p.User.Username;
                pMapping.userProfile = p.User == null || p.User.ProfileImage == null || p.User.ProfileImage.Equals("") ? "/Content/Images/defaultProfile.png" : p.User.ProfileImage;
                pMapping.userFullName = p.User == null ? "Không xác định" : p.User.Fullname;
                pMapping.houseName = p.User.House == null ? "Không xác định" : p.User.House.HouseName;
                pMapping.houseId = p.User.HouseId == null ? 0 : p.User.HouseId.Value;
                List<Image> listImages = imageService.GetImagesByPostId(p.Id);
                PostImageModel imgModel = null;
                foreach (var img in listImages)
                {
                    imgModel = new PostImageModel();
                    imgModel.id = img.id;
                    imgModel.postId = img.postId.Value;
                    imgModel.thumbnailurl = img.thumbnailUrl;
                    imgModel.userCropUrl = img.userCropUrl;
                    imgModel.url = img.url;
                    listImage.Add(imgModel);
                }
                pMapping.ListImages = listImage;
                response.Data = pMapping;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(PostMapping post)
        {
            //Convert to post Mapping
            Post p = postService.findPostById(post.Id);
            MessageViewModels response = new MessageViewModels();
            if (p != null)
            {
                p.Body = post.Body;
                p.UpdateDate = DateTime.Now;
                if (!post.EmbedCode.IsNullOrWhiteSpace())
                {
                    p.EmbedCode = post.EmbedCode;
                }
                else
                {
                    p.EmbedCode = null;
                }

                if (null != post.ListImgRemoved)
                {
                    foreach (var imgId in post.ListImgRemoved)
                    {
                        imageService.RemoveById(imgId);
                    }
                }

                if (null != post.ListImages)
                {
                    var listObjImages = post.ListImages;
                    foreach (var imgObj in listObjImages)
                    {
                        if (imgObj.id == 0)
                        {
                            Image eImg = new Image();
                            eImg.thumbnailUrl = imgObj.thumbnailurl;
                            eImg.userCropUrl = imgObj.thumbnailurl;
                            eImg.url = imgObj.url;
                            eImg.createdDate = DateTime.Now;
                            eImg.postId = p.Id;
                            eImg.originalUrl = imgObj.originUrl;
                            imageService.saveImage(eImg);
                        }
                        else
                        {
                            Image eImg = imageService.FindById(imgObj.id);
                            if (eImg != null && !eImg.thumbnailUrl.Equals(imgObj.userCropUrl))
                            {
                                eImg.thumbnailUrl = eImg.userCropUrl;
                                imageService.Update(eImg);
                            }
                        }
                    }

                    List<Image> postImage = imageService.GetImagesByPostId(p.Id).ToList();

                    bool isUpdateImage = false;
                    if (postImage.Count == 1)
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + postImage[0].originalUrl);
                        postImage[0].thumbnailUrl = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img, AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload, 504, 394, 0, false)).ToString(); ;
                        imageService.Update(postImage[0]);
                    }
                    else if (postImage.Count == 2)
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + postImage[0].originalUrl);
                        System.Drawing.Image img2 = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + postImage[1].originalUrl);
                        if (img.Height > img.Width && img2.Height > img2.Width) // Check 2 image is portrail 
                        {
                            postImage[0].thumbnailUrl = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img, AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload, 394, 394, 0, false)).ToString(); ;
                            postImage[1].thumbnailUrl = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img2, AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload, 394, 394, 0, false)).ToString(); ;
                            imageService.Update(postImage[0]);
                            imageService.Update(postImage[1]);
                        
                        }
                        else if (img.Width > img.Height && img2.Width > img2.Height)// Check 2 image is landscape
                        {
                            postImage[0].thumbnailUrl = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img, AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload, 504, 504, 0, false)).ToString(); ;
                            postImage[1].thumbnailUrl = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img2, AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload, 504, 504, 0, false)).ToString(); ;
                            imageService.Update(postImage[0]);
                            imageService.Update(postImage[1]);
                        }
                        else
                        {
                            foreach (var image in postImage)
                            {
                                image.thumbnailUrl = image.userCropUrl;
                                imageService.Update(image);
                            }
                        }
                    }
                    else if (postImage.Count == 3)
                    {
                        string curElemment = "";

                        /*Process thumbnail image*/
                        int index = -1;
                        for (int i = 0; i < postImage.Count; i++)
                        {
                            curElemment = postImage[i].originalUrl;
                            System.Drawing.Image img = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + curElemment);
                            if (img.Width > img.Height)
                            {
                                string imageSavePath = AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload;

                                postImage[i].thumbnailUrl = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img, imageSavePath, 504, 394, 0, false)).ToString(); ;

                                var tempItem = postImage[i].thumbnailUrl;
                                postImage[i].thumbnailUrl = postImage[0].thumbnailUrl;
                                postImage[0].thumbnailUrl = tempItem;

                                tempItem = postImage[i].originalUrl;
                                postImage[i].originalUrl = postImage[0].originalUrl;
                                postImage[0].originalUrl = tempItem;

                                tempItem = postImage[i].userCropUrl;
                                postImage[i].userCropUrl = postImage[0].userCropUrl;
                                postImage[0].userCropUrl = tempItem;

                                tempItem = postImage[i].url;
                                postImage[i].url = postImage[0].url;
                                postImage[0].url = tempItem;

                                imageService.Update(postImage[0]);
                                index = 0;
                                break;
                            }
                        }
                        Image image = null;
                        for (int i = 0; i < postImage.Count; i++)
                        {
                            image = postImage[i];
                            if (i != index)
                            {
                                image.thumbnailUrl = image.userCropUrl;
                                imageService.Update(image);
                            }
                        }
                    }
                }
                postService.UpdatePost(p);
                PostMapping newData = new PostMapping();
                newData.Id = p.Id;
                newData.Body = p.Body;
                newData.EmbedCode = p.EmbedCode;
                List<Image> listImages = imageService.GetImagesByPostId(p.Id);
                newData.ImageCount = listImages.Count;
                response.Data = newData;
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        // GET: Post
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult deletePost(int postId)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                Post p = postService.findPostById(postId);
                if (p != null)
                {
                    p.Status = SLIM_CONFIG.POST_STATUS_HIDE;
                    postService.UpdatePost(p);
                    response.Data = p.Id;
                }
            }
            catch (Exception e)
            {
                response.StatusCode = -1;
            }
            return Json(response);
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
        private PostImageModel parseImageToModel(Image i)
        {
            PostImageModel imgModel = new PostImageModel();
            imgModel.id = i.id;
            imgModel.originUrl = i.originalUrl;
            imgModel.thumbnailurl = i.thumbnailUrl;
            imgModel.url = i.url;
            imgModel.postId = i.postId.Value;
            return imgModel;
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