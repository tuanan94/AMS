using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;
using AMS.ViewModel;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Ast.Selectors;

namespace AMS.Service
{
    public class UserAnswerService
    {
        GenericRepository<UserAnswerSurvey> userAnswerRepository = new GenericRepository<UserAnswerSurvey>();
        GenericRepository<House> houseRepository = new GenericRepository<House>();
        public List<UserAnswerSurvey> GetListUserAnswerSurvey()
        {
            return userAnswerRepository.List.ToList();
        }
        public UserAnswerSurvey FindById(int id)
        {
            return userAnswerRepository.FindById(id);
        }

       

        public void DeleteUserAnswer(UserAnswerSurvey obj)
        {
            userAnswerRepository.Delete(obj);
        }

        public List<House> GetListBlock()
        {
            return houseRepository.List.ToList().DistinctBy(t => t.Block).ToList();
        }
        public List<House> GetListFloor()
        {
            return houseRepository.List.ToList().DistinctBy(t => t.Floor).ToList();
        }
        public void AddUserAnswerSurvey(UserAnswerSurvey obj)
        {
            userAnswerRepository.Add(obj);
        }

        public int CountAnswer(string answerId, int surveyId)
        {

            int a = userAnswerRepository.List.Count(t => t.Answer == answerId && t.SurveyId == surveyId);
                
                  
                    
            return a;
        }

        public List<UserAnswerSurvey> GetListUserAnswerSurveysBySurveyId()
        {
            return userAnswerRepository.List.ToList().DistinctBy(t => t.SurveyId).ToList();
        }
        public List<UserAnswerSurvey> GetListUserAnswerSurveysBySurveyId(int id)
        {
            return userAnswerRepository.List.Where(t => t.SurveyId ==id).ToList();
        }
        public List<UserAnswerSurvey> GetListUserAnswerSurveysByAnswer(int id)
        {
            return userAnswerRepository.List.ToList().DistinctBy(t => t.Answer ).Where(t=>t.SurveyId== id).ToList();
        }
    }
}