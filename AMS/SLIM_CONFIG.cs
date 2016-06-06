using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS
{
    public class SLIM_CONFIG
    {
        public enum LoginResult { Success, NoUser,WrongPassword}
        public static String imagePath = "~/Images/";
        public static int Role_RESIDENT = 3;
        public static int USER_APPROVE_YES=1;
        public static int USER_APPROVE_REJECT = 2;
        public static int USER_APPROVE_WAITING=0;
        public static int USER_SEX_MALE = 0;
        public static int USER_SEX_FEMALE = 1;
        public static int USER_ROLE_ADMIN = 1;
        public static int USER_ROLE_MANAGER = 2;
        public static int USER_ROLE_RESIDENT = 3;
        public static int USER_ROLE_HOUSEHOLDER = 4;
        public static int USER_ROLE_SUPPORTER = 5;
        public static int POST_STATUS_PUBLIC = 0;
        public static int POST_STATUS_PROTECTED = 1;
        public static int POST_STATUS_HIDE = 2;
        public static String dirPostImage = "postImage";
        public static int RECEIPT_STATUS_UNPUBLISHED = 1;
        public static int RECEIPT_STATUS_UNPAID = 2;
        public static int RECEIPT_STATUS_PAID = 3;

        public static int RECEIPT_TYPE_HD_REQUEST = 1;
        public static int RECEIPT_TYPE_ELECTRIC = 2;
        public static int RECEIPT_TYPE_WATER = 3;
        public static int RECEIPT_TYPE_HOUSE_RENT = 4;


        public static int TRANSACTION_TYPE_INCOME = 1;
        public static int TRANSACTION_TYPE_EXPENSE = 2;

        public static int RECEIPT_ADD_MODE_SAVE = 1;
        public static int RECEIPT_ADD_MODE_SAVE_PUBLISH = 2;




    }

}