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
        public List<HelpdeskServiceCatModel> HdSrvCategories { get; set; }

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
    public class HelpdeskRequestModel
    {
        public int HdServiceId { get; set; }
        public string HdReqTitle { get; set; }
        public int HdReqPrior { get; set; }
        public int HdReqUserId { get; set; }
        public string HdReqUserDesc { get; set; }
        public int HdReqId { get; set; }
    }

    public class HelpdeskRequestTableModel
    {
        public int HdReqId { get; set; }
        public string HdReqTitle { get; set; }
        public int HdReqPrior { get; set; }
        public string HdReqHouse { get; set; }
        public int HdReqStatus { get; set; }
        public string HdReqSrvName { get; set; }
        public string HdReqCreateDate { get; set; }
        public string HdReqDeadline { get; set; }
        public string HdReqSupporter { get; set; }
    }

    public class HdRequestChangeStatusModel
    {
        public int HdReqId { get; set; }
        /*ToUserId just use when assign hdReq*/
        public int ToUserId { get; set; }
        public int FromUserId { get; set; }
        public int ToStatus { get; set; }
        public int FromStatus { get; set; }
        public double Price { get; set; }
        public string DueDate { get; set; }
    }

    public class HdRequestUpdatedModel
    {
        public int HdReqId { get; set; }
        public int HdReqStatus { get; set; }
        public string HdReqModifyDate { get; set; }
    }

    public class HdSuporterModel
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
    }

    public class HdReqDetailInfo
    {
        public List<HelpdeskServiceCatModel> HdSrvCategories { get; set; }
        public int SelectedHdSrvCatId { get; set; }
        public List<HelpdeskServiceModel> ListHdSrvBySelectedCat { get; set; }
        public int SelectedHdSrvId { get; set; }
        public double SelectedHdSrvPrice { get; set; }
        public HelpdeskRequestModel HdReqInfoDetail { get; set; }
    }
}