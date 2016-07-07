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
        public int Count { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Question { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? AnswerDate { get; set; }
        //public string StartDate { get; set; }
        //public string EndDate { get; set; }
        //public string PublishDate { get; set; }

        public int RoleId { get; set; }
      
        public string AnswerContent { get; set; }
      

    }

    public class AnswerViewModel
    {
        public string AnswerContent { get; set; }
        public int QuestionId { get; set; }
    }
    public class QuestionViewModel
    {
        public string QuestionContent { get; set; }
        public int PollId { get; set; }
    }

    public class UserAnser
    {
        public int AnswerId { get; set; }
        public int CountAnswer { get; set; }
    }
}