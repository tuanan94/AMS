using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Constant;
using LINQtoCSV;

namespace AMS.Models
{
    public class ReceiptViewModels
    {
    }

    public class ReceiptModel
    {
        public int ReceiptId { get; set; }
        public int BalanceSheetId { get; set; }
        public string ReceiptTitle { get; set; }
        public string ReceiptDesc { get; set; }
        public string ReceiptHouseName { get; set; }
        public int ReceiptType { get; set; }
        public string PublishDate { get; set; }
        public string ForMonth { get; set; }
        public int Creator { get; set; }
        public int Mode { get; set; }
        public List<OrderItemModel> ListItem { get; set; }
    }
    public class ReceiptInfoModel
    {
        public int ReceiptId { get; set; }
        public int TransactionId { get; set; }
        public string ReceiptTitle { get; set; }
        public double TotalOrder { get; set; }
        public string CreateDate { get; set; }
        public string Block { get; set; }
        public string Floor { get; set; }
        public string ForMonth { get; set; }
        public string HouseName { get; set; }
        public int Status { get; set; }
        public int IsAutomation { get; set; }
    }
    public class OrderItemModel
    {
        public int RecDetailId { get; set; }
        public string Name { get; set; }
        public float UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

    public class MonthlyResidentExpense
    {
        [CsvColumn(Name = "Tháng", OutputFormat = "MM-yyyy", FieldIndex = 1)]
        public string ForMonth { get; set; }

        [CsvColumn(Name = "Tòa nhà", FieldIndex = 2)]
        public string Block { get; set; }

        [CsvColumn(Name = "Tầng", FieldIndex = 3)]
        public string Floor { get; set; }

        [CsvColumn(Name = "Nhà", FieldIndex = 4)]
        public string HouseName { get; set; }

        [CsvColumn(Name = "Từ số", FieldIndex = 5)]
        public int FromNumber { get; set; }

        [CsvColumn(Name = "Đến số", FieldIndex = 6)]
        public int ToNumber { get; set; }
    }

    public class MonthlyResidentExpenseModel
    {
        public int HouseId { get; set; }
        public string Block { get; set; }
        public string Floor { get; set; }
        public string HouseName { get; set; }
        public string Month { get; set; }
        public int Water { get; set; }
        public int FromNumber { get; set; }
        public int ToNumber { get; set; }
        public double WaterCost { get; set; }
        public double Total { get; set; }
        public double Status { get; set; }
        public string DT_RowId { get; set; }
    }

    public class MonthlyReceiptModel
    {
        public int HouseId { get; set; }
        public int ReceiptId { get; set; }
        public string Block { get; set; }
        public string Floor { get; set; }
        public string HouseName { get; set; }
        public string Month { get; set; }
        public int Water { get; set; }
        public int FromNumber { get; set; }
        public int ToNumber { get; set; }
        public double WaterCost { get; set; }
        public double FixedCost { get; set; }
        public double Total { get; set; }
        public double Status { get; set; }
        public string DT_RowId { get; set; }
        public string PaymentDate { get; set; }
        public int NumberOfResident { get; set; }
        public List<UtilityServiceRangePriceModel> WaterRangePrices { get; set; }
    }

    public class AutomationReceiptsTemplateModel
    {
        public string Title { get; set; }
        public long ReceiptId { get; set; }
        public int BalanceSheetId { get; set; }
        public string ForMonth { get; set; }
        public string CreatedDate { get; set; }
        public string PublishDate { get; set; }
        public string LastModified { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int ElectricUtilServiceId { get; set; }
        public int WaterUtilServiceId { get; set; }
        public int HouseRentUtilServiceId { get; set; }
        public int FixCostUtilServiceId { get; set; }
        public List<MonthlyResidentExpenseModel> ResidentExpenseRecords { get; set; }
        public int UserId { get; set; }
    }

    public class MonthlyResidentExpenseOutput
    {
        [CsvColumn(Name = "Tháng", OutputFormat = "MM-yyyy", FieldIndex = 1)]
        public string ForMonth { get; set; }

        [CsvColumn(Name = "Tòa nhà", FieldIndex = 2)]
        public string Block { get; set; }

        [CsvColumn(Name = "Tầng", FieldIndex = 3)]
        public string Floor { get; set; }

        [CsvColumn(Name = "Nhà", FieldIndex = 4)]
        public string HouseName { get; set; }

        [CsvColumn(Name = "Từ số", FieldIndex = 5)]
        public string FromNumber { get; set; }

        [CsvColumn(Name = "Đến số", FieldIndex = 6)]
        public string ToNumber { get; set; }
    }

    public class UtilityServicesGroupByTypeModel
    {
        public List<UtilityService> ElectricSevices { get; set; }
        public List<UtilityService> WaterSevices { get; set; }
        public List<UtilityService> HouseRentSevices { get; set; }
        public List<UtilityService> FixedCostSevices { get; set; }
    }

    public class UtilityServiceIdNameModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class ReceiptRangePriceByHouseModel
    {
        public List<UtilityServiceRangePrice> RangePrices { get; set; }
        public int Count { get; set; }
    }

    public class ReceiptDetailTotalInBls
    {
        public ReceiptDetailTotalInBls(ReceiptDetail receiptDetail, double totalAmount)
        {
            ReceiptDetail = receiptDetail;
            TotalAmount = totalAmount;
        }

        public ReceiptDetail ReceiptDetail { get; set; }
        public double  TotalAmount { get; set; }
    }
}