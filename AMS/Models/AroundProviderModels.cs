using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class AroundProviderModels
    {
    }

    [Serializable]
    public class AroundProviderDetailModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
    
    }
}