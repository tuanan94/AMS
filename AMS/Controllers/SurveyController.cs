using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Service;
using AMS.ViewModel;
using Microsoft.AspNet.Identity;

namespace AMS.Controllers
{
    public class SurveyController : Controller
    {
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
           
         
          
            User currentUser = userService.FindById(int.Parse(User.Identity.GetUserId()));
            List<SurveyViewModel> listPolls = new List<SurveyViewModel>();
            List<List<string>> listAll = new List<List<string>>();
            List<List<Double>> listEach = new List<List<Double>>();
            List<List<string>> listCountAll = new List<List<string>>();
            List<UserAnswerPoll> listUserAnswerPolls = userAnswerService.GetListUserAnswerPollsByUserId(currentUser.Id);
            foreach (var item in listUserAnswerPolls)
            {
                List<Double> listeach = new List<Double>();
                List<Double> listCount = new List<Double>();
                List<string> listCountss = new List<string>();
                List<string> listAnswers = new List<string>();
                SurveyViewModel model = new SurveyViewModel();
                model.Id = item.PollId;
                model.AnswerContent = item.Answer;
                model.Question = PollService.FindById(item.PollId).Question;
                model.ImageUrl = PollService.FindById(item.PollId).ImageUrl;
                model.AnswerDate = item.AnswerDate;
                listPolls.Add(model);
                List<UserAnswerPoll> listAnswerss = userAnswerService.GetListUserAnswerPollsByAnswer(item.PollId);
                foreach (var items in listAnswerss)
                {

                    listCount.Add(userAnswerService.CountAnswer(items.Answer, items.PollId));


                }
                Double count = listCount.Sum();
                foreach (var itema in listAnswerss)
                {

                    Double answerCount = userAnswerService.CountAnswer(itema.Answer, itema.PollId);
                    listeach.Add(answerCount);
                    string percent = string.Format("{0:00.0}", (answerCount / count * 100));
                    listCountss.Add(percent);


                }
                string answer1 = PollService.FindById(item.PollId).Answer1;
                string answer2 = PollService.FindById(item.PollId).Answer2;
                string answer3 = PollService.FindById(item.PollId).Answer3;
                string answer4 = PollService.FindById(item.PollId).Answer4;
                string answer5 = PollService.FindById(item.PollId).Answer5;
                listEach.Add(listeach);
               listCountAll.Add(listCountss);
                listAnswers.Add(answer1);
                listAnswers.Add(answer2);
                listAnswers.Add(answer3);
                listAnswers.Add(answer4);
                listAnswers.Add(answer5);
                listAll.Add(listAnswers);
            }
            ViewBag.Each = listEach;
            ViewBag.Answer = listAll;
            ViewBag.ListCount = listCountAll;
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
        [Authorize]
        public ActionResult DoSurvey()
        {
            User currentUser = userService.FindById(int.Parse(User.Identity.GetUserId()));
            House currentHouse = houseServices.FindById(currentUser.HouseId.Value);
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
            //   int a = listCount.Count;
            ViewBag.ListUserAnswerPoll = listAnswerSurveys;
            ViewBag.List = list;
            ViewBag.ListSurvey = listSurveys;
            ViewBag.ListUserAnswer = listPollId;
            ViewBag.currentUser = currentUser;
            ViewBag.currentHouse = currentHouse;
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
            PollService.DeletePoll(obj);

            return RedirectToAction("Survey");
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
                       
                        survey.CreateDate = (DateTime.Now);
                   
                        survey.EndDate = (model.EndDate);
                        survey.PublishDate = (model.PublishDate);
                        int result = DateTime.Compare(survey.CreateDate.Value, survey.EndDate.Value);
                        int result1 = DateTime.Compare(survey.PublishDate.Value, survey.EndDate.Value);
                        int result2 = DateTime.Compare(survey.CreateDate.Value, survey.PublishDate.Value);
                        if (survey.CreateDate.ToString() == "")
                        {
                            ModelState.AddModelError("StartDate", "Xin chon ngay hop ly.");
                        }
                        if ((result) < 0)
                        {
                            ModelState.AddModelError("StartDate", "Xin chon ngay hop ly.");
                        }
                        else if ((result1) < 0)
                        {
                            ModelState.AddModelError("StartDate", "Xin chon ngay hop ly.");
                        }
                        else if ((result2) < 0)
                        {
                            ModelState.AddModelError("EndDate", "Xin chon ngay hop ly.");
                        }
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
                }

              
            }
            ViewBag.Alert = "Tạo survey thành công!!";
            //  return View("Survey");
            return RedirectToAction("Survey", new { alerts = "Tạo survey thành công!!" });
        }
    }

}