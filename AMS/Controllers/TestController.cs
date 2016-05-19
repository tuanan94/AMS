using AMS.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class TestController : Controller
    {
        TestService testService = new TestService();
        // GET: Test
        public ActionResult Index()
        {
            List<House> allHouse = testService.getAllHouse();
            ViewBag.allHouse = allHouse;
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult addHome(String Block, String Floor, String HouseName)
        {
            bool isValid = true;
            isValid = !HouseName.Equals("");
            //Step 1: Valid
            if (isValid)
            {
                //Step 2: Send to service to do business
                testService.addHouse(Block, Floor, HouseName);
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public Object allHouse()
        {
            return JsonConvert.SerializeObject(testService.getAllHouse());

        }
        [HttpPost]
        public void addHouseAjax(String Block, String Floor, String HouseName)
        {
            bool isValid = true;
            //Step 1: Valid
            isValid = !HouseName.Equals("");
            if (isValid)
            {
                //Step 2: Send to service to do business
                testService.addHouse(Block, Floor, HouseName);
            }
        }

    }
}