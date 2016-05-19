using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class ManagementController : Controller
    {
        //
        // GET: /Management/
        public ActionResult AdminBoard()
        {
            return View();
        }
        public ActionResult ManageHousehold()
        {
            return View();
        }

        public ActionResult ManageRequest()
        {
            return View();
        }
        public ActionResult ManageIncome()
        {
            return View();
        }
        public ActionResult ManageReport()
        {
            return View();
        }
        public ActionResult ManageSurvey()
        {
            return View();
        }
        public ActionResult ManageReipt()
        {
            return View();
        }
        public ActionResult ViewReceipt()
        {
            return View();
        }
        public ActionResult CreateReceipt()
        {
            return View();
        }
	}
}