using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Service;
using AMS.ViewModel;
using System.Globalization;
using Microsoft.AspNet.Identity;


namespace AMS.Views.HelpdeskSupporter
{
    public class HelpdeskSupporterController : Controller
    {
        private readonly HelpdeskSupporterService _helpdeskSupporterService = new HelpdeskSupporterService();
        private readonly UserService _userService = new UserService();
        private readonly HelpdeskRequestHelpdeskSupporterService _hrhs = new HelpdeskRequestHelpdeskSupporterService();
        private readonly HelpdeskRequestLogServices _helpdeskRequestLogService = new HelpdeskRequestLogServices();
        readonly string patternDate = "dd-MM-yyyy HH:mm";

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Index()
        {
            User currUser = _userService.findById(int.Parse(User.Identity.GetUserId()));
            List<HelpdeskRequestHelpdeskSupporter> allRequest = _hrhs.GetCurrentHelpdeskRequest(currUser.Id);
            List<HelpdeskRequest> hr = new List<HelpdeskRequest>();
            foreach (var request in allRequest)
            {
                hr.Add(request.HelpdeskRequest);
            }
            ViewBag.hr = hr;
            ViewBag.currHelpdeskSupporterId = currUser.Id;
            return View();
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Detail(int currHelpdeskSupporterId, int requestId)
        {
            HelpdeskRequest hr = _helpdeskSupporterService.GetHelpdeskRequest(requestId);
            HelpdeskRequestViewModel request = new HelpdeskRequestViewModel();

            request.Title = hr.Title;
            request.Description = hr.Description;
            //---Convert date to dd/MM/yyyy
            DateTime parsedCreateDate = (DateTime)hr.CreateDate;
            request.CreateDate = parsedCreateDate.ToString(patternDate);
            if (hr.DueDate != null)
            {
                DateTime parsedDueDate = (DateTime)hr.DueDate;
                request.DueDate = parsedDueDate.ToString(patternDate);
            }
            
            request.Status = hr.Status.Value;
            request.Price = hr.Price.Value;
            request.HouseName = hr.House.HouseName;
            request.HelpdeskServiceName = hr.HelpdeskService.Name;
            request.Id = hr.Id;
            request.HelpdeskSupporterId = currHelpdeskSupporterId;
            ViewBag.currRequest = request;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Detail()
        {
            int currRequestId = int.Parse(Request["hiddenId"]);
            int status = int.Parse(Request["hiddenStatus"]);
            int currHelpdeskSupporterId = int.Parse(Request["hiddenHelpdeskSupporterId"]);
            HelpdeskRequest currHelpdeskRequest = _helpdeskSupporterService.GetHelpdeskRequest(currRequestId);

            bool result = _helpdeskSupporterService.UpdateStatus(currRequestId, status);
            if (result)
            {
                //if update sucessfully, log this request.
                HelpdeskRequestLog helpdeskRequestLog = new HelpdeskRequestLog();
                helpdeskRequestLog.HelpdeskRequestId = currRequestId;
                helpdeskRequestLog.ChangeFromUserId = currHelpdeskSupporterId;
                helpdeskRequestLog.CreateDate = DateTime.Now;
                helpdeskRequestLog.StatusFrom = int.Parse(Request["hiddenStatusFrom"]);
                helpdeskRequestLog.StatusTo = status;
                _helpdeskRequestLogService.Add(helpdeskRequestLog);

                ViewBag.messageSuccess = "Đã xử lý thành công!";
                return Redirect("~/HelpdeskSupporter/Index?id=" + currHelpdeskSupporterId);
            }
            return Redirect("~/View/Shared/Error");
        }

        public ActionResult History()
        {
            User currUser = _userService.findById(int.Parse(User.Identity.GetUserId()));
            int currUserId = currUser.Id;
            List<HelpdeskRequestHelpdeskSupporter> allRequest = _hrhs.GetHelpdeskRequestById(currUserId);
            List<HelpdeskRequest> hr = new List<HelpdeskRequest>();
            foreach (var request in allRequest)
            {
                hr.Add(request.HelpdeskRequest);
            }
            List<HelpdeskRequestLog> logOfCurrRequest = new List<HelpdeskRequestLog>();
            List<List<HelpdeskRequestLog>> listLog = new List<List<HelpdeskRequestLog>>();
            foreach(var request in hr)
            {
                logOfCurrRequest = _helpdeskRequestLogService.GetHelpdeskRequestLogByUser(currUserId);
                listLog.Add(logOfCurrRequest);
            }
            ViewBag.listLog = listLog;
            return View();
        }
    }
}