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
        public double Total { get; set; }
        public double Qty { get; set; }
    }
    public class UtilityServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int HouseCatId { get; set; }
        public string HouseCatName { get; set; }
        public int UtilServiceForHouseCatId { get; set; }
        public List<UtilityServiceRangePriceModel> WaterUtilServiceRangePrices { get; set; }
        public FixedCostPriceModel FixedCostPrice { get; set; }
        public List<int> DeletedRangePrices { get; set; }
    }

    public class FixedCostModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public FixedCostPriceModel FixedCost { get; set; }
    }
    public class FixedCostPriceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}