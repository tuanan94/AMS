using AMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Microsoft.AspNet.Identity;
using System.Web.Routing;

namespace AMS.Filter
{
    public class AuthorizationPrivilegeFilter_RequestHouse : ActionFilterAttribute
    {
        private UserServices userService;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            userService = DependencyResolver.Current.GetService<UserServices>();
            User curUser = userService.FindById(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

            if (curUser.HouseId == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"controller", "Error"},
                    {"action", "ErrorNoHouse"}
                });
            }
            base.OnActionExecuting(filterContext);
        }
    }

    /*
         * http://stackoverflow.com/questions/3214774/how-to-redirect-from-onactionexecuting-in-base-controller
         * http://talenttuner.com/Blogs/MVC5/understanding-asp-net-mvc-filters/
         * ...
         */

    public class ManagerAuthorize : FilterAttribute, IAuthenticationFilter
    {
        string managerRole = "Manager"; // can be taken from resource file or config file
        //        string adminRole = "Admin"; // can be taken from resource file or config file

        public void OnAuthentication(AuthenticationContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated &&
                (context.HttpContext.User.IsInRole(managerRole)))
            {
                // do nothing
            }
            else
            {
                context.Result = new HttpUnauthorizedResult(); // mark unauthorized
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext context)
        {
            if (context.Result == null || context.Result is HttpUnauthorizedResult)
            {
                context.Result = new RedirectToRouteResult("Default",
                    new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "Account"},
                        {"action", "Login"},
                        {"returnUrl", context.HttpContext.Request.RawUrl}
                    });
            }
        }
    }

    public class AdminAuthorize : FilterAttribute, IAuthenticationFilter
    {
        string adminRole = "Admin"; // can be taken from resource file or config file
        //        string adminRole = "Admin"; // can be taken from resource file or config file

        public void OnAuthentication(AuthenticationContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated &&
                (context.HttpContext.User.IsInRole(adminRole)))
            {
                // do nothing
            }
            else
            {
                context.Result = new HttpUnauthorizedResult(); // mark unauthorized
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext context)
        {
            if (context.Result == null || context.Result is HttpUnauthorizedResult)
            {
                context.Result = new RedirectToRouteResult("Default",
                    new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "Account"},
                        {"action", "Login"},
                        {"returnUrl", context.HttpContext.Request.RawUrl}
                    });
            }
        }
    }

    public class ManagerAdminAuthorize : FilterAttribute, IAuthenticationFilter
    {
        string adminRole = "Admin"; // can be taken from resource file or config file
        string managerRole = "Manager"; // can be taken from resource file or config file

        public void OnAuthentication(AuthenticationContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated &&
                (context.HttpContext.User.IsInRole(adminRole) || context.HttpContext.User.IsInRole(managerRole)))
            {
                // do nothing
            }
            else
            {
                context.Result = new HttpUnauthorizedResult(); // mark unauthorized
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext context)
        {
            if (context.Result == null || context.Result is HttpUnauthorizedResult)
            {
                context.Result = new RedirectToRouteResult("Default",
                    new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "Account"},
                        {"action", "Login"},
                        {"returnUrl", context.HttpContext.Request.RawUrl}
                    });
            }
        }
    }

    public class SupporterAuthorize : FilterAttribute, IAuthenticationFilter
    {
        string managerRole = "HelpdeskSupporter"; // can be taken from resource file or config file
        //        string adminRole = "Admin"; // can be taken from resource file or config file

        public void OnAuthentication(AuthenticationContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated &&
                (context.HttpContext.User.IsInRole(managerRole)))
            {
                // do nothing
            }
            else
            {
                context.Result = new HttpUnauthorizedResult(); // mark unauthorized
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext context)
        {
            if (context.Result == null || context.Result is HttpUnauthorizedResult)
            {
                context.Result = new RedirectToRouteResult("Default",
                    new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "Account"},
                        {"action", "Login"},
                        {"returnUrl", context.HttpContext.Request.RawUrl}
                    });
            }
        }
    }
}