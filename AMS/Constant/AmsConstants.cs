using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Constant
{
    public static class AmsConstants
    {
        public static string DateFormat { get { return "dd-MM-yyyy"; } }
        public static string MonthYearFormat { get { return "MM-yyyy"; } }
        public static string MsgUserNotFound { get { return "Không tìm thấy người dùng"; } }
        public static string UtilityServiceElectricity { get { return "Điện"; } }
        public static string UtilityServiceWater { get { return "Nước"; } }
        public static string UtilityServiceHouseRent { get { return "Thuê nhà"; } }
        public static string UtilityServiceHelpdeskRequest { get { return "Hổ trợ sửa chữa"; } }
        public static string UtilityServiceFixedCost { get { return "Chi phí cố định"; } }
        public static string CsvFilePath { get { return "~/Upload/csv/"; } }
    }
}