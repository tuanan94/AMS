using AMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Routing;

namespace AMS.Filter
{
    public class AuthorizationPrivilegeFilter_RequestHouse:ActionFilterAttribute
    {
        UserService userService = new UserService();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            User curUser = userService.findById(int.Parse(HttpContext.Current.User.Identity.GetUserId()));


            if (curUser.HouseId == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{{ "controller", "Error" },
                                          { "action", "ErrorNoHouse" }  });
            }
            base.OnActionExecuting(filterContext);
        }
    }
}