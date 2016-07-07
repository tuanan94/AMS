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

        public List<UserAnswerPoll> GetListUserAnswerPoll()
        {
            return userAnswerRepository.List.ToList();
        }
        public UserAnswerPoll FindById(int id)
        {
            return userAnswerRepository.FindById(id);
        }

       

        public void DeleteUserAnswer(UserAnswerPoll obj)
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

        public void AddUserAnswerPoll(UserAnswerPoll obj)
        {
            userAnswerRepository.Add(obj);
        }


        public int CountAnswer(string answerId, int PollId)
        {

            int a = userAnswerRepository.List.Count(t => t.Answer == answerId && t.PollId == PollId);
                
                  
                    
            return a;
        }

        public List<UserAnswerPoll> GetListUserAnswerPollsByPollId()
        {
            return userAnswerRepository.List.ToList().DistinctBy(t => t.PollId).ToList();
        }
        public List<UserAnswerPoll> GetListUserAnswerPollsByPollId(int id)
        {
            return userAnswerRepository.List.Where(t => t.PollId ==id).ToList();
        }
        public List<UserAnswerPoll> GetListUserAnswerPollsByUserId(int userId)
        {
            return userAnswerRepository.List.Where(t => t.UserId == userId).ToList();
        }
        public List<UserAnswerPoll> GetListUserAnswerPollsByAnswer(int id)
        {
            return userAnswerRepository.List.ToList().DistinctBy(t => t.Answer ).Where(t=>t.PollId== id).ToList();
        }
        public List<UserAnswerPoll> GetListUserAnswerPollsByAnswerN(int id)
        {
            return userAnswerRepository.List.ToList().Where(t => t.PollId == id).DistinctBy(t => t.Answer).ToList();
        }
    }
}