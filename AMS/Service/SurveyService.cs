using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;
using AMS.ViewModel;

namespace AMS.Service
{
    public class SurveyService
    {
        GenericRepository<Poll> surveyRepository = new GenericRepository<Poll>();

        public List<Poll> GetListSurveys()
        {
            return surveyRepository.List.OrderByDescending(t=>t.Id ).OrderByDescending(t=>t.Priority).ToList();
        }
        public List<Poll> GetListSurveysTop3()
        {
            return surveyRepository.List.OrderByDescending(t => t.Id).OrderByDescending(t => t.Priority).Take(3).ToList();
        }
        public Poll FindById(int id)
        {
            return surveyRepository.FindById(id);
        }
       

        public void AddSurvey(Poll obj)
        {
            surveyRepository.Add(obj);
        }
        public void DeleteSurvey(Poll obj)
        {
           
            surveyRepository.Delete(obj);
        }
        public void UpdateSurvey(Poll obj)
        {
            //Survey survey = new Survey();
            //survey.Title = obj.Title;
            surveyRepository.Update(obj);
        }
    }
}