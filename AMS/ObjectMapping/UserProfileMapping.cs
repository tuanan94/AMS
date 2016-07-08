using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.ObjectMapping
{
    public class UserProfileMapping
    {
        public int Id { get; set; }
        public String FullName { get; set; }
        public String ProfileImage { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }
        public String Email { get; set;}
        public int HouseId { get; set; }
        public String HouseName { get; set; }
        public String HouseProfile { get; set; }
        public List<MoreInfo> moreInfos { get; set; }
    }
    public class MoreInfo
    {
        public String Id { get; set; }
        public String createdDate { get; set; }
        public int type { get; set; }
    }
}