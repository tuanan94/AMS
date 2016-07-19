using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class ImageModels
    {

    }
    public class PostImageModel
    {
        public int id { get; set; }
        public string url { get; set; }
        public string thumbnailurl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string createdDate { get; set; }
        public int postId { get; set; }
    }
    
}