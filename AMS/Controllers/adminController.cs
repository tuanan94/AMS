﻿using AMS.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace AMS.Controllers
{
    public class AdminController : Controller
    {
        AdminService manageHouseInfo = new AdminService();// manageHouseInfo mangage all house info in database
        // GET: admin
        public ActionResult manageHouse()
        {
            List<House> allHouseInfo = manageHouseInfo.getAllHouseInfo();
            ViewBag.allHouseInfo = allHouseInfo;
            return View();
        }
        [HttpGet]
        public Object allHouseInfo()
        {
            return JsonConvert.SerializeObject(manageHouseInfo.getAllHouseInfo(),Formatting.Indented,new JsonSerializerSettings
            {PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult addHouse(String Block, String Floor, String HouseName,String Description,float Area)
        {
            bool isValid = true;
            isValid = !HouseName.Equals("");
            //Step 1: Valid
            if (isValid)
            {
                //Step 2: Send to service to do business
                manageHouseInfo.addHouse(Block, Floor, HouseName,Description,Area);
            }

            return RedirectToAction("ManageHouse");
        }
        [HttpPost]
        public void addHouseAjax(String Block, String Floor, String HouseName, String Description, float Area)
        {
            bool isValid = true;
            //Step 1: Valid
            isValid = !HouseName.Equals("");
            if (isValid)
            {
                //Step 2: Send to service to do business
                manageHouseInfo.addHouse(Block, Floor, HouseName, Description, Area);
            }
        }
        // delete house
        [HttpPost]
        public ActionResult deleteHouse(int Id)
        {
            manageHouseInfo.deleteHouse(Id);
            return RedirectToAction("ManageHouse");
            //return Json("Response from delete house");
        }
   }
}