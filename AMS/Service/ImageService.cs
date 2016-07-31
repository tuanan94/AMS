using AMS.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using AMS.Constant;
using AMS.Controllers;
using AMS.Models;

namespace AMS.Service
{
    public class ImageService
    {
        GenericRepository<Image> imageRepository;
        public ImageService()
        {
            imageRepository = new GenericRepository<Image>();
        }
        public void saveImage(Image image)
        {
            imageRepository.Add(image);
        }
        public void Update(Image image)
        {
            imageRepository.Update(image);
        }
        public Image FindById(int id)
        {
            return imageRepository.FindById(id);
        }
        public void RemoveById(int id)
        {
            var img = imageRepository.FindById(id);
            if (null != img)
            {
                imageRepository.Delete(img);
            }
        }
        public void saveListImage(List<string> imageURLs, List<string> thumbImageURLs, List<string> originImages, int postId)
        {
            string url = "";
            string thumbUrl = "";
            string userCropUrl = "";
            string oriUrl = "";
            List<string> userCropImg = thumbImageURLs.ToList();
            if (imageURLs.Count == 1)
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + originImages[0]);
                thumbImageURLs[0] = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img, AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload, 504, 394, 0, false)).ToString(); ;
            }
            else if (imageURLs.Count == 2)
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + originImages[0]);
                System.Drawing.Image img2 = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + originImages[1]);
                if (img.Height > img.Width && img2.Height > img2.Width)
                {
                    thumbImageURLs[0] = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img, AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload, 394, 394, 0, false)).ToString(); ;
                    thumbImageURLs[1] = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img2, AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload, 394, 394, 0, false)).ToString(); ;
                }
                else if (img.Width > img.Height && img2.Width > img2.Height)
                {
                    thumbImageURLs[0] = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img, AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload, 504, 504, 0, false)).ToString(); ;
                    thumbImageURLs[1] = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img2, AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload, 504, 504, 0, false)).ToString(); ;
                }
            }
            else if (originImages.Count == 3)
            {
                string curElemment = "";
                for (int i = 0; i < imageURLs.Count; i++)
                {
                    curElemment = originImages[i];
                    System.Drawing.Image img = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + curElemment);
                    if (img.Width > img.Height)
                    {
                        string imageSavePath = AppDomain.CurrentDomain.BaseDirectory + AmsConstants.ImageFilePathDownload;
                        thumbImageURLs[i] = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(AroundProviderController.SaveImage(img, imageSavePath, 504, 394, 0, false)).ToString(); ;

                        var tempItem = thumbImageURLs[i];
                        thumbImageURLs[i] = thumbImageURLs[0];
                        thumbImageURLs[0] = tempItem;

                        tempItem = originImages[i];
                        originImages[i] = originImages[0];
                        originImages[0] = tempItem;

                        tempItem = imageURLs[i];
                        imageURLs[i] = imageURLs[0];
                        imageURLs[0] = tempItem;
                        break;
                    }
                }
            }
            for (int i = 0; i < imageURLs.Count; i++)
            {
                url = imageURLs[i];
                thumbUrl = thumbImageURLs[i];
                oriUrl = originImages[i];
                userCropUrl = userCropImg[i];
                if (url != null && !url.Equals(""))
                {
                    saveImage(url, thumbUrl, userCropUrl, oriUrl, postId);
                }
            }
        }
        private int saveImage(string url, string thumbUrl, string userCropUrl, string originUrl, int postId)
        {
            Image image = new Image();
            image.createdDate = DateTime.Now;
            image.postId = postId;
            image.url = url;
            image.thumbnailUrl = thumbUrl;
            image.originalUrl = originUrl;
            image.userCropUrl = userCropUrl;
            imageRepository.Add(image);
            return image.id;

        }
        public List<Image> allImages()
        {
            return imageRepository.List.ToList();
        }
        public List<Image> GetImagesByPostId(int postId)
        {
            return imageRepository.List.Where(i => i.postId == postId).ToList();
        }
        public Image getImageByPostId(int id)
        {
            return imageRepository.List.FirstOrDefault(t => t.postId == id);
        }
        public List<PostImageModel> findImagesByPostId(int postId)
        {
            List<PostImageModel> result = new List<PostImageModel>();
            List<Image> allImage = GetImagesByPostId(postId);
            PostImageModel image = null;
            foreach (Image m in allImage)
            {
                image = new PostImageModel();
                image.id = m.id;
                image.url = m.url;
                image.thumbnailurl = m.thumbnailUrl;
                image.createdDate = m.createdDate.Value.ToString(AmsConstants.DateTimeFormat);
                image.postId = m.postId.Value;
                try
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + image.thumbnailurl);
                    image.width = img.Width;
                    image.height = img.Height;
                }
                catch (Exception)
                {
                    image.width = 0;
                    image.height = 0;
                }

                result.Add(image);
            }
            return result;
        }

    }
}