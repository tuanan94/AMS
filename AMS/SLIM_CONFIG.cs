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
    }
}