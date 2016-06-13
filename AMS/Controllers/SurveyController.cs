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
        HouseServices houseServices = new HouseServices();
        SurveyService surveyService = new SurveyService();
        QuestionService questionService = new QuestionService();
        AnswerService answerService = new AnswerService();
        UserAnswerService userAnswerService = new UserAnswerService();
        UserServices userService = new UserServices();
        //
        // GET: /Survey/
        public ActionResult Survey(string alerts)
        {
            List<House> listHouseBlock = userAnswerService.GetListBlock();
            List<string> listBlock = new List<string>();
            List<House> listHouseFloor = userAnswerService.GetListBlock();
            List<string> listFloor = new List<string>();
            foreach (var item in listHouseBlock)
            {
                listBlock.Add(item.Block);
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
                listSurveyId.Add(o.SurveyId.Value);

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

            List<Answer> listAnswers = answerService.FindByQuestionId(surveyId);

            List<Double> listCount = new List<Double>();
            List<string> listCountss = new List<string>();
            List<Survey> listSurveys = surveyService.GetListSurveysTop3();
            foreach (var item in listAnswers)
            {

                listCount.Add(userAnswerService.CountAnswer(item.Id));
            }
            Double count = listCount.Sum();
            foreach (var item in listAnswers)
            {
                Double answerCount = userAnswerService.CountAnswer(item.Id);
                // Double percent = Math.Round(((answerCount / count) * 100), 2);
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
                int answerId = answerService.FindByContent(strName, model.Id).Id;
                //var answerId = answerService.FindByContent(model.AnswerContent, model.Id).Id;

                userAnswerSurvey.AnswerId = answerId;
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

            List<House> listHouseBlock = userAnswerService.GetListBlock();
            List<string> listBlock = new List<string>();
            List<House> listHouseFloor = userAnswerService.GetListBlock();
            List<string> listFloor = new List<string>();
            foreach (var item in listHouseBlock)
            {
                listBlock.Add(item.Block);
            }
            foreach (var item in listHouseFloor)
            {
                listFloor.Add(item.Floor);
            }
            Survey survey = surveyService.FindById(surveyId);
            List<Double> listCount = new List<Double>();
            List<string> listCounts = new List<string>();
            int a = userAnswerService.CountAnswer(332);
            List<Answer> listAnswers = answerService.FindByQuestionId(survey.Id);
            foreach (var item in listAnswers)
            {

                listCount.Add(userAnswerService.CountAnswer(item.Id));
            }
            Double count = listCount.Sum();
            foreach (var item in listAnswers)
            {
                Double answerCount = userAnswerService.CountAnswer(item.Id);
                // Double percent = Math.Round(((answerCount / count) * 100), 2);
                string percent = string.Format("{0:00.0}", (answerCount / count * 100));
                listCounts.Add(percent);
            }
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
            string[] listBlock = Request.Form.GetValues("selectItemBlock");
            string[] listFloor = Request.Form.GetValues("selectItemFloor");
            string[] member = Request.Form.GetValues("people");
            string[] privacy = Request.Form.GetValues("privacy");
            string[] priority = Request.Form.GetValues("priority");
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
            Survey obj = surveyService.FindById(model.Id);
            // List<Question> questions = questionService.FindBySurveyId(obj.Id);

            //foreach (var itemQuestion in questions)
            //{
            int ii = 0;
            if (a < totalAnsertGroup.Count)
            {
                //List<string> listGroup = totalAnsertGroup[a];
                List<Answer> answers = answerService.FindByQuestionId(obj.Id);
                foreach (var itemAnswer in answers)
                {
                    //  itemAnswer.AnswerContent = listGroup[ii];
                    itemAnswer.AnswerContent = listAnwser[ii];
                    answerService.UpdateAnswer(itemAnswer);
                    ii++;
                }
                //itemQuestion.QuestionContent = listQuestion[a];
                //questionService.UpdateQuestion(itemQuestion);
                a++;
                //}
            }
            obj.Title = model.Title;
            obj.Question = listQuestion[0];
            obj.StartDate = (model.StartDate);
            obj.EndDate = (model.EndDate);
            obj.PublishDate = (model.PublishDate);
            //obj.StartDate = DateTime.Parse(model.StartDate);
            //obj.EndDate = DateTime.Parse(model.EndDate);
            //obj.PublishDate = DateTime.Parse(model.PublishDate);
            obj.Member = int.Parse(member[0]);
            obj.Block = listBlock[0];
            obj.Floor = listFloor[0];
            obj.Privacy = int.Parse(privacy[0]);
            obj.Priority = int.Parse(priority[0]);
            surveyService.UpdateSurvey(obj);
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
                string[] listBlock = Request.Form.GetValues("selectItemBlock");
                string[] listFloor = Request.Form.GetValues("selectItemFloor");
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
                Survey survey = new Survey();
                //survey.Title = model.Title;
                //string today = DateTime.Now.ToString("MM/dd/yyyy");
                //survey.StartDate = DateTime.Parse(today); 
                //survey.Status = 1;
                //surveyService.AddSurvey(survey);
                //add list question
                //Question question = new Question();
                Answer answer = new Answer();
                int number = totalAnsertGroup.Count;
                int ii = 0;
                foreach (var content in listQuestions)
                {

                    if (ii < number)
                    {
                        survey.Title = model.Title;
                        // string today = DateTime.Now.ToString("MM/dd/yyyy");
                        //   survey.StartDate = DateTime.Parse(today);
                        survey.StartDate = (model.StartDate);
                        // String.Format("{0:dd-MM-yyyy}", survey.StartDate);
                        //survey.EndDate = DateTime.Parse(model.EndDate);
                        //survey.PublishDate = DateTime.Parse(model.PublishDate);
                        survey.EndDate = (model.EndDate);
                        survey.PublishDate = (model.PublishDate);
                        int result = DateTime.Compare(survey.StartDate.Value, survey.EndDate.Value);
                        int result1 = DateTime.Compare(survey.PublishDate.Value, survey.EndDate.Value);
                        int result2 = DateTime.Compare(survey.StartDate.Value, survey.PublishDate.Value);
                        if (survey.StartDate.ToString() == "")
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
                        survey.Block = listBlock[0];
                        survey.Floor = listFloor[0];
                        survey.Privacy = int.Parse(privacy[0]);
                        survey.Member = int.Parse(member[0]);
                        survey.Priority = int.Parse(priority[0]);

                        survey.Question = content;
                        surveyService.AddSurvey(survey);
                        //question.QuestionContent = content;
                        //question.SurveyId = survey.Id;
                        //questionService.AddQuestion(question);

                        List<string> listAnwserGroupsub = totalAnsertGroup[ii];
                        foreach (var item in listAnwserGroupsub)
                        {
                            answer.QuestionId = survey.Id;
                            answer.AnswerContent = item;
                            answerService.AddAnswer(answer);

                        }
                        ii++;
                    }

                }
            }
            ViewBag.Alert = "Tạo survey thành công!!";
            //  return View("Survey");
            return RedirectToAction("Survey", new { alerts = "Tạo survey thành công!!" });
        }
    }
}