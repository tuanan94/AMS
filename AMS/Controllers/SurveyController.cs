using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class SurveyController : Controller
    {
        //
        // GET: /Survey/
        public ActionResult Survey()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Surveys()
        {
            string[] listQuestion = Request.Form.GetValues("question");
            string[] listAnwser = Request.Form.GetValues("anwser");
            string[] listCountAnwser = Request.Form.GetValues("count");
          //  string[] listCountAnwser = {"1", "2", "3","1","2","1","2","3","4"};
            List<string> listCountAnwsers = new List<string>(listCountAnwser);
            // count number anwser of question
           List<int> listCount = new List<int>();

           for (int i = 0; i < (listCountAnwsers.Count)-1; i++)
            {
                if (Int32.Parse(listCountAnwsers[i]) > Int32.Parse(listCountAnwsers[i+1]))
                {
                    listCount.Add(Int32.Parse(listCountAnwsers[i]));
                }
            }
     
           listCount.Add(Int32.Parse((listCountAnwsers[(listCountAnwsers.Count) - 1])));
           int a = listCount.Count;

            return View("Survey");
        }
	}
}