using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class UserViewModels
    {
    }

    public class ResidentGroupByTypeViewModel
    {
        public User User { get; set; }
        public int Count { get; set; }

        public ResidentGroupByTypeViewModel(User user, int count)
        {
            User = user;
            Count = count;
        }
    }

    public class UserInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Idenity { get; set; }
        public string HouseName { get; set; }
        public int HouseId { get; set; }
        public string Block { get; set; }
        public int BlockId { get; set; }
        public string Floor { get; set; }
        public int RoldId { get; set; }
        public int Gender { get; set; }
        public string RolName { get; set; }
        public string Dob { get; set; }
        public string HouseHolder { get; set; }
        public int HouseHolderid { get; set; }
        public int IsHouseOwner { get; set; }
        public int Status { get; set; }
        public string CreateDate { get; set; }
        public long CreateDateLong { get; set; }
        public string DT_RowId { get; set; }
        public string IdCreateDate { get; set; }
        public string UserAccountName { get; set; }
        public string CellNumb { get; set; }
        public int RelationLevel { get; set; }
        public int IsDeletable { get; set; }
    }
} 