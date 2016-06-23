using AMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
namespace AMS.Controllers
{
    [Authorize]
    public class HouseController : Controller
    {
        UserService userService = new UserService();
        HouseServices houseService = new HouseServices();
        [Route("House/{houseId}")]
        public ActionResult Index(int? houseId)
        {
            House curHouse = null;
            if (houseId == null)
            {
              return  RedirectToAction("Index","Home");
            }
            else
            {
                curHouse = houseService.FindById(houseId.Value);
                if(curHouse == null)
                {
                    return RedirectToAction("Index","Home");
                }
            }
            User curUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            if (curUser == null)
            {
                return View("Error");
            }
            List<User> members = userService.findByHouseId(curHouse.Id);
      
            ViewBag.curUser = curUser;
            ViewBag.curHouse = curHouse;
            ViewBag.members = members;
            return View();
        }
    }
}