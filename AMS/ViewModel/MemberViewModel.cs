
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.ViewModel
{
    public class MemberViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreateDate { get; set; }
        public string CloseDate { get; set; }
        public int Status { get; set; }
        public int HouseholderId { get; set; }
        public int PendingUserId { get; set; }
        public int ManagerId { get; set; }
    }
}