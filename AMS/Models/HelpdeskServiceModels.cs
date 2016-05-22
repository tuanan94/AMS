using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class HelpdeskServiceModels
    {
         
    }

    public class HelpdeskServiceCatModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class HelpdeskSerivceCatListModel
    {
        public List<HelpdeskServiceCatModel> HdSrvCategories{ get;  set; }

        public HelpdeskSerivceCatListModel(List<HelpdeskServiceCatModel> hdSrvCategories)
        {
            HdSrvCategories = hdSrvCategories;
        }
    }

    public class HelpdeskServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Status { get; set; }
        public int HelpdeskServiceCategoryId { get; set; }
        public string HelpdeskServiceCategoryName { get; set; }
        public List<HelpdeskServiceCatModel> HdSrvCategories { get; set; }
    }
}