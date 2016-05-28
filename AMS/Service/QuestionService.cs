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
        public List<Question> FindBySurveyId(int id)
        {
            return questionRepository.List.Where(t=>t.SurveyId==id).ToList();
        }

        public void AddQuestion(Question obj)
        {
            questionRepository.Add(obj);
        }
    }
}