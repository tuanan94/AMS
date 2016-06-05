using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class BalanceSheetViewModels
    {
    }

    public class TransItemCatModel
    {
        public int TransItemCatId { get; set; }
        public string TransItemCatName { get; set; }
        public int TransItemType { get; set; }
    }

    public class ShortTransItemCatModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TransItemModel
    {
        public int TransactionId { get; set; }
        public int TransItemId { get; set; }
        public string TransItemCatName { get; set; }
        public int TransItemCatId { get; set; }
        public string TransItemForMonth { get; set; }
        public double TransItemTotalAmount { get; set; }
        public double TransItemPaidAmount { get; set; }
        public double TransItemUnpaidAmount { get; set; }
        public string TransItemTitle { get; set; }
        public string TransItemDesc { get; set; }
        public string TransItemCreateDate { get; set; }
    }
}