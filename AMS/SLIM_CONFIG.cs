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
    }
}