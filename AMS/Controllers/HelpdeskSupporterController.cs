using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Service;
using AMS.ViewModel;
using System.Globalization;
using AMS.Constant;
using AMS.Enum;
using AMS.Filter;
using AMS.Models;
using Microsoft.AspNet.Identity;

namespace AMS.Views.HelpdeskSupporter
{
    public class HelpdeskSupporterController : Controller
    {
        private readonly HelpdeskSupporterService _helpdeskSupporterService = new HelpdeskSupporterService();
        private readonly UserService _userService = new UserService();
        private readonly HelpdeskRequestHelpdeskSupporterService _hrhs = new HelpdeskRequestHelpdeskSupporterService();
        private readonly HelpdeskRequestLogServices _helpdeskRequestLogService = new HelpdeskRequestLogServices();
        private readonly HelpdeskRequestServices _helpdeskRequestServices = new HelpdeskRequestServices();
        readonly string patternDate = "dd-MM-yyyy HH:mm";

        [HttpGet]
        [SupporterAuthorize]
        public ActionResult Index()
        {
            User currUser = _userService.findById(int.Parse(User.Identity.GetUserId()));//AnLTNM
            List<HelpdeskRequest> hr = _helpdeskRequestServices.GetAllHdRequestBySupporterId(currUser.Id);//AnLTNM
            ViewBag.hr = hr;
            ViewBag.currHelpdeskSupporterId = currUser.Id;
            return View();
        }

        [HttpGet]
        [Route("HelpdeskSupporter/GetListRequest")]
        public ActionResult GetHeRequestBySupporterId(int supporterId)
        {
            List<HelpdeskRequestViewModel> listRequest = new List<HelpdeskRequestViewModel>();
            HelpdeskRequestViewModel requestModel = null;
            try
            {
                User currUser = _userService.findById(supporterId);//AnLTNM
                if (null != currUser)
                {
                    List<HelpdeskRequest> hr = _helpdeskRequestServices.GetAllHdRequestBySupporterId(currUser.Id);//AnLTNM
                    foreach (var rq in hr)
                    {
                        requestModel = new HelpdeskRequestViewModel();
                        requestModel.Id = rq.Id;
                        requestModel.Title = rq.Title;
                        requestModel.HelpdeskServiceCatName = rq.HelpdeskServiceCategory.Name;
                        requestModel.CreateDate = rq.CreateDate.Value.ToString(AmsConstants.DateTimeFormat);
                        requestModel.CreateDateLong = rq.CreateDate.Value.Ticks;

                        if (null != rq.DueDate)
                        {
                            requestModel.DueDate = rq.DueDate.Value.ToString(AmsConstants.DateTimeFormat);
                            requestModel.DueDateLOng = rq.DueDate.Value.Ticks;
                        }
                        requestModel.Status = rq.Status.Value;
                        requestModel.HouseName = rq.House.HouseName;
                        listRequest.Add(requestModel);
                    }
                }
            }
            catch (Exception)
            {
                return Json(listRequest, JsonRequestBehavior.AllowGet);
            }
            return Json(listRequest, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [SupporterAuthorize]
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
            request.HouseName = hr.House.HouseName;
            request.HelpdeskServiceCatName = hr.HelpdeskServiceCategory.Name;
            request.Id = hr.Id;
            request.HelpdeskSupporterId = currHelpdeskSupporterId;
            ViewBag.currRequest = request;
            return View();
        }

        [HttpPost]
        [SupporterAuthorize]
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

                if (status == 3)
                {
                    //Add notification - AnTT - 09/07/2016 - START
                    NotificationService notificationService = new NotificationService();
                    if (currHelpdeskRequest.House != null)
                    {
                        List<User> memberInHouse = currHelpdeskRequest.House.Users.ToList();
                        foreach (User user in memberInHouse)
                        {
                            notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_HELPDESK_REQUEST, user.Id, SLIM_CONFIG.NOTIC_VERB_FINISH_REQUEST, currHelpdeskSupporterId, helpdeskRequestLog.Id);
                        }
                    }
                    //Add notification - AnTT - 09/07/2016 - END
                }
                ViewBag.messageSuccess = "Đã xử lý thành công!";
                return Redirect("~/HelpdeskSupporter/Detail?currHelpdeskSupporterId=" + currHelpdeskSupporterId + "&requestId=" + currRequestId);
            }
            return Redirect("~/View/Shared/Error");
        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult History()
        {
            User currUser = _userService.findById(int.Parse(User.Identity.GetUserId()));
            int currUserId = currUser.Id;
            List<HelpdeskRequestLog> logOfCurrRequest = _helpdeskRequestLogService.GetHelpdeskRequestLogByUser(currUserId);
            ViewBag.listLog = logOfCurrRequest;
            return View();
        }

        [HttpGet]
        [Route("HelpdeskSupporter/GetHistoryRequest")]
        public ActionResult GetHistoryRequest(int supporterId)
        {
            List<HelpdeskRequestLogViewModel> listLog = new List<HelpdeskRequestLogViewModel>();
            HelpdeskRequestViewModel requestModel = null;
            try
            {
                User currUser = _userService.findById(supporterId);//AnLTNM
                if (null != currUser)
                {
                    List<HelpdeskRequestLog> logOfCurrRequest = _helpdeskRequestLogService.GetHelpdeskRequestLogByUser(currUser.Id);
                    HelpdeskRequestLogViewModel log = null;
                    foreach (var rq in logOfCurrRequest)
                    {
                        log = new HelpdeskRequestLogViewModel();
                        log.Title = rq.HelpdeskRequest.Title;
                        log.SrvCatName = rq.HelpdeskRequest.HelpdeskServiceCategory.Name;
                        log.CreateDate = rq.CreateDate.Value.ToString(AmsConstants.DateTimeFormat);
                        log.CreateDateLong = rq.CreateDate.Value.Ticks;
                        log.StatusTo = rq.StatusTo.Value;
                        log.StatusFrom = rq.StatusFrom.Value;
                        log.HouseName = rq.HelpdeskRequest.House.HouseName;
                        listLog.Add(log);
                    }
                }
            }
            catch (Exception)
            {
                return Json(listLog, JsonRequestBehavior.AllowGet);
            }
            return Json(listLog, JsonRequestBehavior.AllowGet);
        }
    }
}