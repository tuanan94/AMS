using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.ViewModel
{
    public class MemberViewModel
    {
        [Required()]
        public int UserId { get; set; }
        [Required()]
        public int HouseId { get; set; }
        [Required()]
        //[RegularExpression(@"^\d{4}$|^\d{4}-((0?\d)|(1[012]))-(((0?|[12])\d)|3[01])$")]
        public string CreateDate { get; set; }
        [Required()]
        public int IsApproved { get; set; }
    }
}