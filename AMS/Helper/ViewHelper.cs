using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Helper
{
    public class ViewHelper
    {
        public static string Media(string path)
        {
            return path != null ? path.Replace("~/", Util.GetWebRoot()).Replace("\\", "/") : Util.GetWebRoot();
        }
    }
}