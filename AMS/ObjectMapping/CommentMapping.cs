using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.ObjectMapping
{
    public class CommentMapping
    {
        public int id { get; set; }
        public int userId { get; set; }
        public String username { get; set; }
        public String fullName { get; set; }
        public String userProfile { get; set; }
        public String detail { get; set; }
        public DateTime createdDate { get; set; }
    }
}