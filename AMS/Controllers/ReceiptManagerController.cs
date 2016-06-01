using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Constant;
using AMS.Models;
using AMS.Service;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;

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
        HouseServices _houseServices = new HouseServices();
        ReceiptDetailServices _receiptDetailServices = new ReceiptDetailServices();
        ServiceChargeSevices _serviceChargeSevices = new ServiceChargeSevices();
        readonly string parternTime = "dd-MM-yyyy HH:mm";

        [HttpGet]
        [Route("Home/ManageReceipt/View/{userId}")]
        public ActionResult GetUserReceipt(int userId)
        {
            ViewBag.userId = userId;
            return View("~/Views/Home/ViewReceipt.cshtml");
        }

        [HttpGet]
        [Route("Home/ManageReceipt/GetOrderList")]
        public ActionResult GetOrderList(int userId)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(userId);
            if (null != u)
            {
                List<Receipt> receipts = _receiptServices.GetReceiptByHouseId(u.HouseId.Value);
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
                return View("~/Views/Home/ViewReceipt.cshtml");
            }
            return View("~/Views/Home/ViewReceipt.cshtml");
        }

        [HttpGet]
        [Route("Management/ManageReceipt/View/{userId}")]
        public ActionResult ViewManagerOrderList(int userId)
        {
            ViewBag.userId = userId;
            return View("~/Views/Management/ViewManagerReceipt.cshtml");
        }


        [HttpGet]
        [Route("Management/ManageReceipt/Admin/GetManagerOrderList")]
        public ActionResult GetManagerOrderList(int userId)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(userId);
            if (null != u)
            {
                List<Receipt> receipts = _receiptServices.GetAllReceipts();
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
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy người quản lý";
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy người quản lý";
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ManageReceipt/Edit/Detail")]
        public ActionResult ViewUpdateOrderDetail(int userId, int orderId)
        {
            User u = _userServices.FindById(userId);
            ViewBag.userId = userId;
            if (null != u)
            {
                Receipt receipt = _receiptServices.FindById(orderId);
                if (null != receipt)
                {

                    List<House> block = _houseServices.GetAllBlock();
                    if (block != null && block.Count != 0)
                    {
                        List<House> floor = _houseServices.GetFloorInBlock(receipt.House.Block);
                        List<House> rooms = _houseServices.GetRoomsInFloor(receipt.House.Block, receipt.House.Floor);
                        ViewBag.block = block;
                        ViewBag.firstBlockFloor = floor;
                        ViewBag.rooms = rooms;
                    }

                    ViewBag.receipt = receipt;
                    return View("~/Views/Management/UpdateReceipt.cshtml");
                }
                return View("~/Views/Home/ViewReceipt.cshtml");
            }
            return View("~/Views/Home/ViewReceipt.cshtml");
        }

        [HttpGet]
        [Route("Management/ManageReceipt/{userId}")]
        public ActionResult ManageReceipt(int userId)
        {
            List<House> block = _houseServices.GetAllBlock();
            if (block != null && block.Count != 0)
            {
                List<House> floor = _houseServices.GetFloorInBlock(block[0].Block);
                List<House> rooms = _houseServices.GetRoomsInFloor(block[0].Block, floor[0].Floor);
                ViewBag.block = block;
                ViewBag.firstBlockFloor = floor;
                ViewBag.rooms = rooms;
            }
            ViewBag.userId = userId;
            return View("~/Views/Management/CreateManualReceipt.cshtml");
        }

        [HttpPost]
        [Route("Management/ManageReceipt/AddNewReceipt")]
        public ActionResult AddNewReceipt(ReceiptModel receipt)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(receipt.Creator);
            if (null != u)
            {
                House house = _houseServices.FindByHouseName(receipt.ReceiptHouseName);
                if (null != house)
                {
                    try
                    {
                        Receipt eReceipt = new Receipt();
                        eReceipt.HouseId = house.Id;
                        eReceipt.CreateDate = DateTime.Now;
                        eReceipt.LastModified = DateTime.Now;
                        eReceipt.ManagerId = u.Id;
                        eReceipt.Description = receipt.ReceiptDesc;
                        eReceipt.Title = receipt.ReceiptTitle;
                        eReceipt.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED;
                        _receiptServices.Add(eReceipt);

                        ServiceFee item = null;
                        ReceiptDetail detail = null;
                        foreach (var i in receipt.ListItem)
                        {
                            item = new ServiceFee();
                            item.Name = i.Name;
                            item.Price = i.UnitPrice;
                            item.CreateDate = DateTime.Now;
                            _serviceChargeSevices.Add(item);

                            detail = new ReceiptDetail();
                            detail.Quantity = i.Quantity;
                            detail.ReceiptId = eReceipt.Id;
                            detail.ServiceFeeId = item.Id;
                            detail.UnitPrice = item.Price;
                            _receiptDetailServices.Add(detail);
                        }
                    }
                    catch (Exception)
                    {
                        response.Msg = "Tạo hóa đơn thất bại";
                        response.StatusCode = -1;
                        return Json(response);
                    }
                }
            }
            else
            {
                response.Msg = "Không tìm thấy người quản lý";
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/ManageReceipt/UpdateReceipt")]
        public ActionResult UpdateReceipt(ReceiptModel receiptModel)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(receiptModel.Creator);

            if (null != u)
            {
                Receipt receipt = _receiptServices.FindById(receiptModel.ReceiptId);
                if (null != receipt)
                {
                    try
                    {
                        
                        ServiceFee item = null;
                        ReceiptDetail detail = null;
                        bool receiptIsDeleted = false;
                        if (null == receiptModel.ListItem)
                        {
                            List<ReceiptDetail> receiptDetails = receipt.ReceiptDetails.ToList();
                            
                            int receiptId = receipt.Id;
                            _receiptServices.DeleteById(receiptId);
                            receiptIsDeleted = true;
                            foreach (var recDetail in receiptDetails)
                            {
                                _receiptDetailServices.DeleteById(recDetail.Id);
                                _serviceChargeSevices.DeleteById(recDetail.ServiceFeeId.Value);
                            }
                        }
                        else
                        {
                            List<ReceiptDetail> notFoundOrderId = new List<ReceiptDetail>();

                            bool hasOrderDetail = false;
                            var index = 0;
                            foreach (var recDetail in receipt.ReceiptDetails)
                            {
                                hasOrderDetail = false;
                                foreach (var newRecDetailModel in receiptModel.ListItem.ToList())
                                {
                                    if (newRecDetailModel.RecDetailId == 0)
                                    {
                                        ServiceFee fee = new ServiceFee();
                                        fee.Name = newRecDetailModel.Name;
                                        fee.Price = newRecDetailModel.UnitPrice;
                                        fee.CreateDate = DateTime.Now;
                                        _serviceChargeSevices.Add(fee);

                                        ReceiptDetail newRecDetail = new ReceiptDetail();
                                        newRecDetail.Quantity = newRecDetailModel.Quantity;
                                        newRecDetail.ServiceFeeId = fee.Id;
                                        newRecDetail.UnitPrice = newRecDetailModel.UnitPrice;
                                        newRecDetail.ReceiptId = receipt.Id;
                                        _receiptDetailServices.Add(newRecDetail);

                                        receiptModel.ListItem.RemoveAt(index);
                                        index--;
                                    }
                                    else if (recDetail.Id == newRecDetailModel.RecDetailId)
                                    {
                                        ReceiptDetail updateReceiptDetail =
                                            _receiptDetailServices.FindById(newRecDetailModel.RecDetailId);
                                        updateReceiptDetail.Quantity = newRecDetailModel.Quantity;
                                        updateReceiptDetail.UnitPrice = newRecDetailModel.UnitPrice;

                                        ServiceFee updateServiceFee =
                                            _serviceChargeSevices.FindById(recDetail.ServiceFeeId.Value);
                                        updateServiceFee.Name = newRecDetailModel.Name;
                                        updateServiceFee.Price = newRecDetailModel.UnitPrice;

                                        _serviceChargeSevices.Update(updateServiceFee);
                                        _receiptDetailServices.Update(updateReceiptDetail);
                                        hasOrderDetail = true;
                                    }
                                    index++;
                                }
                                if (!hasOrderDetail)
                                {
                                    notFoundOrderId.Add(recDetail);
                                }
                                index = 0;
                            }
                            if (notFoundOrderId.Count != 0)
                            {
                                foreach (var deletedRecDetailId in notFoundOrderId)
                                {
                                    _receiptDetailServices.DeleteById(deletedRecDetailId.Id);
                                    _serviceChargeSevices.DeleteById(deletedRecDetailId.ServiceFee.Id);
                                }
                            }
                        }
                        if (!receiptIsDeleted)
                        {
                            House updateHouse = _houseServices.FindByHouseName(receiptModel.ReceiptHouseName);
                            if (receipt.HouseId != updateHouse.Id)
                            {
                                receipt.HouseId = updateHouse.Id;
                            }
                            receipt.Title = receiptModel.ReceiptTitle;
                            receipt.Description = receiptModel.ReceiptDesc;
                            receipt.LastModified = DateTime.Now;
                            _receiptServices.Update(receipt);
                        }
                        else
                        {
                            response.StatusCode = 5;
                            response.Msg = "Đã xóa thành công hóa đơn.";
                        }
                    }
                    catch (Exception e)
                    {
                        response.Msg = "Cập nhật hóa đơn thất bại";
                        response.StatusCode = -1;
                        return Json(response);
                    }
                }
            }
            else
            {
                response.Msg = "Không tìm thấy người quản lý";
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpGet]
        [Route("Management/ManageReceipt/ReceiptDetailItemList")]
        public ActionResult GetReceiptDetailItemList(int receiptId)
        {
            Receipt receipt = _receiptServices.FindById(receiptId);
            ViewBag.receipt = receipt;
            return PartialView("~/Views/Management/_receiptDetailItemList.cshtml");
        }

        [HttpGet]
        [Route("Management/ManageReceipt/UpdateStatus")]
        public ActionResult UpdateStatusReceipt(int ReceiptId, int UserId, int Status)
        {
            MessageViewModels response = new MessageViewModels();
            Receipt receipt = _receiptServices.FindById(ReceiptId);
            if (null != receipt)
            {
                User u = _userServices.FindById(UserId);
                if (u != null)
                {
                    Object data = null;
                    receipt.LastModified = DateTime.Now;
                    if (Status == SLIM_CONFIG.RECEIPT_STATUS_UNPAID)
                    {
                        receipt.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID;
                        data = new
                        {
                            status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID
                        };
                    }
                    else if (Status == SLIM_CONFIG.RECEIPT_STATUS_PAID)
                    {
                        receipt.Status = SLIM_CONFIG.RECEIPT_STATUS_PAID;
                        receipt.PaymentDate = DateTime.Now;
                        data = new
                        {
                            paymentDate = receipt.PaymentDate.Value.ToString(AmsConstants.DateFormat),
                            status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID
                        };
                    }
                    _receiptServices.Update(receipt);
                    response.Data = data;
                }
                else
                {
                    response.StatusCode = -1;
                    response.Msg = "Không tìm thấy người quản lý !";
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy hóa đơn !";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ManageReceipt/GetRoomAndFloor")]
        public ActionResult GetRoomAndFloor(string blockName, string floorName)
        {
            MessageViewModels response = new MessageViewModels();
            List<House> floor = _houseServices.GetFloorInBlock(blockName);
            List<string> floorStr = new List<string>();
            List<string> roomStr = new List<string>();

            if (floor != null && floor.Count > 0)
            {
                foreach (var f in floor)
                {
                    floorStr.Add(f.Floor);
                }

                List<House> rooms = null;
                if (floorName.IsNullOrWhiteSpace())
                {
                    rooms = _houseServices.GetRoomsInFloor(blockName, floor[0].Floor);
                }
                else
                {
                    rooms = _houseServices.GetRoomsInFloor(blockName, floorName);
                }

                if (rooms != null && rooms.Count > 0)
                {
                    foreach (var room in rooms)
                    {
                        roomStr.Add(room.HouseName);
                    }
                }
                response.Data = new { Floor = floorStr, Room = roomStr };
            }
            else
            {
                response.Data = new { Floor = floorStr, Room = roomStr };
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}