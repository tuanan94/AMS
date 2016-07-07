using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class QuestionService
    {
        GenericRepository<Question> questionRepository = new GenericRepository<Question>();

        public List<Question> GetListQuestion()
        {
            return questionRepository.List.ToList();
        }
        public Question FindById(int id)
        {
            return questionRepository.FindById(id);
        }
        //public List<Question> FindByPollId(int id)
        //{
        //    return questionRepository.List.Where(t=>t.PollId==id).ToList();
        //}

        //public void DeleteQuestionBySurvetId(int id)
        //{
        //    List<Question> obj = questionRepository.List.Where(t => t.PollId == id).ToList();
        //    foreach (var item in obj)
        //    {
        //        questionRepository.Delete(item);
        //    }
          
        //}
        public void AddQuestion(Question obj)
        {
            questionRepository.Add(obj);
        }
        public void DeleteQuestion(Question obj)
        {
            questionRepository.Delete(obj);
        }
        public void UpdateQuestion(Question obj)
        {
            questionRepository.Update(obj);
        }
    }
}