using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;
using AMS.ViewModel;

namespace AMS.Service
{
    public class PollService
    {
        GenericRepository<Poll> PollRepository = new GenericRepository<Poll>();

        public List<Poll> GetListPolls()
        {
            return PollRepository.List.OrderByDescending(t=>t.Id ).OrderByDescending(t=>t.Priority).ToList();
        }

        public List<Poll> GetListPollsTop3()
        {
            return PollRepository.List.OrderByDescending(t => t.Id).OrderByDescending(t => t.Priority).Take(3).ToList();
        }
        public Poll FindById(int id)
        {
            return PollRepository.FindById(id);
        }
       


        public void AddPoll(Poll obj)
        {
            PollRepository.Add(obj);
        }

        public void DeletePoll(Poll obj)
        {
           
            PollRepository.Delete(obj);
        }

        public void UpdatePoll(Poll obj)
        {
            //Poll Poll = new Poll();
            //Poll.Title = obj.Title;
            PollRepository.Update(obj);
        }
    }
}