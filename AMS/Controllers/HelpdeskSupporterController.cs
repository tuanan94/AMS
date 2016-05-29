﻿using System;
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
        // GET: HelpdeskSupporter
        public ActionResult Index()
        {
            //IEnumerable<HelpdeskRequest> allRequest = _helpdeskSupporterService.ListAllRequest();
            //HelpdeskRequestViewModel viewModel = new HelpdeskRequestViewModel();
            //List<HelpdeskRequestViewModel> allViewModel = new List<HelpdeskRequestViewModel>();
            //foreach (var request in allRequest)
            //{
            //    if(request.Status == 1)
            //    {
            //        viewModel.Id = request.Id;
            //        viewModel.Title = request.Title;
            //        viewModel.Description = request.Description;
            //        DateTime parsedCreateDate = (DateTime)request.CreateDate;
            //        viewModel.CreateDate = parsedCreateDate.ToString("dd/MM/yyyy");
            //        DateTime parsedCloseDate = (DateTime)request.CloseDate;
            //        viewModel.CloseDate = parsedCloseDate.ToString("dd/MM/yyyy");
            //        viewModel.Status = (int) request.Status;
            //        viewModel.Price = (double) request.Price;
            //        viewModel.HouseName = request.House.HouseName;
            //        viewModel.HelpdeskServiceName = request.HelpdeskService.Name;
            //        allViewModel.Add(viewModel);
            //    }
            //}
            IEnumerable<HelpdeskRequest> allRequest = _helpdeskSupporterService.ListAllRequest();
            List<HelpdeskRequest> openRequest = new List<HelpdeskRequest>();
            foreach (var request in allRequest)
            {
                if (request.Status == 1)
                {
                    openRequest.Add(request);
                }
            }
            ViewBag.openRequest = openRequest;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetHelpdeskRequest(int selectedId)
        {
            HelpdeskRequest hr = _helpdeskSupporterService.GetHelpdeskRequest(selectedId);
            HelpdeskRequestViewModel request = new HelpdeskRequestViewModel();
            request.Title = hr.Title;
            request.Description = hr.Description;
            //Convert date to dd/MM/yyyy
            DateTime parsedCreateDate = (DateTime) hr.CreateDate;
            request.CreateDate = parsedCreateDate.ToString("dd/MM/yyyy");
            DateTime parsedAssignDate = (DateTime)hr.AssignDate;
            request.AssignDate = parsedAssignDate.ToString("dd/MM/yyyy");
            request.Status = hr.Status.Value;
            request.Price = hr.Price.Value;
            request.HouseName = hr.House.HouseName;
            request.HelpdeskServiceName = hr.HelpdeskService.Name;
            request.Id = hr.Id;
            return Json(request);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Detail(int id)
        {
            HelpdeskRequest hr = _helpdeskSupporterService.GetHelpdeskRequest(id);
            HelpdeskRequestViewModel request = new HelpdeskRequestViewModel();
            request.Title = hr.Title;
            request.Description = hr.Description;
            //Convert date to dd/MM/yyyy
            DateTime parsedCreateDate = (DateTime)hr.CreateDate;
            request.CreateDate = parsedCreateDate.ToString("dd/MM/yyyy");
            DateTime parsedCloseDate = (DateTime)hr.CloseDate;
            request.CloseDate = parsedCloseDate.ToString("dd/MM/yyyy");
            request.Status = hr.Status.Value;
            request.Price = hr.Price.Value;
            request.HouseName = hr.House.HouseName;
            request.HelpdeskServiceName = hr.HelpdeskService.Name;
            request.Id = hr.Id;
            ViewBag.currRequest = request;
            return View();
        }

        public ActionResult Detail()
        {
            return View();
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