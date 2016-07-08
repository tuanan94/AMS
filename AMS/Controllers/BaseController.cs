using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class BaseController : Controller
    {
        override protected void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string userName = User.Identity.Name;

        }
    }
}