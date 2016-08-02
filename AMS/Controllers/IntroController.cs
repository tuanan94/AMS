using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class IntroController : Controller
    {
        // GET: Intro
        public ActionResult Index()
        {
            return View();
        }
    }
}