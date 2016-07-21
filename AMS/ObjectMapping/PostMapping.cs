using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.ObjectMapping
{
    public class PostMapping
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImgUrl { get; set; }
        public Nullable<int> PostStatus { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> UserId { get; set; }
        public string EmbedCode { get; set; }
        public Nullable<int> Disable { get; set; }
        public String username { get; set; }
        public String userProfile { get; set; }
        public String userFullName { get; set; }
        public string houseName { get; set; }
        public int houseId { get; set; }
    }
}