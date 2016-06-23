using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.ObjectMapping
{
    public class Notification
    {
        public String notification { get; set; }
        public int type { get; set; }
        public String source { get; set; }
        public DateTime date { get; set; }
    }
}