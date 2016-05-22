using System;
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
        public System.DateTime CloseDate { get; set; }
        public string AssignDate { get; set; }
        public int Status { get; set; }
        public double Price { get; set; }
        public string HelpdeskServiceName{ get; set; }
        public int HelpdeskSupporterId { get; set; }
        public int ManagerId { get; set; }
        public string HouseName { get; set; }
    
    }
}