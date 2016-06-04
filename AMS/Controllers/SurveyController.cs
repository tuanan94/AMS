using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Service;
using AMS.ViewModel;

namespace AMS.Controllers
{
    public class SurveyController : Controller
    {
        SurveyService surveyService = new SurveyService();
        QuestionService questionService = new QuestionService();
        AnswerService answerService = new AnswerService();
        UserAnswerService userAnswerService = new UserAnswerService();
        //
        // GET: /Survey/
        public ActionResult Survey()
        {

            List<Survey> listSurveys = surveyService.GetListSurveys();
            //   int a = listCount.Count;
            ViewBag.ListSurvey = listSurveys;
            return View();
        }
        public ActionResult View1()
        {
         
           
            return View();
        }
        public ActionResult Graph()
        {


            return View();
        }
        public ActionResult DoSurvey()
        {
            List<Survey> listSurveys = surveyService.GetListSurveys();
            //   int a = listCount.Count;
            ViewBag.ListSurvey = listSurveys;

            return View();
        }
        public ActionResult DoDetailSurvey(int surveyId)
        {
             Survey survey = surveyService.FindById(surveyId);
           List<int> listCount = new List<int>();
            int a = userAnswerService.CountAnswer(332);
            List<Answer> listAnswers = answerService.FindByQuestionId(survey.Id);
            foreach (var item in listAnswers)
            {
                
            }
            ViewBag.Survey = survey;
            ViewBag.Answer = listAnswers;

            return View();
        }
        [HttpPost]
        public ActionResult DoDetailSurvey(SurveyViewModel model, string choiceId)
        {
           //string a= model.AnswerContent;
            string[] listQuestion = Request.Form.GetValues("question");


            string[] choice = Request.Form.GetValues("AnswerContent");
            string[] choiceIds = Request.Form.GetValues("answerId");
            List<string> listChoice = new List<string>(choice);
            List<string> listQuestions = new List<string>(listQuestion);
            int answerId = answerService.FindByContent(choice[0]);
            UserAnswerSurvey userAnswerSurvey = new UserAnswerSurvey();
            userAnswerSurvey.SurveyId = model.Id;
            userAnswerSurvey.UserId = 16;
            userAnswerSurvey.AnswerId = answerId;
            userAnswerService.AddUserAnswerSurvey(userAnswerSurvey);
           return RedirectToAction("DoDetailSurvey", new { surveyId = @model.Id });
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

        public ActionResult DetailSurvey(int surveyId)
        {
         

             Survey survey = surveyService.FindById(surveyId);
             List<Double> listCount = new List<Double>();
             List<Double> listCounts = new List<Double>();
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
                 Double percent = (answerCount / count) * 100;
                 listCounts.Add(percent);
             }
           
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
            obj.StartDate = model.StartDate;
            surveyService.UpdateSurvey(obj);
            return RedirectToAction("DetailSurvey", new { surveyId =obj.Id});
        }

        [HttpPost]
        public ActionResult Surveys(SurveyViewModel model)
        {

        //    string[] title = Request.Form.GetValues("Title");
            var ti = Request.QueryString["Title"];
            string[] listQuestion = Request.Form.GetValues("question");
            string[] listAnwser = Request.Form.GetValues("anwser1");
            string[] listCountAnwser = Request.Form.GetValues("count");
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
            int number= totalAnsertGroup.Count;
            int ii = 0;
            foreach (var content in listQuestions)
            {
                
                if (ii < number)
                {
                    survey.Title = model.Title;
                    string today = DateTime.Now.ToString("MM/dd/yyyy");
                    survey.StartDate = DateTime.Parse(today);
                    survey.Status = 1;
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
          //  return View("Survey");
            return RedirectToAction("Survey");
        }
    }
}