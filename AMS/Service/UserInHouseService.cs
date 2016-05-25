using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Reposiroty;
using AMS.ViewModel;

namespace AMS.Service
{
    public class UserInHouseService
    {
        GenericRepository<UserInHouse> userInHouseRepository = new GenericRepository<UserInHouse>();

        public void addMemberRequest(MemberViewModel member)
        {
            AMS.UserInHouse h = new AMS.UserInHouse();
            h.UserId = member.UserId;
            h.HouseId = member.HouseId;
            DateTime creatdate = DateTime.ParseExact(member.CreateDate, "yyyy/MM/dd",
                System.Globalization.CultureInfo.InvariantCulture);
            h.CreateDate = creatdate;
            userInHouseRepository.Add(h);
        }
    }
}