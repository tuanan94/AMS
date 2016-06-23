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
        GenericRepository<UserAnswerPoll> userAnswerRepository = new GenericRepository<UserAnswerPoll>();
        GenericRepository<House> houseRepository = new GenericRepository<House>();
        public List<UserAnswerPoll> GetListUserAnswerSurvey()
        {
            return userAnswerRepository.List.ToList();
        }
        public UserAnswerPoll FindById(int id)
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
        public void AddUserAnswerSurvey(UserAnswerPoll obj)
        {
            userAnswerRepository.Add(obj);
        }

        public int CountAnswer(int answerId)
        {

            //  int a = userAnswerRepository.List.Count(t => t.AnswerId == answerId);


            // return a;
            return -1;
        }

        public List<UserAnswerPoll> GetListUserAnswerSurveysBySurveyId()
        {
            return userAnswerRepository.List.ToList().DistinctBy(t => t.PollId).ToList();
        }
        public List<UserAnswerPoll> GetListUserAnswerSurveysBySurveyId(int id)
        {
            return userAnswerRepository.List.Where(t => t.PollId ==id).ToList();
        }
    }
}