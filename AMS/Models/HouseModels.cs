using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class HouseModels
    {
    }

    public class HouseCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UtilServiceForHouseCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string UtilSrvCatName { get; set; }
        public string HouseCat { get; set; }
        public int Type { get; set; }
        public string CreateDate { get; set; }
        public string DT_RowId { get; set; }

    }

}