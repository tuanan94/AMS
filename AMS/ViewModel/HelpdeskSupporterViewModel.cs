using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.ViewModel
{
    public class HelpdeskSupporterViewModel
    {

    }
    public class HelpdeskRequestViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreateDate { get; set; }
        public string CloseDate { get; set; }
        public string AssignDate { get; set; }
        public int Status { get; set; }
        public double Price { get; set; }
        public string HelpdeskServiceName{ get; set; }
        public int HelpdeskSupporterId { get; set; }
        public int ManagerId { get; set; }
        public string HouseName { get; set; }
        public System.DateTime ModifyDate { get; set; }
    
        public enum listStatus
        {
            Open = 0,
            Assigned = 1,
            Processing = 2,
            Done = 3,
            WaitForApproval = 4
        }

    }
}