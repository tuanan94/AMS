using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.ViewModel
{
    public class SurveyViewModel
    {
        public SurveyViewModel()
        {
            //StartDate = DateTime.Now;
            //EndDate = DateTime.Now;
            //PublishDate = DateTime.Now;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PublishDate { get; set; }
        //public string StartDate { get; set; }
        //public string EndDate { get; set; }
        //public string PublishDate { get; set; }
        public int RoleId { get; set; }
        public List<QuestionViewModel> QuestionViewModels { get; set; }
        public List<AnswerViewModel> AnswerViewModels { get; set; }
        public string AnswerContent { get; set; }
        public int AnswerId { get; set; }

    }

    public class AnswerViewModel
    {
        public string AnswerContent { get; set; }
        public int QuestionId { get; set; }
    }
    public class QuestionViewModel
    {
        public string QuestionContent { get; set; }
        public int SurveyId { get; set; }
    }

    public class UserAnser
    {
        public int AnswerId { get; set; }
        public int CountAnswer { get; set; }
    }
}