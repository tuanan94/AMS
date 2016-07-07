using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using AMS.Service;
using Microsoft.AspNet.Identity;

namespace AMS.App_Start
{
    public class AuthorizationPrivilegeFilter : ActionFilterAttribute
    {
        // Source of this code using Google.com
        /*
         * http://stackoverflow.com/questions/3214774/how-to-redirect-from-onactionexecuting-in-base-controller
         * http://talenttuner.com/Blogs/MVC5/understanding-asp-net-mvc-filters/
         * ...
         */
        public class ManagerAdminAuthorize : FilterAttribute, IAuthenticationFilter
        {
            string superAdminRole = "Manager"; // can be taken from resource file or config file
            string adminRole = "Admin"; // can be taken from resource file or config file

            public void OnAuthentication(AuthenticationContext context)
            {
                if (context.HttpContext.User.Identity.IsAuthenticated &&
                    (context.HttpContext.User.IsInRole(superAdminRole)
                     || context.HttpContext.User.IsInRole(adminRole)))
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

        public class CheckHouseIsDeleted : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                bool isAuthenticated = httpContext.Request.IsAuthenticated;
                UserServices userServices = new UserServices();
                bool isInBlackList = true;
                User user = userServices.FindById(Int32.Parse(httpContext.User.Identity.GetUserId()));
                if ((user.RoleId == SLIM_CONFIG.USER_ROLE_RESIDENT || user.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER) && user.House.OwnerID == null)
                {
                    isInBlackList = false;
                }

                return isAuthenticated && !isInBlackList;
            }
        }

        public class MandatorySurveyRedirect : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                base.OnActionExecuting(context);

                /*Xử lý gì đó*/
                UserServices userServices = new UserServices();
                User user = userServices.FindById(Int32.Parse(context.HttpContext.User.Identity.GetUserId()));
                
                if ((user.RoleId == SLIM_CONFIG.USER_ROLE_RESIDENT || user.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER) && user.House.OwnerID == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index"
                    }));
                }
                /*Xử lý gì đó*/
            }
        }
    }
}