using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Service;
using AMS.ViewModel;
using System.Globalization;

namespace AMS.Views.HelpdeskSupporter
{
    public class HelpdeskSupporterController : Controller
    {
        private HelpdeskSupporterService _helpdeskSupporterService = new HelpdeskSupporterService();
        private int currId;
        // GET: HelpdeskSupporter
        public ActionResult Index()
        {
            ViewBag.allRequest = _helpdeskSupporterService.ListAllRequest();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetHelpdeskRequest(int selectedId)
        {
            currId = selectedId;
            HelpdeskRequest hr = _helpdeskSupporterService.GetHelpdeskRequest(selectedId);
            HelpdeskRequestViewModel request = new HelpdeskRequestViewModel();
            request.Title = hr.Title;
            request.Description = hr.Description;
            //Convert date to dd/MM/yyyy
            DateTime parsedCreateDate = (DateTime) hr.CreateDate;
            request.CreateDate = parsedCreateDate.ToString("dd/MM/yyyy");
            DateTime parsedAssignDate = (DateTime)hr.AssignDate;
            request.AssignDate = parsedAssignDate.ToString("dd/MM/yyyy");

            //Convert status to string
            //int currStatus = hr.Status.Value;
            //if(currStatus == 1)
            //{
            //    request.Status = "assigned";
            //}
            request.Status = hr.Status.Value;
            request.Price = hr.Price.Value;
            request.HouseName = hr.House.HouseName;
            int test = hr.HelpdeskServiceId.Value;
            request.HelpdeskServiceName = hr.HelpdeskService.Name;
            request.Id = hr.Id;
            return Json(request);
        }

        [HttpPost]
        [ValidateInput(false)]
        public bool UpdateRequest(int id, int status)
        {
            bool result = _helpdeskSupporterService.UpdateStatus(id, status);
            return result;
        }
    }
}