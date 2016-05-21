using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;
using AMS.ViewModel;

namespace AMS.Service
{
    public class PendingMemberService
    {
        //GenericRepository<PendingMemberRequest> pendingMemberRepository = new GenericRepository<PendingMemberRequest>();
       
        //public void addMemberRequest(MemberViewModel member)
        //{
        //    PendingMemberRequest h = new PendingMemberRequest();
        //    h.Title = member.Title;
        //    h.Description = member.Description;
        //    //h.CreateDate = member.CreateDate.ToString("yyyyMMdd", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        //    DateTime creatdate = DateTime.ParseExact(member.CreateDate, "yyyy/MM/dd",
        //        System.Globalization.CultureInfo.InvariantCulture);
        //    h.CreateDate = creatdate;
        //    DateTime closedate = DateTime.ParseExact(member.CloseDate, "yyyy/MM/dd",
        //       System.Globalization.CultureInfo.InvariantCulture);
        //    h.CreateDate = closedate;
        //    h.Status = member.Status;
        //    h.HouseholderId = member.HouseholderId;
        //    h.PendingUserId = member.PendingUserId;
        //    h.ManagerId = member.ManagerId;
        //    pendingMemberRepository.Add(h);
        //}
    }
}