using AMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public void saveListImage(List<String> imageURLs,int postId)
        {
            foreach(String url in imageURLs)
            {
                if(url!=null && !url.Equals(""))
                {
                    saveImage(url, postId);
                }
            }
        }
        private int saveImage(String url,int postId)
        {
            Image image = new Image();
            image.createdDate = DateTime.Now;
            image.postId = postId;
            image.url = url;
            imageRepository.Add(image);
            return image.id;

        }
        public List<Image> allImages()
        {
            return imageRepository.List.ToList();
        }

        public Image getImageByPostId(int id)
        {
            return imageRepository.List.FirstOrDefault(t => t.postId == id);
        }
        public List<Image> findImagesByPostId(int postId)
        {
            List<Image> result = new List<Image>();
            List<Image> allImage = allImages();
            foreach(Image m in allImage)
            {
                if(m.postId == postId)
                {
                    result.Add(m);
                }
            }
            return result;    
        }

    }
}