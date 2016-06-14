using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Schema;
using AMS.Constant;
using AMS.Models;
using AMS.Service;
using Antlr.Runtime.Misc;
using LINQtoCSV;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.VisualBasic.FileIO;


namespace AMS.Controllers
{
    public class ReceiptController : Controller
    {
        //        // GET: ReceiptManager
        //        public ActionResult Index()
        //        {
        //            return View();
        //        }
        private UserServices _userServices = new UserServices();
        private ReceiptServices _receiptServices = new ReceiptServices();
        private HouseServices _houseServices = new HouseServices();
        private ReceiptDetailServices _receiptDetailServices = new ReceiptDetailServices();
        private ServiceChargeSevices _serviceChargeSevices = new ServiceChargeSevices();
        private BalanceSheetService _balanceSheetService = new BalanceSheetService();
        private UtilityCategoryServices _utilityCategoryServices = new UtilityCategoryServices();
        private UtilityServiceServices _utilityServiceServices = new UtilityServiceServices();
        private UtilityServiceRangePriceServices _rangePriceServices = new UtilityServiceRangePriceServices();
        private readonly string parternTime = "dd-MM-yyyy HH:mm";

        [HttpGet]
        [Route("Home/ManageReceipt/View/{userId}")]
        public ActionResult GetUserReceipt(int userId)
        {
            ViewBag.userId = userId;
            return View("ViewReceipt");
        }

        [HttpGet]
        [Route("Home/ManageReceipt/View/HouseExpenseStatus")]
        public ActionResult ViewHouseExpenseStatus(string month)
        {


            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (null != u)
            {
                DateTime thisMonth = DateTime.Now.Date;

                if (!month.IsNullOrWhiteSpace())
                {
                    thisMonth = DateTime.ParseExact(month, "MM-yyyy", CultureInfo.CurrentCulture);
                }


                List<Receipt> receipts = _receiptServices.GetMonthlyReceiptByHouseId(u.HouseId.Value, thisMonth);
                List<Receipt> unpaidReceipts =
                    receipts.Where(r => r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPAID).ToList();
                double unpaid = 0;
                double paid = 0;
                foreach (var r in unpaidReceipts)
                {
                    foreach (var detail in r.ReceiptDetails)
                    {
                        unpaid += detail.Quantity.Value * detail.UnitPrice.Value;
                    }
                }
                List<Receipt> paidReceipts =
                    receipts.Where(r => r.Status == SLIM_CONFIG.RECEIPT_STATUS_PAID).ToList();
                foreach (var r in paidReceipts)
                {
                    foreach (var detail in r.ReceiptDetails)
                    {
                        paid += detail.Quantity.Value * detail.UnitPrice.Value;
                    }
                }
                ViewBag.unpaidTotal = unpaid;
                ViewBag.paidTotal = paid;
                ViewBag.total = paid + unpaid;
                ViewBag.unpaidReceipts = unpaidReceipts;
                ViewBag.paidReceipts = paidReceipts;
                ViewBag.thisMonth = thisMonth.ToString("MM-yyyy");
            }

            return View("HouseExpenseStatus");
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
                        if (r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
                        {
                            r.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID;
                            r.LastModified = DateTime.Now;
                            _receiptServices.Update(r);
                        }
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
                            total += od.UnitPrice.Value * od.Quantity.Value;
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
                    return View("ViewReceiptDetail");
                }
                return View("ViewReceipt");
            }
            return View("ViewReceipt");
        }

        [HttpGet]
        [Route("Management/ManageReceipt/View")]
        public ActionResult ViewManagerOrderList()
        {
            ViewBag.userId = User.Identity.GetUserId();
            return View("ViewManagerReceipt");
        }


        [HttpGet]
        [Route("Management/ManageReceipt/Admin/GetManagerOrderList")]
        public ActionResult GetManagerOrderList(int userId)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(userId);
            if (null != u)
            {
                List<Receipt> receiptsGroupByCreateDate = _receiptServices.GetAllReceiptsGroupByCreateDate();

                List<ReceiptInfoModel> receiptModel = new List<ReceiptInfoModel>();
                if (null != receiptsGroupByCreateDate && receiptsGroupByCreateDate.Count != 0)
                {
                    ReceiptInfoModel model = null;
                    foreach (var r in receiptsGroupByCreateDate)
                    {
                        if (r.PublishDate != null && r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED &&
                            DateTime.Today.Date >= r.PublishDate)
                        {
                            if (r.IsAutomation == SLIM_CONFIG.RECEIPT_TYPE_MANUAL)
                            {
                                r.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID;
                                r.LastModified = DateTime.Now;
                                _receiptServices.Update(r);
                            }
                            else if (r.IsAutomation == SLIM_CONFIG.RECEIPT_TYPE_AUTOMATION)
                            {
                                List<Receipt> groupedByCreateDateReceipts =
                                    _receiptServices.GetReceiptsByCreateDate(r.CreateDate.Value);
                                foreach (var receipt in groupedByCreateDateReceipts)
                                {
                                    receipt.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID;
                                    receipt.LastModified = DateTime.Now;
                                    _receiptServices.Update(r);

                                }
                            }
                        }


                        model = new ReceiptInfoModel();
                        model.ReceiptId = r.Id;
                        model.CreateDate = r.CreateDate.Value.ToString(parternTime);
                        model.Status = r.Status.Value;
                        model.Block = r.House.Block;
                        model.Floor = r.House.Floor;
                        model.HouseName = r.House.HouseName;
                        model.ReceiptTitle = r.Title;
                        model.IsAutomation = r.IsAutomation.Value;
                        List<ReceiptDetail> orderDetails = r.ReceiptDetails.ToList();
                        double total = 0;
                        foreach (var od in orderDetails)
                        {
                            total += od.UnitPrice.Value * od.Quantity.Value;
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
                    return View("UpdateReceipt");
                }
                return View("ViewReceipt");
            }
            return View("ViewReceipt");
        }

        [HttpGet]
        [Route("Management/ManageReceipt/ViewBatchReceipt")]
        public ActionResult ViewBatchReceipt(int receiptId)
        {
            Receipt receipt = _receiptServices.FindById(receiptId);
            if (null != receipt)
            {
                int status = SLIM_CONFIG.RECEIPT_STATUS_PAID;
                List<Receipt> listReceiptGroupByCreateDate =
                    _receiptServices.GetReceiptsByCreateDate(receipt.CreateDate.Value);
                foreach (var re in listReceiptGroupByCreateDate)
                {
                    if (re.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPAID ||
                        re.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
                    {
                        status = re.Status.Value;
                        break;
                    }
                }
                ViewBag.groupStatus = status;
                ViewBag.automationReceipt = receipt;

                var utilService =
                    receipt.ReceiptDetails.Where(r => r.UtilityService.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_ELECTRICITY).ToList();
                if (utilService.Count() != 0)
                {
                    ViewBag.electricSevices = utilService.First().UtilityService.Name;
                }
                else
                {
                    ViewBag.electricSevices = "Chưa có giá";
                }
                utilService = receipt.ReceiptDetails.Where(r => r.UtilityService.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).ToList();
                if (utilService.Count() != 0)
                {
                    ViewBag.waterSevices = utilService.First().UtilityService.Name;
                }
                else
                {
                    ViewBag.waterSevices = "Chưa có giá";
                }
                utilService = receipt.ReceiptDetails.Where(r => r.UtilityService.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_HOUSE_RENT).ToList();
                if (utilService.Count() != 0)
                {
                    ViewBag.houseRentSevices = utilService.First().UtilityService.Name;
                }
                else
                {
                    ViewBag.houseRentSevices = "Chưa có giá";
                }
                utilService = receipt.ReceiptDetails.Where(r => r.UtilityService.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST).ToList();
                if (utilService.Count() != 0)
                {
                    ViewBag.fixedCostSevices = utilService.First().UtilityService.Name;
                }
                else
                {
                    ViewBag.fixedCostSevices = "Chưa có giá";
                }

                return View("UpdateAutomationReceipt");
            }
            return View("ViewReceipt");
        }

        [HttpGet]
        [Route("Management/ManageReceipt/ViewBatchReceiptDetail")]
        public ActionResult ViewBatchReceiptDetail(int receiptId)
        {
            MessageViewModels response = new MessageViewModels();
            Receipt receipt = _receiptServices.FindById(receiptId);
            if (null != receipt)
            {
                UtilityServicesGroupByTypeModel services = GetAllAparmentUtilitySevices();
                List<UtilityServiceIdNameModel> electrics = new List<UtilityServiceIdNameModel>();
                List<UtilityServiceIdNameModel> water = new List<UtilityServiceIdNameModel>();
                List<UtilityServiceIdNameModel> houseRent = new List<UtilityServiceIdNameModel>();
                List<UtilityServiceIdNameModel> fixedCost = new List<UtilityServiceIdNameModel>();
                UtilityServiceIdNameModel item = null;
                foreach (var s in services.ElectricSevices)
                {
                    item = new UtilityServiceIdNameModel();
                    item.Id = s.Id;
                    item.Name = s.Name;
                    electrics.Add(item);
                }
                foreach (var s in services.WaterSevices)
                {
                    item = new UtilityServiceIdNameModel();
                    item.Id = s.Id;
                    item.Name = s.Name;
                    water.Add(item);
                }
                foreach (var s in services.HouseRentSevices)
                {
                    item = new UtilityServiceIdNameModel();
                    item.Id = s.Id;
                    item.Name = s.Name;
                    houseRent.Add(item);
                }
                foreach (var s in services.FixedCostSevices)
                {
                    item = new UtilityServiceIdNameModel();
                    item.Id = s.Id;
                    item.Name = s.Name;
                    fixedCost.Add(item);
                }
                AutomationReceiptsTemplateModel receiptInfo = new AutomationReceiptsTemplateModel();
                receiptInfo.PublishDate = receipt.PublishDate.Value.ToString(AmsConstants.DateFormat);
                receiptInfo.Description = receipt.Description;
                receiptInfo.Title = receipt.Title;
                receiptInfo.Status = receipt.Status.Value;
                receiptInfo.ReceiptId = receipt.Id;
                var utilService =
                    receipt.ReceiptDetails.Where(r => r.UtilityService.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_ELECTRICITY).ToList();
                if (utilService.Count() != 0)
                {
                    receiptInfo.ElectricUtilServiceId = utilService.First().UtilityService.Id;
                }
                utilService = receipt.ReceiptDetails.Where(r => r.UtilityService.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).ToList();
                if (utilService.Count() != 0)
                {
                    receiptInfo.WaterUtilServiceId = utilService.First().UtilityService.Id;
                }
                utilService = receipt.ReceiptDetails.Where(r => r.UtilityService.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_HOUSE_RENT).ToList();
                if (utilService.Count() != 0)
                {
                    receiptInfo.HouseRentUtilServiceId = utilService.First().UtilityService.Id;
                }
                utilService = receipt.ReceiptDetails.Where(r => r.UtilityService.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST).ToList();
                if (utilService.Count() != 0)
                {
                    receiptInfo.FixCostUtilServiceId = utilService.First().UtilityService.Id;
                }
                Object obj = new
                {
                    ReceiptInfo = receiptInfo,
                    Electrics = electrics,
                    Water = water,
                    HouseRent = houseRent,
                    FixedCost = fixedCost,
                };
                response.Data = obj;
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy hóa đơn";
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ManageReceipt/GetBatchReceiptList")]
        public ActionResult GetBatchReceiptList(int receiptId)
        {
            MessageViewModels response = new MessageViewModels();
            Receipt receipt = _receiptServices.FindById(receiptId);
            if (receipt != null)
            {
                List<Receipt> receiptsGroupByCreateDate =
                    _receiptServices.GetReceiptsByCreateDate(receipt.CreateDate.Value);
                List<MonthlyReceiptModel> receiptModel = new List<MonthlyReceiptModel>();
                if (null != receiptsGroupByCreateDate && receiptsGroupByCreateDate.Count != 0)
                {
                    MonthlyReceiptModel model = null;
                    int status = SLIM_CONFIG.RECEIPT_STATUS_PAID;
                    foreach (var r in receiptsGroupByCreateDate)
                    {
                        if (r.PublishDate != null && r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED &&
                            DateTime.Today.Date >= r.PublishDate)
                        {
                            r.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID;
                            r.LastModified = DateTime.Now;
                            _receiptServices.Update(r);
                        }
                        if (r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED ||
                            r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPAID)
                        {
                            status = r.Status.Value;
                        }

                        model = new MonthlyReceiptModel();
                        model.ReceiptId = r.Id;
                        model.Status = r.Status.Value;
                        model.Block = r.House.Block;
                        model.Floor = r.House.Floor;
                        model.HouseName = r.House.HouseName;
                        model.DT_RowId = new StringBuilder("receipt_id_").Append(r.Id).ToString();

                        List<ReceiptDetail> orderDetails = r.ReceiptDetails.ToList();
                        double total = 0;
                        foreach (var od in orderDetails)
                        {
                            if (od.UtilityService.UtilityServiceCategory.Type ==
                                SLIM_CONFIG.UTILITY_SERVICE_TYPE_ELECTRICITY)
                            {
                                model.Electric = od.Quantity.Value;
                                model.ElectricCost = od.TotalBill.Value;
                            }
                            else if (od.UtilityService.UtilityServiceCategory.Type ==
                                     SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                            {
                                model.Water = od.Quantity.Value;
                                model.WaterCost = od.TotalBill.Value;
                            }
                            else if (od.UtilityService.UtilityServiceCategory.Type ==
                                     SLIM_CONFIG.UTILITY_SERVICE_TYPE_HOUSE_RENT)
                            {
                                model.HouseRentCost = od.TotalBill.Value;
                            }
                            else if (od.UtilityService.UtilityServiceCategory.Type ==
                                     SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
                            {
                                model.FixedCost = od.TotalBill.Value;
                            }
                            total += od.TotalBill.Value;
                        }
                        model.Total = total;
                        receiptModel.Add(model);
                    }
                    String electricName = "Chưa có giá";
                    String waterName = "Chưa có giá";
                    String houseRentName = "Chưa có giá";
                    String fixedCostName = "Chưa có giá";

                    var utilService = receipt.ReceiptDetails.Where(rd => rd.UtilityService.UtilityServiceCategory.Type ==
                                    SLIM_CONFIG.UTILITY_SERVICE_TYPE_ELECTRICITY).ToList();
                    if (utilService.Count != 0)
                    {
                        electricName = utilService.First().UtilityService.Name;
                    }
                    utilService = receipt.ReceiptDetails.Where(rd => rd.UtilityService.UtilityServiceCategory.Type ==
                                    SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).ToList();
                    if (utilService.Count != 0)
                    {
                        waterName = utilService.First().UtilityService.Name;
                    }
                    utilService = receipt.ReceiptDetails.Where(rd => rd.UtilityService.UtilityServiceCategory.Type ==
                                    SLIM_CONFIG.UTILITY_SERVICE_TYPE_HOUSE_RENT).ToList();
                    if (utilService.Count != 0)
                    {
                        houseRentName = utilService.First().UtilityService.Name;
                    }
                    utilService = receipt.ReceiptDetails.Where(rd => rd.UtilityService.UtilityServiceCategory.Type ==
                                    SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST).ToList();
                    if (utilService.Count != 0)
                    {
                        fixedCostName = utilService.First().UtilityService.Name;
                    }
                    Object obj = new
                    {
                        data = receiptModel,
                        publishDate = receipt.PublishDate.Value.ToString(AmsConstants.DateFormat),
                        title = receipt.Title,
                        status = status,
                        description = receipt.Description,
                        electricName = electricName,
                        waterName = waterName,
                        houseRentName = houseRentName,
                        fixedCostName = fixedCostName
                    };
                    return Json(obj, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ManageReceipt/GetMonthLyReceiptDetai")]
        public ActionResult GetMonthLyReceiptDetai(int receiptId)
        {
            MessageViewModels response = new MessageViewModels();
            Receipt receipt = _receiptServices.FindById(receiptId);
            if (receipt != null)
            {

                MonthlyReceiptModel model = null;
                model = new MonthlyReceiptModel();
                model.ReceiptId = receipt.Id;
                model.Status = receipt.Status.Value;
                model.Block = receipt.House.Block;
                model.Floor = receipt.House.Floor;
                model.HouseName = receipt.House.HouseName;
                model.PaymentDate = receipt.PaymentDate == null ? "Chưa thanh toán" : receipt.PaymentDate.Value.ToString(AmsConstants.DateFormat);

                List<ReceiptDetail> orderDetails = receipt.ReceiptDetails.ToList();
                double total = 0;
                foreach (var od in orderDetails)
                {
                    if (od.UtilityService.UtilityServiceCategory.Type ==
                        SLIM_CONFIG.UTILITY_SERVICE_TYPE_ELECTRICITY)
                    {
                        model.Electric = od.Quantity.Value;
                        model.ElectricCost = od.TotalBill.Value;
                    }
                    else if (od.UtilityService.UtilityServiceCategory.Type ==
                             SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                    {
                        model.Water = od.Quantity.Value;
                        model.WaterCost = od.TotalBill.Value;
                    }
                    else if (od.UtilityService.UtilityServiceCategory.Type ==
                             SLIM_CONFIG.UTILITY_SERVICE_TYPE_HOUSE_RENT)
                    {
                        model.HouseRentCost = od.TotalBill.Value;
                    }
                    else if (od.UtilityService.UtilityServiceCategory.Type ==
                             SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
                    {
                        model.FixedCost = od.TotalBill.Value;
                    }
                    total += od.TotalBill.Value;
                }
                model.Total = total;
                response.Data = model;
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy hóa đơn";
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ManageReceipt/CreateManualReceiptView")]
        public ActionResult ManageManualReceipt()
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
            ViewBag.userId = User.Identity.GetUserId();
            return View("CreateManualReceipt");
        }

        [HttpGet]
        [Route("Management/ManageReceipt/GetMonthlyResidentExpenseList")]
        public ActionResult GetMonthlyResidentExpenseList(string csvFilePath)
        {
            MessageViewModels response = new MessageViewModels();
            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            CsvContext cc = new CsvContext();
            IEnumerable<MonthlyResidentExpense> consumptionRecords =
                cc.Read<MonthlyResidentExpense>(
                    Server.MapPath(new StringBuilder(AmsConstants.CsvFilePath).Append(csvFilePath).ToString()),
                    inputFileDescription);
            //            var productsByName =
            //                from p in products
            //                orderby p.ProductName
            //                select new { p.ProductName, p.LaunchDate, p.Price, p.Description };
            UtilityService utilityService = _utilityServiceServices.FindById(150);
            MonthlyResidentExpenseModel monthlyResidentExpense = null;
            List<MonthlyResidentExpenseModel> monthlyResidentExpenseList = new List<MonthlyResidentExpenseModel>();
            House house = null;
            foreach (var r in consumptionRecords)
            {
                house = _houseServices.FindByBlockFloorHouseName(r.Block, r.Floor, r.HouseName);
                if (null != house)
                {
                    double totalElectric = 0;
                    double totalWater = 0;
                    monthlyResidentExpense = new MonthlyResidentExpenseModel();
                    monthlyResidentExpense.Status = SLIM_CONFIG.UTILITY_SERVICE_GET_CONSUMPTION_COMPLETE;
                    if (r.Electric == 0 || r.Water == 0)
                    {
                        monthlyResidentExpense.Status = SLIM_CONFIG.UTILITY_SERVICE_GET_CONSUMPTION_UN_COMPLETE;
                    }

                    monthlyResidentExpense.Electric = r.Electric;
                    monthlyResidentExpense.ElectricCost = totalElectric;
                    monthlyResidentExpense.Water = r.Water;
                    monthlyResidentExpense.WaterCost = totalWater;
                    monthlyResidentExpense.HouseName = r.HouseName;
                    monthlyResidentExpense.Floor = r.Floor;
                    monthlyResidentExpense.Block = r.Block;
                    monthlyResidentExpense.HouseId = house.Id;
                    monthlyResidentExpense.Total = totalWater + totalElectric;
                    monthlyResidentExpense.DT_RowId = new StringBuilder("consump_house_").Append(house.Id).ToString();

                    monthlyResidentExpenseList.Add(monthlyResidentExpense);

                }

            }
            response.Data = monthlyResidentExpenseList;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/ManageReceipt/CreateAutomationUtilitySevice")]
        public ActionResult CreateAutomationUtilitySevice(AutomationReceiptsTemplateModel automationReceipt)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (null != u)
            {
                Receipt receipt = new Receipt();
                receipt.CreateDate = DateTime.Now;
                receipt.LastModified = DateTime.Now;
                receipt.PublishDate = DateTime.ParseExact(automationReceipt.PublishDate, AmsConstants.DateFormat,
                    CultureInfo.CurrentCulture);
                receipt.Description = automationReceipt.Description;
                receipt.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED;
                receipt.Title = automationReceipt.Title;
                receipt.IsAutomation = SLIM_CONFIG.RECEIPT_TYPE_AUTOMATION;
                receipt.ManagerId = u.Id;
                UtilityService electricService =
                    _utilityServiceServices.FindById(automationReceipt.ElectricUtilServiceId);
                UtilityService waterService = _utilityServiceServices.FindById(automationReceipt.WaterUtilServiceId);
                UtilityService houseRendService =
                    _utilityServiceServices.FindById(automationReceipt.HouseRentUtilServiceId);
                UtilityService fixedCostService =
                    _utilityServiceServices.FindById(automationReceipt.FixCostUtilServiceId);
                var electricCost = 0.0;
                var waterCost = 0.0;
                var houseRentCost = 0.0;
                var FixedCost = 0.0;
                foreach (var houseRecord in automationReceipt.ResidentExpenseRecords)
                {
                    House house = _houseServices.FindById(houseRecord.HouseId);
                    if (house != null)
                    {
                        receipt.HouseId = house.Id;
                        _receiptServices.Add(receipt);
                        ReceiptDetail receiptDetail = null;

                        receiptDetail = new ReceiptDetail();
                        electricCost = calculateElectricUtiService(houseRecord.Electric,
                            electricService.UtilityServiceRangePrices.ToList());
                        receiptDetail.Quantity = houseRecord.Electric;
                        if (houseRecord.Electric != 0)
                        {
                            receiptDetail.UnitPrice = electricCost / houseRecord.Electric; //layvesion
                        }
                        else
                        {
                            receiptDetail.UnitPrice = 0;
                        }
                        receiptDetail.ServiceFeeId = electricService.Id;
                        receiptDetail.TotalBill = electricCost;
                        receiptDetail.ReceiptId = receipt.Id;
                        _receiptDetailServices.Add(receiptDetail);

                        receiptDetail = new ReceiptDetail();
                        waterCost = calculateWaterUtiService(houseRecord.Water,
                            waterService.UtilityServiceRangePrices.ToList(), house.Users.Count);
                        receiptDetail.Quantity = houseRecord.Water;
                        if (houseRecord.Water != 0)
                        {
                            receiptDetail.UnitPrice = electricCost/houseRecord.Water; //layvesion
                        }
                        else
                        {
                            receiptDetail.UnitPrice = 0;
                        }
                        receiptDetail.ServiceFeeId = waterService.Id;
                        receiptDetail.TotalBill = waterCost;
                        receiptDetail.ReceiptId = receipt.Id;
                        _receiptDetailServices.Add(receiptDetail);
                    }

                }
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/ManageReceipt/UpdateAutomationUtilitySevice")]
        public ActionResult UpdateAutomationUtilitySevice(AutomationReceiptsTemplateModel automationReceipt)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (null != u)
            {
                Receipt receipt = _receiptServices.FindById(automationReceipt.ReceiptId);
                if (receipt != null)
                {
                    List<Receipt> groupedByCreateDateReceipts =
                        _receiptServices.GetReceiptsByCreateDate(receipt.CreateDate.Value);
                    foreach (var receiptItem in groupedByCreateDateReceipts)
                    {
                        receiptItem.PublishDate = DateTime.ParseExact(automationReceipt.PublishDate,
                            AmsConstants.DateFormat,
                            CultureInfo.CurrentCulture);
                        receiptItem.LastModified = DateTime.Now;
                        receiptItem.Title = automationReceipt.Title;
                        receiptItem.Description = automationReceipt.Description;
                        _receiptServices.Update(receiptItem);

                        UtilityService service =
                            _utilityServiceServices.FindById(automationReceipt.ElectricUtilServiceId);


                        /*Stat update electric bill*/
                        List<ReceiptDetail> receiptDetails =
                            receiptItem.ReceiptDetails.Where(
                                rd =>
                                    rd.UtilityService.UtilityServiceCategory.Type ==
                                    SLIM_CONFIG.UTILITY_SERVICE_TYPE_ELECTRICITY).ToList();
                        if (receiptDetails.Count != 0)
                        {
                            ReceiptDetail electricReceiptDetail =
                                _receiptDetailServices.FindById(receiptDetails.First().Id);
                            if (service != null &&
                                service.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_ELECTRICITY &&
                                service.Id != electricReceiptDetail.ServiceFeeId)
                            {
                                electricReceiptDetail.ServiceFeeId = service.Id;
                                electricReceiptDetail.TotalBill =
                                    calculateElectricUtiService(electricReceiptDetail.Quantity.Value,
                                        service.UtilityServiceRangePrices.ToList());
                                if (electricReceiptDetail.Quantity.Value != 0)
                                {
                                    electricReceiptDetail.UnitPrice = electricReceiptDetail.TotalBill/
                                                                      electricReceiptDetail.Quantity.Value;
                                }
                                else
                                {
                                    electricReceiptDetail.UnitPrice = 0;
                                }
                                
                                _receiptDetailServices.Update(electricReceiptDetail);
                            } //update
                        }
                        else
                        {
                            ReceiptDetail electricReceiptDetail = new ReceiptDetail();
                            if (service != null &&
                                service.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_ELECTRICITY)
                            {
                                electricReceiptDetail.ServiceFeeId = service.Id;
                                electricReceiptDetail.TotalBill = 0.0;
                                electricReceiptDetail.Quantity = 0;
                                electricReceiptDetail.UnitPrice = 0.0;
                                electricReceiptDetail.ReceiptId = receiptItem.Id;
                                _receiptDetailServices.Add(electricReceiptDetail);
                            } //add
                        }
                        /*End update electric bill*/

                        /*Stat update water bill*/
                        service = _utilityServiceServices.FindById(automationReceipt.WaterUtilServiceId);
                        receiptDetails =
                            receiptItem.ReceiptDetails.Where(
                                rd =>
                                    rd.UtilityService.UtilityServiceCategory.Type ==
                                    SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).ToList();
                        if (receiptDetails.Count != 0)
                        {
                            ReceiptDetail waterReceiptDetail = _receiptDetailServices.FindById(receiptDetails.First().Id);
                            if (service != null &&
                                service.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER &&
                                service.Id != waterReceiptDetail.ServiceFeeId)
                            {
                                waterReceiptDetail.ServiceFeeId = service.Id;
                                waterReceiptDetail.TotalBill =
                                    calculateWaterUtiService(waterReceiptDetail.Quantity.Value,
                                        service.UtilityServiceRangePrices.ToList(), receiptItem.House.Users.Count);
                                if (waterReceiptDetail.Quantity.Value != 0 )
                                {
                                    waterReceiptDetail.UnitPrice = waterReceiptDetail.TotalBill /
                                                               waterReceiptDetail.Quantity.Value;
                                }
                                else
                                {
                                    waterReceiptDetail.UnitPrice = 0;
                                }
                                
                                _receiptDetailServices.Update(waterReceiptDetail);
                            } //update
                        }
                        else
                        {
                            ReceiptDetail waterReceiptDetail = new ReceiptDetail();
                            if (service != null &&
                                service.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                            {
                                waterReceiptDetail.ServiceFeeId = service.Id;
                                waterReceiptDetail.TotalBill = 0.0;
                                waterReceiptDetail.Quantity = 0;
                                waterReceiptDetail.UnitPrice = 0.0;
                                waterReceiptDetail.ReceiptId = receiptItem.Id;
                                _receiptDetailServices.Add(waterReceiptDetail);
                            } //add
                        }
                        /*End update water bill*/

                        /*Stat update house rent bill*/
                        service = _utilityServiceServices.FindById(automationReceipt.HouseRentUtilServiceId);
                        receiptDetails =
                            receiptItem.ReceiptDetails.Where(
                                rd =>
                                    rd.UtilityService.UtilityServiceCategory.Type ==
                                    SLIM_CONFIG.UTILITY_SERVICE_TYPE_HOUSE_RENT).ToList();
                        if (receiptDetails.Count != 0)
                        {
                            ReceiptDetail houseRentReceiptDetail =
                                _receiptDetailServices.FindById(receiptDetails.First().Id);
                            if (service != null &&
                                service.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_HOUSE_RENT &&
                                service.Id != houseRentReceiptDetail.ServiceFeeId)
                            {
                                houseRentReceiptDetail.ServiceFeeId = service.Id;
                                houseRentReceiptDetail.TotalBill = service.Price;
                                houseRentReceiptDetail.UnitPrice = service.Price;
                                houseRentReceiptDetail.Quantity = 1;
                                _receiptDetailServices.Update(houseRentReceiptDetail);
                            } //update
                        }
                        else
                        {
                            ReceiptDetail houseRentReceiptDetail = new ReceiptDetail();
                            if (service != null &&
                                service.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_HOUSE_RENT)
                            {
                                houseRentReceiptDetail.ServiceFeeId = service.Id;
                                houseRentReceiptDetail.TotalBill = service.Price;
                                houseRentReceiptDetail.Quantity = 1;
                                houseRentReceiptDetail.UnitPrice = service.Price;
                                houseRentReceiptDetail.ReceiptId = receiptItem.Id;
                                _receiptDetailServices.Add(houseRentReceiptDetail);
                            } //add
                        }
                        /*End update house rent bill*/

                        /*Stat update fixed cost bill*/
                        service = _utilityServiceServices.FindById(automationReceipt.FixCostUtilServiceId);
                        receiptDetails =
                            receiptItem.ReceiptDetails.Where(
                                rd =>
                                    rd.UtilityService.UtilityServiceCategory.Type ==
                                    SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST).ToList();
                        if (receiptDetails.Count != 0)
                        {
                            ReceiptDetail fixedCostReceiptDetail =
                                _receiptDetailServices.FindById(receiptDetails.First().Id);
                            if (service != null &&
                                service.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST &&
                                service.Id != fixedCostReceiptDetail.ServiceFeeId)
                            {
                                fixedCostReceiptDetail.ServiceFeeId = service.Id;
                                fixedCostReceiptDetail.TotalBill = service.Price;
                                fixedCostReceiptDetail.UnitPrice = service.Price;
                                fixedCostReceiptDetail.Quantity = 1;
                                _receiptDetailServices.Update(fixedCostReceiptDetail);
                            } //update
                        }
                        else
                        {
                            ReceiptDetail fixedCostReceiptDetail = new ReceiptDetail();
                            if (service != null &&
                                service.UtilityServiceCategory.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
                            {
                                fixedCostReceiptDetail.ServiceFeeId = service.Id;
                                fixedCostReceiptDetail.TotalBill = service.Price;
                                fixedCostReceiptDetail.Quantity = 1;
                                fixedCostReceiptDetail.UnitPrice = service.Price;
                                fixedCostReceiptDetail.ReceiptId = receiptItem.Id;
                                _receiptDetailServices.Add(fixedCostReceiptDetail);
                            } //add
                        }
                        /*End update fixed cost bill*/

                    }
                }
                else
                {
                    response.StatusCode = -1;
                    response.Msg = "KHông tìn thấy hóa đơn";
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "KHông tìn thấy hóa đơn";
            }
            return Json(response);
        }

        [HttpGet]
        [Route("Management/ManageReceipt/CreateAutomationReceiptView")]
        public ActionResult ManageAutomationReceipt()
        {
            UtilityServicesGroupByTypeModel services = GetAllAparmentUtilitySevices();
            ViewBag.electricSevices = services.ElectricSevices;
            ViewBag.waterSevices = services.WaterSevices;
            ViewBag.houseRentSevices = services.HouseRentSevices;
            ViewBag.fixedCostSevices = services.FixedCostSevices;
            return View("CreateAutomationReceipt");
        }

        [HttpPost]
        [Route("Management/ManageReceipt/UpdateAutomationReceiptStatus")]
        public ActionResult UpdateAutomationReceiptStatus(int receiptId, int mode)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (null != u)
            {
                Receipt receipt = _receiptServices.FindById(receiptId);
                if (receipt != null)
                {
                    try
                    {
                        List<Receipt> receiptGroupByCreateDate = _receiptServices.GetReceiptsByCreateDate(receipt.CreateDate.Value);
                        if (mode == SLIM_CONFIG.MODE_DELETE)
                        {
                            foreach (var r in receiptGroupByCreateDate)
                            {
                                List<ReceiptDetail> temp3 = r.ReceiptDetails.ToList();
                                //This is very inportant to delete this table
                                r.ReceiptDetails = null;
                                if (r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
                                {
                                    _receiptServices.Delete(r);
                                    foreach (var detail in temp3)
                                    {
                                        _receiptDetailServices.DeleteById(detail.Id);
                                    }
                                }
                            }
                            
                        }
                        else if (mode == SLIM_CONFIG.MODE_PUBLISH)
                        {
                            foreach (var receiptItem in receiptGroupByCreateDate)
                            {
                                if (receiptItem.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
                                {
                                    receiptItem.PublishDate = DateTime.Today;
                                    receiptItem.LastModified = DateTime.Now;
                                    receiptItem.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID;
                                    _receiptServices.Update(receiptItem);
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {

                        response.StatusCode = -1;
                        response.Msg = "Cập nhật thất bại";
                    }

                }
                else
                {
                    response.StatusCode = -1;
                    response.Msg = "KHông tìn thấy hóa đơn";
                }
            }
            return Json(response);
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
                        eReceipt.PublishDate = DateTime.ParseExact(receipt.PublishDate, AmsConstants.DateFormat,
                            CultureInfo.CurrentCulture);
                        eReceipt.Type = receipt.ReceiptType;
                        eReceipt.ManagerId = u.Id;
                        eReceipt.Description = receipt.ReceiptDesc;
                        eReceipt.Title = receipt.ReceiptTitle;
                        eReceipt.IsAutomation = SLIM_CONFIG.RECEIPT_TYPE_MANUAL;
                        if (receipt.Mode == SLIM_CONFIG.RECEIPT_ADD_MODE_SAVE_PUBLISH)
                        {
                            eReceipt.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID;
                        }
                        else
                        {
                            eReceipt.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED;
                        }
                        _receiptServices.Add(eReceipt);

                        UtilityService item = null;
                        ReceiptDetail detail = null;
                        foreach (var i in receipt.ListItem)
                        {
                            item = new UtilityService();
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

                        UtilityService item = null;
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
                                        UtilityService fee = new UtilityService();
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

                                        UtilityService updateServiceFee =
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
                                    _serviceChargeSevices.DeleteById(deletedRecDetailId.UtilityService.Id);
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
            return PartialView("_receiptDetailItemList");
        }

        [HttpGet]
        [Route("Management/ManageReceipt/UpdateStatus")]
        public ActionResult UpdateStatusReceipt(int ReceiptId, int Status)
        {
            MessageViewModels response = new MessageViewModels();
            Receipt receipt = _receiptServices.FindById(ReceiptId);
            if (null != receipt)
            {
                User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
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

        [HttpPost]
        [Route("Management/ManageReceipt/UploadConsumptionRecord")]
        public ActionResult UploadConsumptionRecord()
        {
            MessageViewModels response = new MessageViewModels();
            string fileName = "";
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["csvFile"];
                if (pic != null && pic.FileName != null)
                {
                    string newPath = Server.MapPath(AmsConstants.CsvFilePath);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    DateTime now = DateTime.Now;
                    fileName =
                        new StringBuilder().Append("CSV_")
                            .Append(now.Day)
                            .Append("_")
                            .Append(now.Month)
                            .Append("_")
                            .Append(now.Year)
                            .Append("_")
                            .Append(now.Hour)
                            .Append("_Time_")
                            .Append(now.Minute)
                            .Append("_")
                            .Append(now.Millisecond)
                            .Append(".csv")
                            .ToString();
                    pic.SaveAs(new StringBuilder(newPath).Append(fileName).ToString());
                    response.Data = fileName;
                }
                else
                {
                    response.StatusCode = -1;
                    response.Msg = "Tải lên tập tin  thất bại";
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Tải lên tập tin  thất bại";
            }
            return Json(response);
        }

        private double calculateElectricUtiService(double consumption, List<UtilityServiceRangePrice> rangePrices)
        {
            double total = 0;
            double previous = 0;
            foreach (UtilityServiceRangePrice u in rangePrices)
            {
                double calculatingPart = 0;
                if (consumption >= u.ToAmount.Value)
                {
                    calculatingPart = u.ToAmount.Value - previous;
                    previous = u.ToAmount.Value;
                }
                else if (consumption >= u.FromAmount.Value && consumption < u.ToAmount.Value)
                {
                    calculatingPart = consumption - previous;
                }
                total += calculatingPart * u.Price.Value;
            }
            return total;
        }

        private double calculateWaterUtiService(double consumption, List<UtilityServiceRangePrice> rangePrices,
            int numberOfResident)
        {
            double total = 0;
            double previous = 0;
            foreach (UtilityServiceRangePrice u in rangePrices)
            {
                u.FromAmount = u.FromAmount * numberOfResident;
                u.ToAmount = u.ToAmount * numberOfResident;

                double calculatingPart = 0;
                if (consumption >= u.ToAmount.Value)
                {
                    calculatingPart = u.ToAmount.Value - previous;
                    previous = u.ToAmount.Value;
                }
                else if (consumption >= u.FromAmount.Value && consumption < u.ToAmount.Value)
                {
                    calculatingPart = consumption - previous;
                }
                total += calculatingPart * u.Price.Value;
            }
            return total;
        }

        private UtilityServicesGroupByTypeModel GetAllAparmentUtilitySevices()
        {
            UtilityServiceCategory electricCategory =
                _utilityCategoryServices.FindByType(SLIM_CONFIG.UTILITY_SERVICE_TYPE_ELECTRICITY);
            UtilityServiceCategory waterCategory =
                _utilityCategoryServices.FindByType(SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER);
            UtilityServiceCategory houseRentCategory =
                _utilityCategoryServices.FindByType(SLIM_CONFIG.UTILITY_SERVICE_TYPE_HOUSE_RENT);
            UtilityServiceCategory fixedCostCategory =
                _utilityCategoryServices.FindByType(SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST);

            List<UtilityService> electricSevices = new List<UtilityService>();
            List<UtilityService> waterSevices = new List<UtilityService>();
            List<UtilityService> houseRentSevices = new List<UtilityService>();
            List<UtilityService> fixedCostSevices = new List<UtilityService>();

            if (electricCategory != null)
            {
                electricSevices = electricCategory.UtilityServices.ToList();
            }
            if (waterCategory != null)
            {
                waterSevices = waterCategory.UtilityServices.ToList();
            }
            if (houseRentCategory != null)
            {
                houseRentSevices = waterCategory.UtilityServices.ToList();
            }
            if (fixedCostCategory != null)
            {
                fixedCostSevices = waterCategory.UtilityServices.ToList();
            }

            UtilityServicesGroupByTypeModel utilServiceGroup = new UtilityServicesGroupByTypeModel();
            utilServiceGroup.ElectricSevices = electricSevices;
            utilServiceGroup.WaterSevices = waterSevices;
            utilServiceGroup.HouseRentSevices = houseRentSevices;
            utilServiceGroup.FixedCostSevices = fixedCostSevices;
            return utilServiceGroup;
        }
    }
}
