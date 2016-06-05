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

        public int CountAnswer(int answerId)
        {

            int a =
                userAnswerRepository.List.Count(t => t.AnswerId == answerId);
                  
                    
            return a;
        }
    }
}