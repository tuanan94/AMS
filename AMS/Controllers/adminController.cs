using AMS.Service;
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
        public ActionResult addHouse(int Id,String Block, String Floor, String HouseName,String Description,double Area)
        {
            bool isValid = true;
            isValid = !HouseName.Equals("");
            //Step 1: Valid
            if (isValid)
            {
                //Step 2: Send to service to do business
                manageHouseInfo.addHouse(Id,Block, Floor, HouseName,Description,Area);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public void addHouseAjax(int Id, String Block, String Floor, String HouseName, String Description, double Area)
        {
            bool isValid = true;
            //Step 1: Valid
            isValid = !HouseName.Equals("");
            if (isValid)
            {
                //Step 2: Send to service to do business
                manageHouseInfo.addHouse(Id, Block, Floor, HouseName, Description, Area);
            }
        }
    }
}