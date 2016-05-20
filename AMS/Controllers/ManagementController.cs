using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AMS.Service;
using Newtonsoft.Json;

namespace AMS.Controllers
{
    public class ManagementController : Controller
    {
        HelpdeskServicesService _helpdeskServicesService = new HelpdeskServicesService();
        //
        // GET: /Management/
        public ActionResult AdminBoard()
        {
            return View();
        }
        public ActionResult ManageHousehold()
        {
            return View();
        }

        public ActionResult ManageRequest()
        {


            String action = this.Request.QueryString["action"];
            if (null != action)
            {
                String url = "";
                if (action.Equals("h_desk_mng"))
                {
                    url = "TestPartial";
                }
                else if (action.Equals("addHelpdeskSrv"))
                {
                    NameValueCollection nvc = this.Request.Form;
                    String name = nvc["h_desk_srv_name"];
                    String typeId = nvc["h_desk_type"];
                    String price = nvc["h_desk_price"];
                    String status = nvc["h_desk_srv_status"];
                    String desc = nvc["h_desk_srv_desc"];
                    
                    //                return RedirectToAction("Index","HelpdeskService");
                    HelpdeskService hdService = new HelpdeskService();
                    hdService.Name = name;
                    hdService.HelpdeskServiceCategoryId = Int32.Parse(typeId);
                    hdService.Price=Double.Parse(price);
                    hdService.Status = Int32.Parse(status);
                    hdService.Description = desc;
                    _helpdeskServicesService.Add(hdService);
                }
                return PartialView(url);
            }
            else
            {
                return View();
            }
        }
        public ActionResult ManageIncome()
        {
            return View();
        }
        public ActionResult ManageReport()
        {
            return View();
        }
        public ActionResult ManageReipt()
        {
            return View();
        }
        public ActionResult ViewReceipt()
        {
            return View();
        }
        public ActionResult CreateReceipt()
        {
            return View();
        }
    }
}