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
        SurveyService surveyService = new SurveyService();
        QuestionService questionService = new QuestionService();
        AnswerService answerService = new AnswerService();
        UserAnswerService userAnswerService = new UserAnswerService();
        UserServices userService = new UserServices();
        BlockSurveyService blockSurveyService = new BlockSurveyService();
        //
        // GET: /Survey/
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
            List<Survey> listSurveys = surveyService.GetListSurveys();
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
            List<Survey> listSurveys = surveyService.GetListSurveys();
            List<UserAnswerSurvey> listUserAnswerSurveys = userAnswerService.GetListUserAnswerSurveysBySurveyId();
            List<UserAnswerSurvey> listAnswerSurveys = userAnswerService.GetListUserAnswerSurvey();
            List<UserAnswerSurvey> list = new List<UserAnswerSurvey>();
            List<int> listSurveyId = new List<int>();
            foreach (var o in listUserAnswerSurveys)
            {
                listSurveyId.Add(o.SurveyId);

            }
            foreach (var obj in listSurveyId)
            {
                list = userAnswerService.GetListUserAnswerSurveysBySurveyId(obj);
            }
            //   int a = listCount.Count;
            ViewBag.ListUserAnswerSurvey = listAnswerSurveys;
            ViewBag.List = list;
            ViewBag.ListSurvey = listSurveys;
            ViewBag.ListUserAnswer = listSurveyId;
            ViewBag.currentUser = currentUser;
            ViewBag.currentHouse = currentHouse;
            return View();
        }
        public ActionResult DoDetailSurvey(int surveyId)
        {
            Survey survey = surveyService.FindById(surveyId);
            User currentUser = userService.FindById(int.Parse(User.Identity.GetUserId()));
            House currentHouse = houseServices.FindById(currentUser.HouseId.Value);

            List<string> listAnswers = new List<string>();
            List<Survey> listSurveyss = surveyService.GetListSurveys();
            List<Double> listCount = new List<Double>();
            List<string> listCountss = new List<string>();
            List<Survey> listSurveys = surveyService.GetListSurveysTop3();
            string answer1 = surveyService.FindById(surveyId).Answer1;
            string answer2 = surveyService.FindById(surveyId).Answer2;
            string answer3 = surveyService.FindById(surveyId).Answer3;
            string answer4 = surveyService.FindById(surveyId).Answer4;
            string answer5 = surveyService.FindById(surveyId).Answer5;
            listAnswers.Add(answer1);
            listAnswers.Add(answer2);
            listAnswers.Add(answer3);
            listAnswers.Add(answer4);
            listAnswers.Add(answer5);
            List<UserAnswerSurvey> listAnswerss = userAnswerService.GetListUserAnswerSurveysByAnswer(surveyId);
            foreach (var item in listAnswerss)
            {

                listCount.Add(userAnswerService.CountAnswer(item.Answer, surveyId));


            }
            Double count = listCount.Sum();
            foreach (var item in listAnswerss)
            {

                Double answerCount = userAnswerService.CountAnswer(item.Answer, surveyId);

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
                UserAnswerSurvey userAnswerSurvey = new UserAnswerSurvey();
                userAnswerSurvey.SurveyId = model.Id;
                userAnswerSurvey.UserId = currentUser.Id;
                //   userAnswerSurvey.UserId = 4;
               // string answerId = answerService.FindByContent(strName, model.Id).Id;
                //var answerId = answerService.FindByContent(model.AnswerContent, model.Id).Id;

                userAnswerSurvey.Answer = strName;
                userAnswerService.AddUserAnswerSurvey(userAnswerSurvey);
            }





            return RedirectToAction("DoSurvey");

            // return RedirectToAction("DoDetailSurvey", new { surveyId = @model.Id });
        }
        public ActionResult DeleteSurvey(int surveyId)
        {
            Survey obj = surveyService.FindById(surveyId);
            List<Question> questions = questionService.FindBySurveyId(obj.Id);
            foreach (var itemQuestion in questions)
            {
                List<Answer> answers = answerService.FindByQuestionId(itemQuestion.Id);
                foreach (var itemAnswer in answers)
                {
                    answerService.DeleteAnswer(itemAnswer);
                }
                questionService.DeleteQuestion(itemQuestion);
            }
            surveyService.DeleteSurvey(obj);

            return RedirectToAction("Survey");
        }

        public ActionResult DetailSurvey(int surveyId, string alert)
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
            Survey survey = surveyService.FindById(surveyId);
            List<Double> listCount = new List<Double>();
            List<string> listCounts = new List<string>();
            //int a = userAnswerService.CountAnswer(332);
            List<string> listAnswers = new List<string>();
            List<UserAnswerSurvey> listAnswerss = userAnswerService.GetListUserAnswerSurveysByAnswer(surveyId);
           foreach (var item in listAnswerss)
           {
              
                   listCount.Add(userAnswerService.CountAnswer(item.Answer, surveyId)); 
              
               
           }
           Double count = listCount.Sum();
           foreach (var item in listAnswerss)
           {
              
                   Double answerCount = userAnswerService.CountAnswer(item.Answer,surveyId);

                   string percent = string.Format("{0:00.0}", (answerCount / count * 100));
                   listCounts.Add(percent);
               
             
           }

            string answer1 = surveyService.FindById(surveyId).Answer1;
            string answer2 = surveyService.FindById(surveyId).Answer2;
            string answer3 = surveyService.FindById(surveyId).Answer3;
            string answer4 = surveyService.FindById(surveyId).Answer4;
            string answer5 = surveyService.FindById(surveyId).Answer5;
            listAnswers.Add(answer1);
            listAnswers.Add(answer2);
            listAnswers.Add(answer3);
            listAnswers.Add(answer4);
            listAnswers.Add(answer5);
            ViewBag.alert = alert;
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
            string[] listBlock = Request.Form.GetValues("block");
            string[] member = Request.Form.GetValues("people");
            string[] privacy = Request.Form.GetValues("privacy");
            string[] priority = Request.Form.GetValues("priority");
            List<int> listCount = new List<int>();
            List<string> listBlockrs = new List<string>(listBlock);
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
            Survey obj = surveyService.FindById(model.Id);
         
            obj.Answer1 = listAnwser[0];
            obj.Answer2 = listAnwser[1];
            obj.Answer3 = listAnwser[2];
            obj.Answer4 = listAnwser[3];
            if (listAnwser.Length == 5)
            {
                obj.Answer5 = listAnwser[(listAnwser.Length) - 1];
            }
           

         
            obj.Description = model.Title;
            obj.Question = listQuestion[0];
           
            obj.EndDate = (model.EndDate);
            obj.PublishDate = (model.PublishDate);
          
            obj.Mode = int.Parse(member[0]);
         
            obj.Priority = int.Parse(priority[0]);
            surveyService.UpdateSurvey(obj);

            int total = listBlockrs.Count;
            List<BlockSurvey> listBlockSurveysDb = blockSurveyService.FindBySurveyId(model.Id);
           List<BlockSurvey> listB= new List<BlockSurvey>();
            int totalAll = listBlockSurveysDb.Count;
         List<int> listEditBlock = new List<int>();
         List<int> listLoad = new List<int>();

            foreach (var VARIABLE in listBlockrs)
            {
                listEditBlock.Add(blockService.FindBlockByName(VARIABLE).Id);
            }
            foreach (var aa in listBlockSurveysDb)
            {
                listLoad.Add(aa.BlockId);
            }
         
           
          

            if (listEditBlock.Count < listLoad.Count)
            {
                List<int> list3 = listLoad.Except(listEditBlock).ToList();
                // number of check less than before, delete
                foreach (var object1 in list3)
                {
                    BlockSurvey blockSurvey = blockSurveyService.FIndBlockSurveyByBlockIdSurveyId(object1, model.Id);
                    blockSurveyService.DeleteBlockPoll(blockSurvey);
                }
            }
            else if (listEditBlock.Count == listLoad.Count)
            {
                // number of check equal than before, add
              
            }
            if (listEditBlock.Count > listLoad.Count)
            {
                List<int> list4 = listEditBlock.Except(listLoad).ToList();
                // number of check more than before, 
                  BlockSurvey blockSurvey = new BlockSurvey();
                  foreach (var obj1 in list4)
                {
                    blockSurvey.BlockId = obj1;
                    blockSurvey.SurveyId = model.Id;
                    blockSurveyService.AddBlockSurvey(blockSurvey);
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
            return RedirectToAction("DetailSurvey", new { surveyId = obj.Id, alert = "Cập nhật thành công!" });
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
                string[] listBlock = Request.Form.GetValues("block");
                string[] imagUrl = Request.Form.GetValues("image_from_list");
                string[] member = Request.Form.GetValues("people");
                string[] privacy = Request.Form.GetValues("privacy");
                string[] priority = Request.Form.GetValues("priority");
                // string[] listMem = Request.Form.GetValues("count");
                //  string[] listCountAnwser = {"1", "2", "3","1","2","1","2","3","4"};
                List<string> listCountAnwsers = new List<string>(listCountAnwser);
                List<string> listQuestions = new List<string>(listQuestion);
                List<string> listAnwsers = new List<string>(listAnwser);
                List<string> listBlockrs = new List<string>(listBlock);
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
                Survey survey = new Survey();
             
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
                      
                      
                        surveyService.AddSurvey(survey);
                      

                      
                    }
                    
                }
                int k = 0;
               // BlockSurvey blockSurvey = new BlockSurvey();
                List<Block> list = new List<Block>();
                foreach (var item in listBlockrs)
                {

                    list.Add(blockService.FindBlockByName(item));

                }
                foreach (var obj in list)
                {
                    BlockSurvey blockSurvey = new BlockSurvey();
                    blockSurvey.BlockId = obj.Id;
                    blockSurvey.SurveyId = survey.Id;
                    blockSurveyService.AddBlockSurvey(blockSurvey);


                }
            }
            ViewBag.Alert = "Tạo survey thành công!!";
            //  return View("Survey");
            return RedirectToAction("Survey", new { alerts = "Tạo survey thành công!!" });
        }
    }
}