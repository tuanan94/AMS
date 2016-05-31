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
    }

}