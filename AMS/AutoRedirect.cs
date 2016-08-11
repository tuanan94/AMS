using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AMS.Service;
using Microsoft.Owin.Security.Provider;
using Microsoft.AspNet.Identity;
namespace AMS
{
    public class AutoRedirect : ActionFilterAttribute
    {
        List<UserAnswerPoll> listUserAnswer = new List<UserAnswerPoll>();

        HouseServices houseServices = new HouseServices();
        BlockPollService BlockPollService = new BlockPollService();
        PollService surveyService = new PollService();
        UserAnswerService userAnswerService = new UserAnswerService();
        UserServices userServices = new UserServices();
        public string Auto(User currentUser)
        {
           
            List<Poll> listSurveys = surveyService.GetListPolls();
            string alert = "";
            foreach (var item in listSurveys)
            {
                if (DateTime.Now >= item.PublishDate && item.Priority == 1 && DateTime.Now <= item.EndDate)
                {
                   // User currentUser = userServices.FindById(int.Parse(User.Identity.GetUserId()));
                    listUserAnswer = (userAnswerService.GetListUserAnswerPollsByPollId(item.Id));
                    List<BlockPoll> listBlockPolls = BlockPollService.FindByPollId(item.Id);
                    int k = 0;
                    int p = 0;
                    if (listSurveys.Count != 0)
                    {
                        if (item.Mode == 1 && item.Status == 1)
                        {
                            foreach (var VARIABLE in listUserAnswer)
                            {
                                if (VARIABLE.UserId == currentUser.Id)
                                {
                                    k++;
                                }
                            }
                            foreach (var obj in listBlockPolls)
                            {
                                if (currentUser.HouseId != null)
                                {
                                    if (obj.BlockId == BlockPollService.FindBlockIdByHouseId(currentUser.HouseId.Value).BlockId)
                                    {
                                        p++;
                                    }
                                }

                            }
                            if ((k != 1 && p == 1 && item.Status == 1) )
                            //if (k != 1)
                            {
                                alert = "Bạn Có Một Survey Cần phải Làm!";
                            }
                            else if (k != 1 && listBlockPolls.Count == 0 && item.Status == 1)
                            {
                                alert = "Bạn Có Một Survey Cần phải Làm!";
                            }
                        }
                        else if (item.Mode == 2 && currentUser.RoleId == 4 && item.Status == 1)
                        {
                            foreach (var VARIABLE in listUserAnswer)
                            {
                                if (VARIABLE.UserId == currentUser.Id)
                                {
                                    k++;
                                }
                            }
                            foreach (var obj in listBlockPolls)
                            {
                                if (obj.BlockId == BlockPollService.FindBlockIdByHouseId(currentUser.HouseId.Value).BlockId)
                                {
                                    p++;
                                }
                            }
                            if (k != 1 && p == 1 && item.Status == 1)
                            // if (k != 1)
                            {
                                alert = "Bạn Có Một Survey Cần phải Làm!";
                            }
                            else if (k != 1 && listBlockPolls.Count == 0 && item.Status == 1)
                            {
                                alert = "Bạn Có Một Survey Cần phải Làm!";
                            }
                        }
                        else if ((item.Mode == 3 && currentUser.RoleId == 4 && item.Status == 1) || (item.Mode == 3 && currentUser.RoleId == 3 && item.Status == 1))
                        {
                            foreach (var VARIABLE in listUserAnswer)
                            {
                                if (VARIABLE.UserId == currentUser.Id)
                                {
                                    k++;
                                }
                            }
                            foreach (var obj in listBlockPolls)
                            {
                                if (currentUser.HouseId!=null &&  obj.BlockId == BlockPollService.FindBlockIdByHouseId(currentUser.HouseId.Value).BlockId)
                                {
                                    p++;
                                }
                            }
                            if (k != 1 && p == 1 && item.Status == 1)
                            // if (k != 1)
                            {
                                alert = "Bạn Có Một Survey Cần phải Làm!";
                            }
                            else if (k != 1 && listBlockPolls.Count == 0 && item.Status == 1)
                            {
                                alert = "Bạn Có Một Survey Cần phải Làm!";
                            }
                        }

                    }
                }
            }
            return alert;
        }
        public class MandatorySurveyRedirect : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                base.OnActionExecuting(context);

                /*Xử lý gì đó*/
                UserServices userServices = new UserServices();
                AutoRedirect autoRedirect = new AutoRedirect();
                User user = userServices.FindById(Int32.Parse(context.HttpContext.User.Identity.GetUserId()));
                string alert = autoRedirect.Auto(user);
                //if ((user.RoleId == SLIM_CONFIG.USER_ROLE_RESIDENT || user.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER) && user.House.OwnerID == null)
                //{
                //    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                //    {
                //        controller = "Home",
                //        action = "Index"
                //    }));
                //}
                if (!alert.Equals(""))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Survey",
                        action = "DoSurvey"
                    }));
                }
    
                /*Xử lý gì đó*/
            }
        }
    }
}
