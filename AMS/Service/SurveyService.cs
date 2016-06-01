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
        GenericRepository<Survey> surveyRepository = new GenericRepository<Survey>();

        public List<Survey> GetListSurveys()
        {
            return surveyRepository.List.ToList();
        }
        public Survey FindById(int id)
        {
            return surveyRepository.FindById(id);
        }
       

        public void AddSurvey(Survey obj)
        {
            surveyRepository.Add(obj);
        }
        public void DeleteSurvey(Survey obj)
        {
           
            surveyRepository.Delete(obj);
        }
        public void UpdateSurvey(Survey obj)
        {
            //Survey survey = new Survey();
            //survey.Title = obj.Title;
            surveyRepository.Update(obj);
        }
    }
}