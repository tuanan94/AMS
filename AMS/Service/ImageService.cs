using AMS.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AMS.Constant;
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
        public void saveListImage(List<String> imageURLs, List<String> thumbImageURLs, int postId)
        {
            string url = "";
            string thumbUrl = "";
            for (int i = 0; i < imageURLs.Count; i++)
            {
                url = imageURLs[i];
                thumbUrl = thumbImageURLs[i];
                if (url != null && !url.Equals(""))
                {
                    saveImage(url, thumbUrl, postId);
                }
            }
        }
        private int saveImage(String url, string thumbUrl, int postId)
        {
            Image image = new Image();
            image.createdDate = DateTime.Now;
            image.postId = postId;
            image.url = url;
            image.thumbnailUrl = thumbUrl;
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