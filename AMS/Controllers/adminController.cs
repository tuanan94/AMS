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
    }
}