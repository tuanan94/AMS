using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class AnswerService
    {
        GenericRepository<Answer> answerRepository = new GenericRepository<Answer>();

        public List<Answer> GetListAnswer()
        {
            return answerRepository.List.ToList();
        }
        public Answer FindById(int id)
        {
            return answerRepository.FindById(id);
        }
        public List<Answer> FindByAnswerId(int id)
        {
            return answerRepository.List.Where(t => t.QuestionId == id).ToList();
        }

        public void AddAnswer(Answer obj)
        {
            answerRepository.Add(obj);
        }
    }
}