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
        public long ClickCount { get; set; }
        public double RatePoint { get; set; }
        public double ProviderCatId { get; set; }
    }
    public class AroundServiceCatModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long Count { get; set; }
        public string DT_RowId { get; set; }

    }
    
    public class AroundServiceModel
    {
        public int SrvProvId { get; set; }
        public string SrvProvName { get; set; }
        public string SrvProvCatName { get; set; }
        public int SrvProvCatId { get; set; }
        public long SrvProvView { get; set; }
        public string SrvProvCreateDate { get; set; }
        public string SrvProvImageUrl { get; set; }
        public string SrvProvTel { get; set; }
        public string SrvProvAddress { get; set; }
        public string SrvProvGmapLink { get; set; }
        public long SrvProvCreateDateLong { get; set; }
        public string SrvProvDesc { get; set; }
        public string DT_RowId { get; set; }
    }

    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public string CreateDate { get; set; }
        public long CreateDateLong { get; set; }
        public int SrvProvId { get; set; }
        public string DT_RowId { get; set; }
    }
}