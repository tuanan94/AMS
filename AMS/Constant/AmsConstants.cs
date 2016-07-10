using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Constant
{
    public static class AmsConstants
    {
        public static string DateFormat { get { return "dd-MM-yyyy"; } }
        public static string DateTimeFormat { get { return "dd-MM-yyyy HH:mm"; } }
        public static string MonthYearFormat { get { return "MM-yyyy"; } }
        public static string TimeFormat { get { return "HH:mm"; } }
        public static string MsgUserNotFound { get { return "Không tìm thấy người dùng"; } }
        public static string UtilityServiceElectricity { get { return "Điện"; } }
        public static string UtilityServiceWater { get { return "Nước"; } }
        public static string UtilityServiceHouseRent { get { return "Thuê nhà"; } }
        public static string UtilityServiceHelpdeskRequest { get { return "Dịch vụ sửa chữa"; } }
        public static string UtilityServiceFixedCost { get { return "Chi phí cố định"; } }

        public static string TransactionNameForWaterBill { get { return "Nước sinh hoạt dân cư"; } }
        public static string TransactionNameForFixedCostBill { get { return "Dịch vụ quản lý căn hộ"; } }

        public static string CsvFilePath { get { return "~/Upload/csv/"; } }
        public static string ImageFilePath { get { return "~/Upload/img/"; } }
        public static string ImageFilePathDownload { get { return "/Upload/img/"; } }

        public static string DefaultStoreImg { get { return "/Content/images/defaultStore.png"; } }
    }
}