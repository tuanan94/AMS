using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Service;

namespace AMS.Controllers
{
    public class HelpdeskServiceController : Controller
    {
        HelpdeskServicesService mngService = new HelpdeskServicesService();
        // GET: HelpdeskService
        public ActionResult Index()
        {
            List<HelpdeskService> helpdeskServices = mngService.GetHelpdeskServices();
            ViewBag.helpdeskServices = helpdeskServices;
            return View("~/Views/Management/ManageRequest.cshtml");
        }


//        // GET: HelpdeskService/Edit/5
//        public ActionResult Edit(int id)
//        {
//            return View();
//        }
//
//        // POST: HelpdeskService/Edit/5
//        [HttpPost]
//        public ActionResult Edit(int id, FormCollection collection)
//        {
//            try
//            {
//                // TODO: Add update logic here
//
//                return RedirectToAction("Index");
//            }
//            catch
//            {
//                return View();
//            }
//        }
//
//        // GET: HelpdeskService/Delete/5
//        public ActionResult Delete(int id)
//        {
//            return View();
//        }

    }
}
