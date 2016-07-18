using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Filter;
using AMS.Service;
using AMS.ViewModel;
using Microsoft.AspNet.Identity;

namespace AMS.Controllers
{
    public class SurveyController : Controller
    {
        NotificationService notificationService = new NotificationService();
        BlockService blockService = new BlockService();
        HouseServices houseServices = new HouseServices();
        PollService PollService = new PollService();
        QuestionService questionService = new QuestionService();
        AnswerService answerService = new AnswerService();
        UserAnswerService userAnswerService = new UserAnswerService();
        UserServices userService = new UserServices();
        BlockPollService BlockPollService = new BlockPollService();
        //
        // GET: /Survey/
        [Authorize]
        public ActionResult ViewHistory()
        {


         //   string[] counts = Request.Form.GetValues("Count");
            User currentUser = userService.FindById(int.Parse(User.Identity.GetUserId()));
            List<SurveyViewModel> listPolls = new List<SurveyViewModel>();
            List<List<string>> listAll = new List<List<string>>();
            List<List<Double>> listEach = new List<List<Double>>();
            List<List<string>> listCountAll = new List<List<string>>();
            List<List<Double>> listCountAlls = new List<List<Double>>();
            List<UserAnswerPoll> listUserAnswerPolls = userAnswerService.GetListUserAnswerPollsByUserId(currentUser.Id);
            foreach (var item in listUserAnswerPolls)
            {
                List<Double> listeach = new List<Double>();
                List<Double> listCount = new List<Double>();
                List<Double> listCountsss = new List<Double>();
                List<string> listCountss = new List<string>();
                List<string> listAnswers = new List<string>();
                SurveyViewModel model = new SurveyViewModel();
                model.Id = item.PollId;
                model.AnswerContent = item.Answer;
                model.Question = PollService.FindById(item.PollId).Question;
                model.ImageUrl = PollService.FindById(item.PollId).ImageUrl;
                model.AnswerDate = item.AnswerDate;
                listPolls.Add(model);
             //  List<UserAnswerPoll> listAnswerss = userAnswerService.GetListUserAnswerPollsByAnswer(item.PollId);
                List<UserAnswerPoll> listAnswerss = userAnswerService.GetListUserAnswerPollsByAnswerN(item.PollId);
                foreach (var items in listAnswerss)
                {

                    listCount.Add(userAnswerService.CountAnswer(items.Answer, items.PollId));


                }
                Double count = listCount.Sum();
                //foreach (var itema in listAnswerss)
                //{

                //    Double answerCount = userAnswerService.CountAnswer(itema.Answer, itema.PollId);
                //    listeach.Add(answerCount);
                //    string percent = string.Format("{0:00.0}", (answerCount / count * 100));
                //    listCountss.Add(percent);


                //}
                string answer1 = PollService.FindById(item.PollId).Answer1;
                Double answerCount1 = userAnswerService.CountAnswer(answer1, item.PollId);
                string percent1 = string.Format("{0:00.0}", (answerCount1 / count * 100));
                string answer2 = PollService.FindById(item.PollId).Answer2;
                Double answerCount2 = userAnswerService.CountAnswer(answer2, item.PollId);
                string percent2 = string.Format("{0:00.0}", (answerCount2 / count * 100));
                string answer3 = PollService.FindById(item.PollId).Answer3;
                Double answerCount3 = userAnswerService.CountAnswer(answer3, item.PollId);
                string percent3 = string.Format("{0:00.0}", (answerCount3 / count * 100));
                string answer4 = PollService.FindById(item.PollId).Answer4;
                Double answerCount4 = userAnswerService.CountAnswer(answer4, item.PollId);
                string percent4 = string.Format("{0:00.0}", (answerCount4 / count * 100));
                string answer5 = PollService.FindById(item.PollId).Answer5;
                Double answerCount5 = userAnswerService.CountAnswer(answer5, item.PollId);
                string percent5 = string.Format("{0:00.0}", (answerCount5 / count * 100));

                Double a1 = double.Parse(percent1);
                Double a2 = double.Parse(percent2);
                Double a3 = double.Parse(percent3);
                Double a4 = double.Parse(percent4);
                Double a5 = double.Parse(percent5);

                listCountsss.Add(a1);
                listCountsss.Add(a2);
                listCountsss.Add(a3);
                listCountsss.Add(a4);
                listCountsss.Add(a5);
                listCountAlls.Add(listCountsss);
               listeach.Add(answerCount1);
               listeach.Add(answerCount2);
               listeach.Add(answerCount3);
               listeach.Add(answerCount4);
               listeach.Add(answerCount5);
               listEach.Add(listeach);
                //listAnswers.Add(answer5);
                //listAnswers.Add(answer4);
                //listAnswers.Add(answer3);
                //listAnswers.Add(answer2);
                //listAnswers.Add(answer1);
                listAnswers.Add(answer1);
                listAnswers.Add(answer2);
                listAnswers.Add(answer3);
                listAnswers.Add(answer4);
                listAnswers.Add(answer5);

                listCountss.Add(percent1);
                listCountss.Add(percent2);
                listCountss.Add(percent3);
                listCountss.Add(percent4);
                listCountss.Add(percent5);
                listCountAll.Add(listCountss);
                listAll.Add(listAnswers);
            }
            List<Poll> lists = new List<Poll>();
            List<Poll> listSurveys = PollService.GetListPolls();
            List<UserAnswerPoll> listUserAnswer = new List<UserAnswerPoll>();
            int aa = 0;
            foreach (var item in listSurveys)
            {
                if (DateTime.Now >= item.PublishDate)
                {

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
                                //po.Question = item.Question;
                                //po.Answer1 = item.Answer1;
                                //po.Answer2 = item.Answer2;

                                //po.Answer3 = item.Answer3;
                                //po.Answer4 = item.Answer4;
                                //po.Answer5 = item.Answer5;
                                //po.EndDate = item.EndDate;
                                lists.Add(item);
                                aa++;
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
                                lists.Add(item);
                                aa++;
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
                                lists.Add(item);
                                aa++;
                            }
                        }

                    }

                    else if (listSurveys.Count == 0)
                    {

                    }
                }
                
            }

           ViewBag.Count = aa;
            ViewBag.Each = listEach;
            ViewBag.Answer = listAll;
            ViewBag.ListCount = listCountAlls;
            ViewBag.List = listPolls;
            return View();
        }

        public ActionResult ListPoll()
        {
            List<Poll> listSurveys = PollService.GetListPolls();
            ViewBag.ListSurvey = listSurveys;
            return View();
        }

        public ActionResult Survey(string alerts)
        {
            List<Block> listHouseBlock = blockService.GetListBlock();
            List<string> listBlock = new List<string>();
            List<House> listHouseFloor = userAnswerService.GetListBlock();
            List<string> listFloor = new List<string>();
            foreach (var item in listHouseBlock)
            {
                listBlock.Add(item.BlockName);
            }
            foreach (var item in listHouseFloor)
            {
                listFloor.Add(item.Floor);
            }
            List<Poll> listSurveys = PollService.GetListPolls();
            //   int a = listCount.Count;
            ViewBag.alerts = alerts;
            ViewBag.ListBlock = listBlock;
            ViewBag.ListFloor = listFloor;
            ViewBag.ListSurvey = listSurveys;
            return View();
        }
        public ActionResult View1()
        {


            return View();
        }

        public ActionResult View2()
        {
            return View();
        }
        public ActionResult Graph()
        {


            return View();
        }
        public ActionResult Graph1()
        {


            return View();
        }

        [AuthorizationPrivilegeFilter_RequestHouse]
        [Authorize]
        public ActionResult DoSurvey()
        {
            User currentUser = userService.FindById(int.Parse(User.Identity.GetUserId()));
            House currentHouse = null;
            if(currentUser.HouseId == null && currentUser.HouseId.HasValue)
            {
                currentHouse = houseServices.FindById(currentUser.HouseId.Value);
            }
            List<UserAnswerPoll> listUserAnswer = new List<UserAnswerPoll>();
            List<Poll> lists = new List<Poll>();
            List<Poll> listSurveys = PollService.GetListPolls();
            List<UserAnswerPoll> listUserAnswerPolls = userAnswerService.GetListUserAnswerPollsByPollId();
            List<UserAnswerPoll> listAnswerSurveys = userAnswerService.GetListUserAnswerPoll();
            List<UserAnswerPoll> list = new List<UserAnswerPoll>();
            List<int> listPollId = new List<int>();
            foreach (var o in listUserAnswerPolls)
            {
                listPollId.Add(o.PollId);

            }
            foreach (var obj in listPollId)
            {
                list = userAnswerService.GetListUserAnswerPollsByPollId(obj);
            }
            foreach (var item in listSurveys)
            {
                if (DateTime.Now >= item.PublishDate)
                {
                    
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
                                //po.Question = item.Question;
                                //po.Answer1 = item.Answer1;
                                //po.Answer2 = item.Answer2;
                               
                                //po.Answer3 = item.Answer3;
                                //po.Answer4 = item.Answer4;
                                //po.Answer5 = item.Answer5;
                                //po.EndDate = item.EndDate;
                                lists.Add(item);
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
                                lists.Add(item);
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
                                lists.Add(item);
                            }
                        }

                    }

                    else if (listSurveys.Count == 0)
                    {

                    }
                }
            }
            ViewBag.ListSurveys = lists;
            //   int a = listCount.Count;
            ViewBag.ListUserAnswerPoll = listAnswerSurveys;
            ViewBag.List = list;
            ViewBag.ListSurvey = listSurveys;
            ViewBag.ListUserAnswer = listPollId;
            ViewBag.currentUser = currentUser;
         //   ViewBag.currentHouse = currentHouse;
            return View();
        }
        public ActionResult DoDetailSurvey(int PollId)
        {
            Poll survey = PollService.FindById(PollId);
            User currentUser = userService.FindById(int.Parse(User.Identity.GetUserId()));
            House currentHouse = houseServices.FindById(currentUser.HouseId.Value);

           
            List<Poll> listSurveyss = PollService.GetListPolls();
            List<Double> listCount = new List<Double>();
            List<string> listCountss = new List<string>();
            List<Poll> listSurveys = PollService.GetListPollsTop3();
            string answer1 = PollService.FindById(PollId).Answer1;
            string answer2 = PollService.FindById(PollId).Answer2;
            string answer3 = PollService.FindById(PollId).Answer3;
            string answer4 = PollService.FindById(PollId).Answer4;
            string answer5 = PollService.FindById(PollId).Answer5;
            List<string> listAnswers = new List<string>();
            listAnswers.Add(answer1);
            listAnswers.Add(answer2);
            listAnswers.Add(answer3);
            listAnswers.Add(answer4);
            listAnswers.Add(answer5);
            List<UserAnswerPoll> listAnswerss = userAnswerService.GetListUserAnswerPollsByAnswer(PollId);
            foreach (var item in listAnswerss)
            {

                listCount.Add(userAnswerService.CountAnswer(item.Answer, PollId));


            }
            Double count = listCount.Sum();
            foreach (var item in listAnswerss)
            {

                Double answerCount = userAnswerService.CountAnswer(item.Answer, PollId);

                string percent = string.Format("{0:00.0}", (answerCount / count * 100));
                listCountss.Add(percent);


            }
            ViewBag.currentUser = currentUser;
            ViewBag.currentHouse = currentHouse;
            ViewBag.CountAnswer = listCountss;
            ViewBag.ListSurvey = listSurveys;
            ViewBag.Survey = survey;
            ViewBag.Answer = listAnswers;

            return View();
        }
        [HttpPost]
        public ActionResult DoDetailSurvey(SurveyViewModel model)
        {
            //string a= model.AnswerContent;
            string[] listQuestion = Request.Form.GetValues("question");
            User currentUser = userService.FindById(int.Parse(User.Identity.GetUserId()));
            String action = this.Request.QueryString["answer"];
            string forum = ((string)this.RouteData.Values["answer"] == null) ? (string)this.RouteData.Values["answer"] : "";

            //string[] choice = new string[] { Request.Form.Get("AnswerContent") };
            string[] choice = Request.Form.GetValues("answer");
            //string choice = Request.Form.Get("AnswerContent");
            // choice = Request.Form.GetValues("AnswerContent");
            //string[] choiceIds = Request.Form.GetValues("answerId");
            //List<string> listChoice = new List<string>(choice);
            //List<string> listQuestions = new List<string>(listQuestion);

            if (Request["answer"] != null)
            {
                string strName = Request["answer"].ToString();
                UserAnswerPoll UserAnswerPoll = new UserAnswerPoll();
                UserAnswerPoll.PollId = model.Id;
                UserAnswerPoll.UserId = currentUser.Id;
                //   UserAnswerPoll.UserId = 4;
               // string answerId = answerService.FindByContent(strName, model.Id).Id;
                //var answerId = answerService.FindByContent(model.AnswerContent, model.Id).Id;
                UserAnswerPoll.AnswerDate = DateTime.Now;
                UserAnswerPoll.Answer = strName;
                userAnswerService.AddUserAnswerPoll(UserAnswerPoll);
            }





            return RedirectToAction("DoSurvey");

            // return RedirectToAction("DoDetailSurvey", new { PollId = @model.Id });
        }
        public ActionResult DeleteSurvey(int PollId)
        {
            Poll obj = PollService.FindById(PollId);
            //List<Question> questions = questionService.FindByPollId(obj.Id);
            //foreach (var itemQuestion in questions)
            //{
            //    List<Answer> answers = answerService.FindByQuestionId(itemQuestion.Id);
            //    foreach (var itemAnswer in answers)
            //    {
            //        answerService.DeleteAnswer(itemAnswer);
            //    }
            //    questionService.DeleteQuestion(itemQuestion);
            //}
            List<UserAnswerPoll> listUserAnswerPolls = userAnswerService.GetListUserAnswerPollsByPollId(obj.Id);
            foreach (var item in listUserAnswerPolls)
            {
                userAnswerService.DeleteUserAnswer(item);
            }
            List<BlockPoll> listBlockPolls = BlockPollService.FindByPollId(obj.Id);
            foreach (var item1 in listBlockPolls)
            {
                BlockPollService.DeleteBlockPoll(item1);
            }
            PollService.DeletePoll(obj);

            return RedirectToAction("ListPoll");
        }

        public ActionResult DetailSurvey(int PollId, string alert)
        {
            List<Double> listeach = new List<Double>();
            List<Block> listHouseBlock = blockService.GetListBlock();
            List<string> listBlock = new List<string>();
            List<House> listHouseFloor = userAnswerService.GetListBlock();
            List<string> listFloor = new List<string>();
            foreach (var item in listHouseBlock)
            {
                listBlock.Add(item.BlockName);
            }
            foreach (var item in listHouseFloor)
            {
                listFloor.Add(item.Floor);
            }
            Poll survey = PollService.FindById(PollId);
            List<Double> listCount = new List<Double>();
            List<string> listCounts = new List<string>();
            //int a = userAnswerService.CountAnswer(332);
            List<string> listAnswers = new List<string>();
            List<UserAnswerPoll> listAnswerss = userAnswerService.GetListUserAnswerPollsByAnswer(PollId);
           foreach (var item in listAnswerss)
           {
              
                   listCount.Add(userAnswerService.CountAnswer(item.Answer, PollId)); 
              
               
           }
           Double count = listCount.Sum();
           foreach (var item in listAnswerss)
           {
              
                   Double answerCount = userAnswerService.CountAnswer(item.Answer,PollId);
               listeach.Add(answerCount);
                   string percent = string.Format("{0:00.0}", (answerCount / count * 100));
                   listCounts.Add(percent);
               
             
           }

            string answer1 = PollService.FindById(PollId).Answer1;
            string answer2 = PollService.FindById(PollId).Answer2;
            string answer3 = PollService.FindById(PollId).Answer3;
            string answer4 = PollService.FindById(PollId).Answer4;
            string answer5 = PollService.FindById(PollId).Answer5;
            listAnswers.Add(answer1);
            listAnswers.Add(answer2);
            listAnswers.Add(answer3);
            listAnswers.Add(answer4);
            listAnswers.Add(answer5);
            ViewBag.alert = alert;
            ViewBag.Each = listeach;
            ViewBag.ListBlock = listBlock;
            ViewBag.ListFloor = listFloor;
            ViewBag.CountAnswer = listCounts;
            ViewBag.Survey = survey;
            ViewBag.Answer = listAnswers;
            return View();
        }
        [HttpPost]
        public ActionResult UpdateSurvey(SurveyViewModel model)
        {
            string[] listQuestion = Request.Form.GetValues("question");
            string[] listAnwser = Request.Form.GetValues("anwser1");
            string[] listCountAnwser = Request.Form.GetValues("count");
            List<string> listCountAnwsers = new List<string>(listCountAnwser);
            List<string> listQuestions = new List<string>(listQuestion);
            List<string> listAnwsers = new List<string>(listAnwser);
           
            string[] member = Request.Form.GetValues("people");
            string[] privacy = Request.Form.GetValues("privacy");
            string[] priority = Request.Form.GetValues("priority");
            string[] status = Request.Form.GetValues("status");
            List<int> listCount = new List<int>();
        
            List<List<string>> totalAnsertGroup = new List<List<string>>();
            for (int i = 0; i < (listCountAnwsers.Count) - 1; i++)
            {
                if (Int32.Parse(listCountAnwsers[i]) > Int32.Parse(listCountAnwsers[i + 1]))
                {
                    listCount.Add(Int32.Parse(listCountAnwsers[i]));
                }
            }
            listCount.Add(Int32.Parse((listCountAnwsers[(listCountAnwsers.Count) - 1])));
            for (int i = 0; i < listCount.Count; i++)
            {
                List<string> listAnwserGroup = new List<string>();
                int countAneser = 0;
                for (int j = 0; j < listAnwsers.Count; j++)
                {
                    if (countAneser != listCount[i] && countAneser < listCount[i])
                    {
                        listAnwserGroup.Add(listAnwsers[j]);
                        countAneser++;
                        listAnwsers.Remove(listAnwsers[j]);
                        j = j - 1;
                    }
                }
                totalAnsertGroup.Add(listAnwserGroup);
            }
            //
            int a = 0;
            Poll obj = PollService.FindById(model.Id);

            if (listAnwser.Length == 2)
            {
                obj.Answer1 = listAnwser[0];
                obj.Answer2 = listAnwser[1];
            }
            else if (listAnwser.Length == 3)
            {
                obj.Answer1 = listAnwser[0];
                obj.Answer2 = listAnwser[1];
                obj.Answer3 = listAnwser[2];
            }
            else if (listAnwser.Length == 4)
            {
                obj.Answer1 = listAnwser[0];
                obj.Answer2 = listAnwser[1];
                obj.Answer3 = listAnwser[2];
                obj.Answer4 = listAnwser[3];
            }
            else if (listAnwser.Length == 5)
            {
                obj.Answer1 = listAnwser[0];
                obj.Answer2 = listAnwser[1];
                obj.Answer3 = listAnwser[2];
                obj.Answer4 = listAnwser[3];
                if (listAnwser.Length == 5)
                {
                    obj.Answer5 = listAnwser[(listAnwser.Length) - 1];
                }
            }
           
           
          
           

         
            obj.Description = model.Title;
            obj.Question = listQuestion[0];
           
            obj.EndDate = (model.EndDate);
            obj.PublishDate = (model.PublishDate);
            obj.Status = int.Parse(status[0]);
            obj.Mode = int.Parse(member[0]);
         
            obj.Priority = int.Parse(priority[0]);
            PollService.UpdatePoll(obj);
            string[] listBlock = Request.Form.GetValues("block");
            if (listBlock != null)
            {
                List<string> listBlockrs = new List<string>(listBlock);

                List<BlockPoll> listBlockPollsDb = BlockPollService.FindByPollId(model.Id);

                List<int> listEditBlock = new List<int>();
                List<int> listLoad = new List<int>();

                foreach (var VARIABLE in listBlockrs)
                {
                    listEditBlock.Add(blockService.FindBlockByName(VARIABLE).Id);
                }
                foreach (var aa in listBlockPollsDb)
                {
                    listLoad.Add(aa.BlockId);
                }
                if (listEditBlock.Count < listLoad.Count)
                {
                    List<int> list3 = listLoad.Except(listEditBlock).ToList();
                    // number of check less than before, delete
                    foreach (var object1 in list3)
                    {
                        BlockPoll BlockPoll = BlockPollService.FIndBlockPollByBlockIdPollId(object1, model.Id);
                        BlockPollService.DeleteBlockPoll(BlockPoll);
                    }
                }
                else if (listEditBlock.Count >= listLoad.Count)
                {
                    List<int> list4 = listEditBlock.Except(listLoad).ToList();
                    // number of check more than before, 
                    BlockPoll BlockPoll = new BlockPoll();
                    foreach (var obj1 in list4)
                    {
                        BlockPoll.BlockId = obj1;
                        BlockPoll.PollId = model.Id;
                        BlockPollService.AddBlockPoll(BlockPoll);
                    }

                }
            }
            else
            {
               

                List<BlockPoll> listBlockPollsDb = BlockPollService.FindByPollId(model.Id);

              
                List<int> listLoad = new List<int>();

            
                foreach (var aa in listBlockPollsDb)
                {
                    listLoad.Add(aa.BlockId);
                }
               
                    List<int> list3 = listLoad.ToList();
                    // number of check less than before, delete
                    foreach (var object1 in list3)
                    {
                        BlockPoll BlockPoll = BlockPollService.FIndBlockPollByBlockIdPollId(object1, model.Id);
                        BlockPollService.DeleteBlockPoll(BlockPoll);
                    }
                
            }
        
            List<int> aaa= new List<int>();
            aaa.Add(1);
            aaa.Add(2);
            List<int> aaa1 = new List<int>();
            aaa1.Add(2); 
            aaa1.Add(3); 
            aaa1.Add(4);
            List<int> asd = aaa1.Except(aaa).ToList();
            return RedirectToAction("DetailSurvey", new { PollId = obj.Id, alert = "Cập nhật thành công!" });
        }

        [HttpPost]
        public ActionResult Surveys(SurveyViewModel model)
        {
            if (ModelState.IsValid)
            {
                //    string[] title = Request.Form.GetValues("Title");
                var ti = Request.QueryString["Title"];
                string[] listQuestion = Request.Form.GetValues("question");
                string[] listAnwser = Request.Form.GetValues("anwser1");
                string[] listCountAnwser = Request.Form.GetValues("count");
             
                string[] imagUrl = Request.Form.GetValues("image_from_list");
                string[] member = Request.Form.GetValues("people");
                string[] privacy = Request.Form.GetValues("privacy");
                string[] priority = Request.Form.GetValues("priority");
                // string[] listMem = Request.Form.GetValues("count");
                //  string[] listCountAnwser = {"1", "2", "3","1","2","1","2","3","4"};
                List<string> listCountAnwsers = new List<string>(listCountAnwser);
                List<string> listQuestions = new List<string>(listQuestion);
                List<string> listAnwsers = new List<string>(listAnwser);
               
                // count number anwser of question
                List<int> listCount = new List<int>();
                List<List<string>> totalAnsertGroup = new List<List<string>>();
                for (int i = 0; i < (listCountAnwsers.Count) - 1; i++)
                {
                    if (Int32.Parse(listCountAnwsers[i]) > Int32.Parse(listCountAnwsers[i + 1]))
                    {
                        listCount.Add(Int32.Parse(listCountAnwsers[i]));
                    }
                }
                listCount.Add(Int32.Parse((listCountAnwsers[(listCountAnwsers.Count) - 1])));
                for (int i = 0; i < listCount.Count; i++)
                {
                    List<string> listAnwserGroup = new List<string>();
                    int countAneser = 0;
                    for (int j = 0; j < listAnwsers.Count; j++)
                    {
                        if (countAneser != listCount[i] && countAneser < listCount[i])
                        {
                            listAnwserGroup.Add(listAnwsers[j]);
                            countAneser++;
                            listAnwsers.Remove(listAnwsers[j]);
                            j = j - 1;
                        }
                    }
                    totalAnsertGroup.Add(listAnwserGroup);
                }
                Poll survey = new Poll();
             
                Answer answer = new Answer();
                int number = totalAnsertGroup.Count;
                int ii = 0;
                foreach (var content in listQuestions)
                {

                    if (ii < number)
                    {
                        survey.Description = model.Title;
                       
                      //  survey.CreateDate = (DateTime.Now);
                   
                        survey.EndDate = (model.EndDate);
                        survey.PublishDate = (model.PublishDate);
                       // int result = DateTime.Compare(survey.CreateDate.Value, survey.EndDate.Value);
                        int result1 = DateTime.Compare(survey.PublishDate.Value, survey.EndDate.Value);
                       // int result2 = DateTime.Compare(survey.CreateDate.Value, survey.PublishDate.Value);
                        //if (survey.CreateDate.ToString() == "")
                        //{
                        //    ModelState.AddModelError("StartDate", "Xin chon ngay hop ly.");
                        //}
                        //if ((result) < 0)
                        //{
                        //    ModelState.AddModelError("StartDate", "Xin chon ngay hop ly.");
                        //}
                        if ((result1) < 0)
                        {
                            ModelState.AddModelError("StartDate", "Xin chon ngay hop ly.");
                        }
                        //else if ((result2) < 0)
                        //{
                        //    ModelState.AddModelError("EndDate", "Xin chon ngay hop ly.");
                        //}
                        survey.Status = 1;
                 
                     //   survey.Privacy = int.Parse(privacy[0]);
                        survey.Mode = int.Parse(member[0]);
                        survey.Priority = int.Parse(priority[0]);
                        survey.ImageUrl = imagUrl[0];
                        survey.Question = content;
                        survey.Answer1 = listAnwser[0];
                        survey.Answer2 = listAnwser[1];
                        survey.Answer3 = listAnwser[2];
                        survey.Answer4 = listAnwser[3];
                        if (listAnwser.Length == 5)
                        {
                            survey.Answer5 = listAnwser[(listAnwser.Length)-1];
                        }
                      
                      
                        PollService.AddPoll(survey);
                      

                      
                    }
                    
                }
                int k = 0;
               // BlockPoll BlockPoll = new BlockPoll();
              
                string[] listBlock = Request.Form.GetValues("block");

                if (listBlock != null)
                {
                    List<string> listBlockrs = new List<string>(listBlock);
                    List<Block> list = new List<Block>();
                    foreach (var item in listBlockrs)
                    {

                        list.Add(blockService.FindBlockByName(item));

                    }
                    foreach (var obj in list)
                    {
                        BlockPoll BlockPoll = new BlockPoll();
                        BlockPoll.BlockId = obj.Id;
                        BlockPoll.PollId = survey.Id;
                        BlockPollService.AddBlockPoll(BlockPoll);


                    }
                    int p = 0;
                    // List<User> listuUsers = userService.GetAllResident();
                    List<User> listAllUsers = userService.GetAllUsers();
                    User curUser = userService.FindById(int.Parse(User.Identity.GetUserId()));
                    foreach (var u in listAllUsers)
                    {
                        if (u.HouseId != null)
                        {
                            int kk = 0;
                            if (list.ElementAt(p).Id == BlockPollService.FindBlockIdByHouseId(u.HouseId.Value).BlockId &&
                                p < list.Count)
                            {
                                kk++;
                            }
                            if (kk == 1)
                            {
                                Console.WriteLine(u);
                                //notificationService.addNotification("", u.Id, SLIM_CONFIG.NOTIC_VERB_POLL, 2, null);
                                notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_POLL, u.Id,
                                    SLIM_CONFIG.NOTIC_VERB_CREATE, curUser.Id, null);

                            }
                        }
                        else if (survey.Mode == 1)
                        {
                            int kk = 0;

                            if (u.HouseId.HasValue)
                            {
                                if (list.ElementAt(p).Id ==
                                    BlockPollService.FindBlockIdByHouseId(u.HouseId.Value).BlockId && p < list.Count)
                                {
                                    kk++;
                                }
                            }
                            if (kk == 1 || kk == 0)
                            {
                                Console.WriteLine(u);
                                //notificationService.addNotification("", u.Id, SLIM_CONFIG.NOTIC_VERB_POLL, 2, null);
                                notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_POLL, u.Id,
                                    SLIM_CONFIG.NOTIC_VERB_CREATE, curUser.Id, null);

                            }

                        }
                        else if (survey.Mode == 2 && SLIM_CONFIG.USER_ROLE_HOUSEHOLDER == u.RoleId)
                        {
                            int kk = 0;

                            if (u.HouseId.HasValue)
                            {
                                if (list.ElementAt(p).Id ==
                                    BlockPollService.FindBlockIdByHouseId(u.HouseId.Value).BlockId && p < list.Count)
                                {
                                    kk++;
                                }
                            }
                            if (kk == 1 || kk == 0)
                            {
                                Console.WriteLine(u);
                                //notificationService.addNotification("", u.Id, SLIM_CONFIG.NOTIC_VERB_POLL, 2, null);
                                notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_POLL, u.Id,
                                    SLIM_CONFIG.NOTIC_VERB_CREATE, curUser.Id, null);

                            }

                        }
                        else if ((survey.Mode == 3 && SLIM_CONFIG.USER_ROLE_HOUSEHOLDER == u.RoleId) ||
                            (survey.Mode == 3 && SLIM_CONFIG.USER_ROLE_RESIDENT == u.RoleId))
                        {
                            int kk = 0;

                            if (u.HouseId.HasValue)
                            {
                                if (list.ElementAt(p).Id ==
                                    BlockPollService.FindBlockIdByHouseId(u.HouseId.Value).BlockId && p < list.Count)
                                {
                                    kk++;
                                }
                            }
                            if (kk == 1 || kk == 0)
                            {
                                Console.WriteLine(u);
                                //notificationService.addNotification("", u.Id, SLIM_CONFIG.NOTIC_VERB_POLL, 2, null);
                                notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_POLL, u.Id,
                                    SLIM_CONFIG.NOTIC_VERB_CREATE, curUser.Id, null);

                            }

                        }
                    }
                }
                else
                {
                    // List<User> listuUsers = userService.GetAllResident();
                    List<User> listAllUsers = userService.GetAllUsers();
                    User curUser = userService.FindById(int.Parse(User.Identity.GetUserId()));
                    foreach (var u in listAllUsers)
                    {
                        if (survey.Mode == 1)
                        {
                                Console.WriteLine(u);
                                //notificationService.addNotification("", u.Id, SLIM_CONFIG.NOTIC_VERB_POLL, 2, null);
                                notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_POLL, u.Id, SLIM_CONFIG.NOTIC_VERB_CREATE, curUser.Id, null);
                        }else if (survey.Mode == 2 && SLIM_CONFIG.USER_ROLE_HOUSEHOLDER == u.RoleId)
                        {
                            notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_POLL, u.Id, SLIM_CONFIG.NOTIC_VERB_CREATE, curUser.Id, null);

                        }
                        else if ((survey.Mode == 3 && SLIM_CONFIG.USER_ROLE_HOUSEHOLDER == u.RoleId)||
                            (survey.Mode == 3 && SLIM_CONFIG.USER_ROLE_RESIDENT == u.RoleId))
                        {
                            notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_POLL, u.Id, SLIM_CONFIG.NOTIC_VERB_CREATE, curUser.Id, null);

                        }
                    }
                }
            }
            ViewBag.Alert = "Tạo survey thành công!!";
            //  return View("Survey");
            return RedirectToAction("ListPoll", new { alerts = "Tạo survey thành công!!" });
        }
    }

}