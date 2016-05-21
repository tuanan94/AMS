using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Service;

namespace AMS.Views.HelpdeskSupporter
{
    public class HelpdeskSupporterController : Controller
    {
        private HelpdeskSupporterService _helpdeskSupporterService = new HelpdeskSupporterService();

        // GET: HelpdeskSupporter
        public ActionResult Index()
        {
            ViewBag.allRequest = _helpdeskSupporterService.ListAllRequest();
            return View();
        }
    }
}