using AMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.ViewModel;
namespace AMS.Controllers
{
    public class HomeController : Controller
    {
        TestService testService = new TestService();
        PendingMemberService pendingMemberService = new PendingMemberService();
        public ActionResult Test()
        {
            List<House> allHouse = testService.getAllHouse();
            ViewBag.allHouse = allHouse;
            return View();
         }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        //BinhHT
        [HttpGet]
        public ActionResult ViewProfile()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ManageMember()
        {
            return View();
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/Create")]
        public ActionResult CreateNewHdRequest()
        {
            return View("CreateHdRequest");
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/ViewHistory")]
        public ActionResult ViewHistoryHdRequest()
        {
            return View("ViewHistoryHdRequests");
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/AdminView")]
        public ActionResult ViewRequestingHdRequestToManager()
        {
            return View("ViewRequestingHdRequestsOfManager");
        }

        [HttpPost]
        public ActionResult ManageMember(MemberViewModel member)
        {
            //pendingMemberService.addMemberRequest(member);
            return View();
        }
    }
}