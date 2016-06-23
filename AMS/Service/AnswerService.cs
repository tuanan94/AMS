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
        public Answer FindByContent(string content, int questionId)
        {
            return answerRepository.List.FirstOrDefault(t => t.AnswerContent == content && t.QuestionId ==questionId);
        }

        //public int FindByContent(string content)
        //{
        //    return answerRepository.List.FirstOrDefault(t => t.AnswerContent == content).Id;
        //}
        public List<Answer> FindByQuestionId(int id)
        {
            return answerRepository.List.Where(t => t.QuestionId == id).ToList();
        }
        public void DeleteAnswerByQuestionId(int id)
        {
            List<Answer> obj = answerRepository.List.Where(t => t.QuestionId == id).ToList();
            foreach (var item in obj)
            {
                answerRepository.Delete(item);
            }

        }
        public void AddAnswer(Answer obj)
        {
            answerRepository.Add(obj);
        }
        public void DeleteAnswer(Answer obj)
        {

            answerRepository.Delete(obj);
        }
        public void UpdateAnswer(Answer obj)
        {

            answerRepository.Update(obj);
        }
    }
}