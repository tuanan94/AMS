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
        //
        // GET: /Survey/
        public ActionResult Survey()
        {

            List<Survey> listSurveys = surveyService.GetListSurveys();
            //   int a = listCount.Count;
            ViewBag.ListSurvey = listSurveys;
            return View();
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
                    answerService.DeleteAnswerByQuestionId(itemAnswer.Id);
                }
                questionService.DeleteQuestionBySurvetId(itemQuestion.Id);
            }
          surveyService.DeleteSurvey(obj);

            return RedirectToAction("Survey");
        }

        public ActionResult DetailSurvey(int surveyId)
        {
            

            return RedirectToAction("Survey");
        }
        [HttpPost]
        public ActionResult AddSurvey()
        {
           
           
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
            survey.Title = "Survey 1";
            surveyService.AddSurvey(survey);
            //add list question
            Question question = new Question();
            Answer answer = new Answer();
            int number= totalAnsertGroup.Count;
            int ii = 0;
            foreach (var content in listQuestions)
            {
                
                if (ii < number)
                {
                    question.QuestionContent = content;
                    question.SurveyId = survey.Id;
                    questionService.AddQuestion(question);

                    List<string> listAnwserGroupsub = totalAnsertGroup[ii];
                    foreach (var item in listAnwserGroupsub)
                    {
                        answer.QuestionId = question.Id;
                        answer.AnswerContent = item;
                        answerService.AddAnswer(answer);
                       
                    }
                    ii++;
                }

              
            }

            return RedirectToAction("Survey");
        }
    }
}