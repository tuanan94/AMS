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

    public class TransCategoryModel
    {
        public int TransCategoryId { get; set; }
        public string TransCategoryName { get; set; }
        public int TransItemType { get; set; }
    }

    public class ShortTransCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TransactionModel
    {
        public int BalanceSheetId { get; set; }
        public int TransId { get; set; }
        public string TransCatName { get; set; }
        public int TransCatId { get; set; }
        public int TransType { get; set; }
        public string TransForMonth { get; set; }
        public double TransTotalAmount { get; set; }
        public double TransPaidAmount { get; set; }
        public double TransUnpaidAmount { get; set; }
        public string TransTitle { get; set; }
        public string TransDesc { get; set; }
        public string TransCreateDate { get; set; }
        public string DT_RowId { get; set; }
    }

    public class AmountGroupByTransCategory
    {
        public int Id { get; set; }
        public string Name  { get; set; }
        public double TotalAmount { get; set; }
        public double PaidAmount { get; set; }
        public double UnpaidAmount { get; set; }
    }
}