using AMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class ProfileController : Controller
    {
        UserService userService = new UserService();
        [Route("Profile/{userId}")]
        public ActionResult Index(int userId)
        {
            User user = userService.findById(userId);
            ViewBag.user = user;
            return View();
        }
    }
}