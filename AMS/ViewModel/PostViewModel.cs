using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.ViewModel
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public int CountComment { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImgUrl { get; set; }
        public string PostStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateDate { get; set; }
        public int Status { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Media { get; set; }

        public IEnumerable<Post> Post { get; set; }  //comment
    }

    public class ListPostViewModel
    {
       public HttpPostedFileBase Media { get; set; }
       public string ImgUrl { get; set; }
       public List<PostViewModel> listPost
       {
           get;
           set;
       }
       public string Title { get; set; }
       public int Id { get; set; }
    }
}