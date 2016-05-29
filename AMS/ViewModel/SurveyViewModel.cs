using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.ViewModel
{
    public class SurveyViewModel
    {
        public string Title { get; set; }

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