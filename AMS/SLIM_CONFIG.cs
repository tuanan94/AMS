using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace AMS
{
    public class SLIM_CONFIG
    {  


        public static int PROGRAM_MODE_PRODUCT = 1;
        public static int PROGRAM_MODE_DEBUG = 0;

        /// <summary>
        /// Thay đổi Current_mode thành mode_debug sẽ bỏ 1 vài tính năng của survey
        /// </summary>
        public static int CURRENT_MODE = PROGRAM_MODE_DEBUG;

        public enum LoginResult { Success, NoUser, WrongPassword }
        public static String imagePath = "~/Images/";
        public static int Role_RESIDENT = 3;

        public static int USER_APPROVE_WAITING = 0;
        public static int USER_STATUS_ENABLE = 1;
        public static int USER_APPROVE_REJECT = 2;
        public static int USER_STATUS_DISABLE = 3;
        public static int USER_STATUS_DELETE = 4;

        public static int USER_SEX_MALE = 0;
        public static int USER_SEX_FEMALE = 1;
        public static int USER_SEX_ORTHER = 2;

        public static int USER_ROLE_ADMIN = 1;
        public static int USER_ROLE_MANAGER = 2;
        public static int USER_ROLE_RESIDENT = 3;
        public static int USER_ROLE_HOUSEHOLDER = 4;
        public static int USER_ROLE_SUPPORTER = 5;
        public static int POST_STATUS_PUBLIC = 0;
        public static int POST_STATUS_PROTECTED = 1;
        public static int POST_STATUS_HIDE = 2;
        public static int POST_NUMBER_SOCIAL_FEED = 4;

        public static String dirPostImage = "postImage";
        public static String dirHouseProfileImage = "houseImage";
        public static String dirProfileImage = "profileImage";

        public static int RECEIPT_STATUS_UNPUBLISHED = 1;
        public static int RECEIPT_STATUS_UNPAID = 2;
        public static int RECEIPT_STATUS_PAID = 3;

        public static int TRANSACTION_TYPE_INCOME = 1;
        public static int TRANSACTION_TYPE_EXPENSE = 2;

        public static int RECEIPT_ADD_MODE_SAVE = 1;
        public static int RECEIPT_ADD_MODE_SAVE_PUBLISH = 2;

        public static int NOTIFICATION_TYPE_MANAGERPOST = 0;

        public static int RECEIPT_TYPE_MANUAL = 1;
        public static int RECEIPT_TYPE_AUTOMATION = 2;

        public static int BALANCE_SHEET_OPEN = 1;
        public static int BALANCE_SHEET_CLOSE = 2;

        public static int UTILITY_SERVICE_TYPE_WATER = 2;
        public static int UTILITY_SERVICE_TYPE_HD_REQUEST = 4;
        public static int UTILITY_SERVICE_TYPE_FIXED_COST = 5;
        public static int UTILITY_SERVICE_TYPE_NOT_FOR_RESIDENT = 6;

        public static int UTILITY_SERVICE_GET_CONSUMPTION_COMPLETE = 1;
        public static int UTILITY_SERVICE_GET_CONSUMPTION_UN_COMPLETE = 2;

        public static int MODE_DELETE = 1;
        public static int MODE_PUBLISH = 2;

        public static int RESIDENT_IN_REGISTRATION_BOOK = 1;
        public static int RESIDENT_HAS_KT3 = 2;
        public static int RESIDENT_OTHER = 3;


        public static int UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE = 1;
        public static int UTILITY_SERVICE_OF_HOUSE_CAT_DISABLE = 2;
        public static int UTILITY_SERVICE_OF_HOUSE_CAT_REMOVED = 3;
        
        public static int RECEIPT_IS_ADDED_IN_BLS = 1;
        public static int RECEIPT_IS_NOT_ADDED_IN_BLS = 2;

        public static int TRANS_CAT_STATUS_ENABLE = 1;
        public static int TRANS_CAT_STATUS_DISABLE = 2;
        public static int TRANS_CAT_STATUS_DENY_REMOVE = 3;

        public static int HOUSE_STATUS_ENABLE = 1;
        public static int HOUSE_STATUS_DISABLE = 2;

        public static int HOUSE_HAS_RESIDENT = 1;
        public static int HOUSE_HAS_N0_RESIDENT = 2;

        public static int HOUSE_ADD_NEW_FLOOR = 0;
        public static int HOUSE_ADD_EXISTING_FLOOR = 1;

        public static int GET_ALL = 1;
        public static int GET_ALL_ACTIVE= 2;

        public static int TASK_DONE= 1;
        public static int TASK_NOT_DONE = 2;


        public static double MAX_NUMBER = 999999999.0;

        /* Weather API. 507468bd4d6345f2aa372826162306. Service Provider: http://developer.worldweatheronline.com/default.aspx */
        public static String WeatherAPI_KEY = "507468bd4d6345f2aa372826162306";
        public static String WeatherAPI_URL = "https://api.worldweatheronline.com/premium/v1/weather.ashx";


        public enum WeatherResult {Sun,Cloud,Rain,NoneDefined}


        //NotificationArea-START
        public static String NOTIC_TARGET_OBJECT_POST = "bài viết";
        public static String NOTIC_TARGET_OBJECT_ADD_MEMBER_REQUEST = "Yêu cầu thêm thành viên";
        public static String NOTIC_TARGET_OBJECT_POLL = "bình chọn";
        public static String NOTIC_TARGET_OBJECT_HELPDESK_REQUEST = "yêu cầu sửa chữa";
        public static String NOTIC_VERB_COMMENT = "bình luận";
        public static String NOTIC_VERB_APPROVE = "duyệt";
        public static String NOTIC_VERB_CREATE = "tạo";
        public static String NOTIC_VERB_ASSIGN_REQUEST = "chuyển";
        public static String NOTIC_VERB_FINISH_REQUEST = "xữ lí xong";
        public static String NOTIC_DELETE_TYPE_CHANGEID = "delete_type_changeId";
        //NotificationArea-END

    }

}