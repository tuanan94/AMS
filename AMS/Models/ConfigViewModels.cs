using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class ConfigViewModels
    {
    }

    public class UtilityServiceRangePriceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FromAmount { get; set; }
        public string ToAmount { get; set; }
        public double Price { get; set; }
    }
    public class UtilityServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public List<UtilityServiceRangePriceModel> UtilityServicePriceRanges { get; set; }
    }

    public class FixedCostModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public List<FixedCostPriceModel> FixedCosts { get; set; }
    }
    public class FixedCostPriceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}