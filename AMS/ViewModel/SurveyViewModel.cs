using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.ViewModel
{
    public class SurveyViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CloseDate { get; set; }
        public List<QuestionViewModel> QuestionViewModels { get; set; }
        public List<AnswerViewModel> AnswerViewModels { get; set; } 
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
}