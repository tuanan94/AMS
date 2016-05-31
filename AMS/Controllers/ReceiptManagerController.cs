using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Models;
using AMS.Service;

namespace AMS.Controllers
{
    public class ReceiptManagerController : Controller
    {
        //        // GET: ReceiptManager
        //        public ActionResult Index()
        //        {
        //            return View();
        //        }
        UserServices _userServices = new UserServices();
        ReceiptServices _receiptServices = new ReceiptServices();
        readonly string parternTime = "dd-MM-yyyy HH:mm";

        [HttpGet]
        [Route("Home/ManageReceipt/View/{userId}")]
        public ActionResult GetUserReceipt(int userId)
        {
            ViewBag.userId = userId;
            return View("~/Views/Home/ReceiptView.cshtml");
        }

        [HttpGet]
        [Route("Home/ManageReceipt/GetOrderList")]
        public ActionResult GetOrderList(int userId)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(userId);
            if (null != u)
            {
                List<Receipt> receipts = _receiptServices.FindByHouseId(u.HouseId.Value);
                List<ReceiptInfoModel> receiptModel = new List<ReceiptInfoModel>();
                if (null != receipts && receipts.Count != 0)
                {
                    ReceiptInfoModel model = null;
                    foreach (var r in receipts)
                    {
                        model = new ReceiptInfoModel();
                        model.ReceiptId = r.Id;
                        model.CreateDate = r.CreateDate.Value.ToString(parternTime);
                        model.Status = r.Status.Value;
                        model.HouseName = r.House.HouseName;
                        model.ReceiptTitle = r.Title;

                        List<ReceiptDetail> orderDetails = r.ReceiptDetails.ToList();
                        double total = 0;
                        foreach (var od in orderDetails)
                        {
                            total = od.UnitPrice.Value * od.Quantity.Value;
                        }
                        model.TotalOrder = total;
                        receiptModel.Add(model);
                    }
                    return Json(receiptModel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    response.StatusCode = -1;
                    response.Msg = "Không tìn thấy cư dân";
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìn thấy cư dân";
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Home/ManageReceipt/View/Detail")]
        public ActionResult GetOrderDetail(int userId, int orderId)
        {
            User u = _userServices.FindById(userId);
            ViewBag.userId = userId;
            if (null != u)
            {
                Receipt receipt = _receiptServices.FindById(orderId);
                if (null != receipt)
                {
                    ViewBag.receipt = receipt;
                    return View("~/Views/Home/ViewReceiptDetail.cshtml");
                }
                return View("~/Views/Home/ReceiptView.cshtml");
            }
            return View("~/Views/Home/ReceiptView.cshtml");
        }
    }
}