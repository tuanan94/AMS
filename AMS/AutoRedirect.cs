using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Service;
using Microsoft.Owin.Security.Provider;
using Microsoft.AspNet.Identity;
namespace AMS
{
    public class AutoRedirect
    {
        List<UserAnswerPoll> listUserAnswer = new List<UserAnswerPoll>();

        HouseServices houseServices = new HouseServices();
        QuestionService questionService = new QuestionService();
        AnswerService answerService = new AnswerService();
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
                if (DateTime.Now >= item.PublishDate && item.Priority == 1)
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
                            if ((k != 1 && p == 1 && item.Status == 1) || (k != 1 && p == 0 && item.Status == 1))
                            //if (k != 1)
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
                        }

                    }
                }
            }
            return alert;
        }
    
    }
}
