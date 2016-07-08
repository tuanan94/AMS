using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public ActionResult ErrorNoHouse()
        {
            return View("error_no_house");
        }
    }
}