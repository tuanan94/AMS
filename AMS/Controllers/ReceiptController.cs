using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using AMS.Constant;
using AMS.Models;
using AMS.Service;
using LINQtoCSV;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

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
        private Service.HouseServices _houseServices = new Service.HouseServices();
        private ReceiptDetailServices _receiptDetailServices = new ReceiptDetailServices();
        private ServiceChargeSevices _serviceChargeSevices = new ServiceChargeSevices();
        private BlockServices _blockServices = new BlockServices();
        private UtilityServiceServices _utilityServiceServices = new UtilityServiceServices();
        private UtilServiceForHouseCatServices _utlSrvForHouseCatServices = new UtilServiceForHouseCatServices();
        private BalanceSheetService _balanceSheetService = new BalanceSheetService();
        private UtilityServiceCateoryService _utilityServiceCateoryService = new UtilityServiceCateoryService();
        private TransactionService _transactionService = new TransactionService();
        private HouseCategoryServices _houseCategoryServices = new HouseCategoryServices();

        //        private readonly string parternTime = "dd-MM-yyyy HH:mm";

        [HttpGet]
        [Route("Home/ManageReceipt/View")]
        public ActionResult GetUserReceipt()
        {
            //            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            //            if (null != u)
            //            {
            //                Receipt lastPaidReceipt = _receiptServices.GetAllReceiptFromTheLastPayment(u.HouseId.Value);
            //                if (lastPaidReceipt != null)
            //                {
            //                    List<Receipt> receiptFromTheLastPaymentDate = _receiptServices.GetAllReceiptFromLastPaidReceipt(u.HouseId.Value, lastPaidReceipt.PaymentDate.Value);
            //
            //                    List<Receipt> unpaidReceipts =
            //                    receiptFromTheLastPaymentDate.Where(r => r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPAID).ToList();
            //                    double unpaid = 0;
            //                    double paid = 0;
            //                    foreach (var r in unpaidReceipts)
            //                    {
            //                        unpaid += r.ReceiptDetails.Sum(rd => rd.Total).Value;
            //                    }
            //                    List<Receipt> paidReceipts =
            //                        receiptFromTheLastPaymentDate.Where(r => r.Status == SLIM_CONFIG.RECEIPT_STATUS_PAID).ToList();
            //                    foreach (var r in paidReceipts)
            //                    {
            //                        paid += r.ReceiptDetails.Sum(rd => rd.Total).Value;
            //                    }
            //                }
            //                
            //
            //                ViewBag.unpaidTotal = unpaid;
            //                ViewBag.paidTotal = paid;
            //                ViewBag.total = paid + unpaid;
            //                ViewBag.receipts = receipts;
            //            }
            return View("ViewReceipt");
        }

        [HttpGet]
        [Route("Home/ManageReceipt/View/HouseExpenseStatus")]
        public ActionResult ViewHouseExpenseStatus()
        {
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (null != u)
            {
                List<Receipt> receipts = _receiptServices.GetReceiptByHouseId(u.HouseId.Value);
                List<Receipt> unpaidReceipts =
                    receipts.Where(r => r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPAID).ToList();
                double unpaid = 0;
                double paid = 0;
                foreach (var r in unpaidReceipts)
                {
                    unpaid += r.ReceiptDetails.Sum(rd => rd.Total).Value;
                }
                List<Receipt> paidReceipts =
                    receipts.Where(r => r.Status == SLIM_CONFIG.RECEIPT_STATUS_PAID).ToList();
                foreach (var r in paidReceipts)
                {
                    paid += r.ReceiptDetails.Sum(rd => rd.Total).Value;
                }

                ViewBag.unpaidTotal = unpaid;
                ViewBag.paidTotal = paid;
                ViewBag.total = paid + unpaid;
                ViewBag.receipts = receipts;
            }

            return View("HouseExpenseStatus");
        }

        [HttpGet]
        [Route("Home/ManageReceipt/GetOrderList")]
        public ActionResult GetOrderList(int userId, string from, string to)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(userId);
            if (null != u)
            {
                try
                {
                    DateTime fromDate = DateTime.ParseExact(from, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                    DateTime toDate = DateTime.ParseExact(to, AmsConstants.DateFormat, CultureInfo.CurrentCulture);

                    List<Receipt> receipts = _receiptServices.GetReceiptByHouseFromDateToDate(u.HouseId.Value, fromDate, toDate);
                    List<ReceiptInfoModel> receiptModel = new List<ReceiptInfoModel>();
                    double sum = 0;
                    double paidAmount = 0;
                    double restAmount = 0;
                    if (null != receipts && receipts.Count != 0)
                    {
                        ReceiptInfoModel model = null;
                        foreach (var receipt in receipts)
                        {
                            if (receipt.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
                            {
                                if (DateTime.Today.Date >= receipt.PublishDate.Value.Date)
                                {
                                    receipt.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID;
                                    receipt.LastModified = DateTime.Now;
                                    _receiptServices.Update(receipt);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            model = new ReceiptInfoModel();
                            model.ReceiptId = receipt.Id;
                            model.PublishDate = receipt.PublishDate.Value.ToString(AmsConstants.DateTimeFormat);
                            model.PublishDateStick = receipt.PublishDate.Value.Ticks;
                            model.Status = receipt.Status.Value;
                            model.HouseName = receipt.House.HouseName;
                            model.ReceiptTitle = receipt.Title;

                            List<ReceiptDetail> receipDetail = receipt.ReceiptDetails.ToList();
                            double total = 0;
                            foreach (var trans in receipDetail)
                            {
                                total += trans.Total.Value;
                            }
                            model.TotalOrder = total;
                            sum += total;
                            if (model.Status == SLIM_CONFIG.RECEIPT_STATUS_PAID)
                            {
                                paidAmount += model.TotalOrder;
                            }
                            receiptModel.Add(model);
                        }
                        restAmount = sum - paidAmount;
                        object obj = new
                        {
                            Data = receiptModel,
                            Total = sum,
                            PaidAmount = paidAmount,
                            RestAmount = restAmount
                        };
                        response.Data = obj;
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        response.StatusCode = -1;
                        response.Msg = "Không tìn thấy cư dân";
                    }
                }
                catch (Exception)
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
        [Route("Home/ManageReceipt/GetUnpaidReceipt")]
        public ActionResult GetOrderList(int userId)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(userId);
            if (null != u)
            {
                try
                {
                    List<Receipt> receipts = _receiptServices.GetAllUnpaidReceip(u.HouseId.Value);
                    List<ReceiptInfoModel> receiptModel = new List<ReceiptInfoModel>();
                    double sum = 0;
                    double paidAmount = 0;
                    double restAmount = 0;
                    if (receipts.Count == 0)
                    {
                        receipts = _receiptServices.GetLastActivityReceipt(u.HouseId.Value);
                    }

                    ReceiptInfoModel model = null;
                    foreach (var receipt in receipts)
                    {
                        if (receipt.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
                        {
                            receipt.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID;
                            receipt.LastModified = DateTime.Now;
                            _receiptServices.Update(receipt);
                        }
                        model = new ReceiptInfoModel();
                        model.ReceiptId = receipt.Id;
                        model.PublishDate = receipt.PublishDate.Value.ToString(AmsConstants.DateTimeFormat);
                        model.PublishDateStick = receipt.PublishDate.Value.Ticks;
                        model.Status = receipt.Status.Value;
                        model.HouseName = receipt.House.HouseName;
                        model.ReceiptTitle = receipt.Title;

                        List<ReceiptDetail> receipDetail = receipt.ReceiptDetails.ToList();
                        double total = 0;
                        foreach (var trans in receipDetail)
                        {
                            total += trans.Total.Value;
                        }
                        model.TotalOrder = total;
                        sum += total;
                        if (model.Status == SLIM_CONFIG.RECEIPT_STATUS_PAID)
                        {
                            paidAmount += model.TotalOrder;
                        }
                        receiptModel.Add(model);
                    }
                    restAmount = sum - paidAmount;
                    object obj = new
                    {
                        Data = receiptModel,
                        Total = sum,
                        PaidAmount = paidAmount,
                        RestAmount = restAmount
                    };
                    response.Data = obj;
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
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
        [Authorize]
        [Route("Management/ManageReceipt/View")]
        public ActionResult ViewManagerOrderList()
        {
            ViewBag.userId = User.Identity.GetUserId();
            return View("ViewManagerReceipt");
        }

        [HttpGet]
        [Route("Home/ManageReceipt/ViewElectricBill")]
        public ActionResult ViewElectricBill(int receiptDetailId)
        {
            ReceiptDetail rd = _receiptDetailServices.FindById(receiptDetailId);
            List<UtilityServiceRangePrice> rangePrice = rd.UtilityService.UtilityServiceRangePrices.ToList();
            MonthlyReceiptModel monthlyReceiptModel = new MonthlyReceiptModel();

            double total = 0;
            double tempTotal = 0;
            double previous = 0;
            double fromAmount = 0;
            double toAmount = 0;
            int consumption = rd.Quantity.Value;
            UtilityServiceRangePriceModel rangePriceIncash = null;
            List<UtilityServiceRangePriceModel> listRangePriceIncash = new List<UtilityServiceRangePriceModel>();

            foreach (UtilityServiceRangePrice u in rangePrice)
            {
                rangePriceIncash = new UtilityServiceRangePriceModel();
                rangePriceIncash.ToAmount = u.ToAmount.Value.ToString();
                rangePriceIncash.FromAmount = u.FromAmount.Value.ToString();

                fromAmount = u.FromAmount.Value;
                toAmount = u.ToAmount.Value;

                double calculatingPart = 0;
                if (consumption >= toAmount)
                {
                    calculatingPart = toAmount - previous;
                    previous = toAmount;
                }
                else if (consumption >= fromAmount && consumption < toAmount)
                {
                    calculatingPart = consumption - previous;
                }

                if (calculatingPart == 0)
                {
                    continue;
                }

                tempTotal = calculatingPart * u.Price.Value;
                total += tempTotal;

                rangePriceIncash.Qty = calculatingPart;
                rangePriceIncash.Price = u.Price.Value;
                rangePriceIncash.Total = tempTotal;
                rangePriceIncash.Name = u.Name;
                listRangePriceIncash.Add(rangePriceIncash);
            }

            monthlyReceiptModel.WaterRangePrices = listRangePriceIncash;
            monthlyReceiptModel.Total = rd.Total.Value;
            monthlyReceiptModel.FromNumber = rd.FromNumber.Value;
            monthlyReceiptModel.ToNumber = rd.ToNumber.Value;
            monthlyReceiptModel.Qty = rd.Quantity.Value;
            monthlyReceiptModel.ReceiptDetailId = rd.Id;

            ViewBag.monthlyReceiptModel = monthlyReceiptModel;
            ViewBag.utilServPriceFrame = rd.UtilityService.Name;

            return View("ViewWateBiiling");
        }

        [HttpGet]
        [Route("Home/ManageReceipt/GetLast12MonthReceiptBill")]
        public ActionResult GetLast12MonthReceiptBill(int receiptDetailId)
        {
            MessageViewModels response = new MessageViewModels();
            ReceiptDetail rd = _receiptDetailServices.FindById(receiptDetailId);
            List<ReceiptDetail> last12BillFromThisBill =
                _receiptDetailServices.Get12ReceiptFromThisReceipt(rd.Receipt.HouseId.Value, rd.Receipt.PublishDate.Value.Date);
            HistoryWaterUsingModel waterUsing = null;
            List<HistoryWaterUsingModel> last12MonthUsing = new List<HistoryWaterUsingModel>();
            foreach (var lastRd in last12BillFromThisBill)
            {
                waterUsing = new HistoryWaterUsingModel();
                waterUsing.Amount = lastRd.Quantity.Value;
                waterUsing.Month = lastRd.Receipt.BalanceSheet.StartDate.Value.ToString(AmsConstants.DateFormat);
                last12MonthUsing.Add(waterUsing);
            }
            int min = last12BillFromThisBill.Min(t => t.Quantity).Value;
            int max = last12BillFromThisBill.Max(t => t.Quantity).Value;
            object obj = new
            {
                Data = last12MonthUsing,
                Min = min,
                Max = max
            };
            response.Data = obj;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ManageReceipt/Admin/GetManagerOrderList")]
        public ActionResult GetManagerOrderList(int userId)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(userId);
            if (null != u)
            {
                List<Receipt> receiptsGroupByCreateDate = _receiptServices.GetAllReceiptsForHouseGroupByCreateDate();
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
                        model.CreateDate = r.CreateDate.Value.ToString(AmsConstants.DateTimeFormat);
                        model.CreateDateLong = r.CreateDate.Value.Ticks;
                        model.Status = r.Status.Value;
                        model.IsAutomation = r.IsAutomation.Value;

                        if (model.IsAutomation == SLIM_CONFIG.RECEIPT_TYPE_AUTOMATION)
                        {
                            if (r.Status.Value != SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
                            {
                                if (_receiptServices.CheckAllAutomationReceiptIsPaid(r.CreateDate.Value))
                                {
                                    model.Status = SLIM_CONFIG.RECEIPT_STATUS_PAID;
                                }
                                else
                                {
                                    model.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID;
                                }
                            }
                            else
                            {
                                model.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED;
                            }

                        }
                        model.Block = r.House.Block.BlockName;
                        model.Floor = r.House.Floor;
                        model.HouseName = r.House.HouseName;
                        model.ReceiptTitle = r.Title;
                        List<ReceiptDetail> receipDetails = r.ReceiptDetails.ToList();
                        double total = 0;
                        foreach (var receiptDetail in receipDetails)
                        {
                            //                            total += od.UnitPrice.Value * od.Quantity.Value;
                            total += receiptDetail.Total.Value;
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
                    BalanceSheet curentBls = _balanceSheetService.GetCurentActivateBalanceSheet();
                    if (null != curentBls)
                    {
                        ViewBag.forMonthOfBls = curentBls.StartDate.Value.ToString(AmsConstants.DateFormat);
                        ViewBag.forMonthViewOfBls = curentBls.StartDate.Value.ToString(AmsConstants.MonthYearFormat);
                    }
                    else
                    {
                        ViewBag.forMonthOfBls = null;
                    }

                    List<House> floor = _houseServices.GetFloorHasResidentInBlock(receipt.House.Block.Id);
                    List<House> rooms = _houseServices.GetActiveRoomsInFloor(receipt.House.Block.Id, receipt.House.Floor);
                    ViewBag.block = _blockServices.GetAllBlocks();
                    ViewBag.firstBlockFloor = floor;
                    ViewBag.rooms = rooms;

                    ViewBag.receipt = receipt;
                    return View("UpdateReceipt");
                }
                return View("ViewReceipt");
            }
            return View("ViewReceipt");
        }

        [HttpGet]
        [Route("Management/ManageReceipt/ViewBatchReceiptDetail")]
        public ActionResult ViewBatchReceiptDetail(long receiptId)
        {
            MessageViewModels response = new MessageViewModels();
            var receipts = _receiptServices.GetReceiptsByCreateDate(new DateTime(receiptId));
            if (null != receipts && receipts.Count != 0)
            {
                UtilityServiceIdNameModel item = null;

                Receipt receipt = receipts.First();

                AutomationReceiptsTemplateModel receiptInfo = new AutomationReceiptsTemplateModel();
                receiptInfo.PublishDate = receipt.PublishDate.Value.ToString(AmsConstants.DateFormat);
                receiptInfo.Description = receipt.Description;
                receiptInfo.Title = receipt.Title;
                receiptInfo.Status = receipt.Status.Value;
                receiptInfo.ReceiptId = receipt.Id;
                receiptInfo.ForMonth = receipt.BalanceSheet.StartDate.Value.ToString(AmsConstants.MonthYearFormat);
                receiptInfo.BalanceSheetId = receipt.BalanceSheet.Id;

                foreach (var receiptDetail in receipt.ReceiptDetails)
                {
                    if (receiptDetail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
                    {
                        receiptInfo.WaterUtilServiceId = receiptDetail.UtilityService.Id;
                    }
                    if (receiptDetail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
                    {
                        receiptInfo.FixCostUtilServiceId = receiptDetail.UtilityService.Id;
                    }
                }
                response.Data = receiptInfo;
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
        public ActionResult GetBatchReceiptList(long receiptId)
        {
            MessageViewModels response = new MessageViewModels();
            List<Receipt> receiptsGroupByCreateDate =
                _receiptServices.GetReceiptsByCreateDate(new DateTime(receiptId));
            List<MonthlyReceiptModel> receiptModel = new List<MonthlyReceiptModel>();
            if (null != receiptsGroupByCreateDate && receiptsGroupByCreateDate.Count != 0)
            {
                var receipt = receiptsGroupByCreateDate.First();

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
                    model.Block = r.House.Block.BlockName;
                    model.Floor = r.House.Floor;
                    model.HouseName = r.House.HouseName;
                    model.DT_RowId = new StringBuilder("receipt_id_").Append(r.Id).ToString();

                    List<ReceiptDetail> orderDetails = r.ReceiptDetails.ToList();
                    double total = 0;
                    foreach (var od in orderDetails)
                    {
                        if (od.UtilityService.Type ==
                                 SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                        {
                            model.FromNumber =
                            model.Water = od.Quantity.Value;
                            model.WaterCost = od.Total.Value;
                            model.FromNumber = od.FromNumber.Value;
                            model.ToNumber = od.ToNumber.Value;
                        }
                        else if (od.UtilityService.Type ==
                                SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
                        {
                            model.FixedCost = od.Total.Value;
                        }
                        total += od.Total.Value;
                    }
                    model.Total = total;
                    receiptModel.Add(model);
                }
                String waterName = "Chưa có giá";
                String fixedCostName = "Chưa có giá";

                var utilService = receipt.ReceiptDetails.Where(rd => rd.UtilityService.Type ==
                                SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).ToList();
                if (utilService.Count != 0)
                {
                    waterName = utilService.First().UtilityService.Name;
                }

                utilService = receipt.ReceiptDetails.Where(rd => rd.UtilityService.Type ==
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
                    forMonth = receipt.BalanceSheet.StartDate.Value.ToString(AmsConstants.MonthYearFormat),
                    blsId = receipt.BalanceSheet.Id,
                    status = status,
                    description = receipt.Description,
                    waterName = waterName,
                    fixedCostName = fixedCostName
                };
                return Json(obj, JsonRequestBehavior.AllowGet);
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
                model.Block = receipt.House.Block.BlockName;
                model.Floor = receipt.House.Floor;
                model.HouseName = receipt.House.HouseName;
                model.PaymentDate = receipt.PaymentDate == null ? "Chưa thanh toán" : receipt.PaymentDate.Value.ToString(AmsConstants.DateFormat);

                List<ReceiptDetail> orderDetails = receipt.ReceiptDetails.ToList();
                double total = 0;
                List<UtilityServiceRangePriceModel> rangeWaterPrices = new List<UtilityServiceRangePriceModel>();
                UtilityServiceRangePriceModel priceModel = null;
                foreach (var od in orderDetails)
                {
                    if (od.UtilityService.Type ==
                            SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                    {
                        model.Water = od.Quantity.Value;
                        model.WaterCost = od.Total.Value;
                        model.FromNumber = od.FromNumber.Value;
                        model.ToNumber = od.ToNumber.Value;

                        foreach (var price in od.UtilityService.UtilityServiceRangePrices)
                        {
                            priceModel = new UtilityServiceRangePriceModel();
                            priceModel.FromAmount = price.FromAmount.Value.ToString();
                            priceModel.ToAmount = price.ToAmount.Value.ToString();
                            priceModel.Price = price.Price.Value;
                            rangeWaterPrices.Add(priceModel);
                        }
                        model.WaterRangePrices = rangeWaterPrices;
                    }
                    else if (od.UtilityService.Type ==
                             SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
                    {
                        model.FixedCost = od.Total.Value;
                    }
                    total += od.Total.Value;
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
        [Route("Management/ManageReceipt/GetMonthlyResidentExpenseList")]
        public ActionResult GetMonthlyResidentExpenseList(string csvFilePath)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                CsvFileDescription inputFileDescription = new CsvFileDescription
                {
                    SeparatorChar = ',',
                    FirstLineHasColumnNames = true
                };
                CsvContext cc = new CsvContext();
                List<MonthlyResidentExpense> consumptionRecords =
                    cc.Read<MonthlyResidentExpense>(
                        Server.MapPath(new StringBuilder(AmsConstants.CsvFilePath).Append(csvFilePath).ToString()),
                        inputFileDescription).ToList();
                //            var productsByName =
                //                from p in products
                //                orderby p.ProductName
                //                select new { p.ProductName, p.LaunchDate, p.Price, p.Description };
                MonthlyResidentExpenseModel monthlyResidentExpense = null;
                List<MonthlyResidentExpenseModel> monthlyResidentExpenseList = new List<MonthlyResidentExpenseModel>();
                House house = null;
                MonthlyResidentExpense houseConsummption = null;
                for (int i = 0; i < consumptionRecords.Count(); i++)
                {
                    houseConsummption = consumptionRecords[i];
                    if (i == 0)
                    {

                    }
                    else
                    {
                        if (houseConsummption.Block != null && houseConsummption.Floor != null &&
                            houseConsummption.HouseName != null)
                        {
                            house = _houseServices.FindByBlockFloorHouseName(houseConsummption.Block,
                                houseConsummption.Floor, houseConsummption.HouseName);

                            if (null != house)
                            {
                                double totalWater = 0;
                                monthlyResidentExpense = new MonthlyResidentExpenseModel();
                                monthlyResidentExpense.Status = SLIM_CONFIG.UTILITY_SERVICE_GET_CONSUMPTION_COMPLETE;
                                if (null == house.WaterMeter)
                                {
                                    house.WaterMeter = 0;
                                    _houseServices.Update(house);
                                }
                                if (houseConsummption.FromNumber != house.WaterMeter.Value)
                                {
                                    response.StatusCode = 5;
                                    response.Data = houseConsummption.HouseName;
                                    response.Msg = "Số tháng trước không chính xác: " + houseConsummption.HouseName;
                                    return Json(response, JsonRequestBehavior.AllowGet);
                                }

                                if (houseConsummption.ToNumber != 0 && (houseConsummption.ToNumber < houseConsummption.FromNumber || house.WaterMeter.Value > houseConsummption.ToNumber))
                                {
                                    response.StatusCode = 6;
                                    response.Msg = "Số ghi nước tháng mới phải lớn hơn số tháng trước: " + houseConsummption.HouseName;
                                    response.Data = houseConsummption.HouseName;
                                    return Json(response, JsonRequestBehavior.AllowGet);
                                }

                                if (houseConsummption.ToNumber == 0)
                                {
                                    monthlyResidentExpense.Status =
                                        SLIM_CONFIG.UTILITY_SERVICE_GET_CONSUMPTION_UN_COMPLETE;
                                    monthlyResidentExpense.ToNumber = 0;
                                    monthlyResidentExpense.Water = 0;
                                } // currently i don not validate FromNumber is validated in db.
                                else
                                {
                                    monthlyResidentExpense.ToNumber = houseConsummption.ToNumber;
                                    monthlyResidentExpense.Water = houseConsummption.ToNumber -
                                                                   houseConsummption.FromNumber;
                                }
                                monthlyResidentExpense.FromNumber = houseConsummption.FromNumber;
                                monthlyResidentExpense.WaterCost = totalWater;
                                monthlyResidentExpense.HouseName = houseConsummption.HouseName;
                                monthlyResidentExpense.Floor = houseConsummption.Floor;
                                monthlyResidentExpense.Block = houseConsummption.Block;
                                monthlyResidentExpense.HouseId = house.Id;
                                monthlyResidentExpense.Total = totalWater;
                                monthlyResidentExpense.DT_RowId =
                                    new StringBuilder("consump_house_").Append(house.Id).ToString();
                                monthlyResidentExpenseList.Add(monthlyResidentExpense);
                            }
                            else
                            {
                                response.StatusCode = 4;
                                response.Data = houseConsummption.HouseName;
                                response.Msg = "Không tìm thấy nhà" + houseConsummption.HouseName;
                                return Json(response, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            response.StatusCode = 2;
                            response.Data = houseConsummption.HouseName;
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                response.Data = monthlyResidentExpenseList;
            }
            catch (Exception)
            {
                response.StatusCode = -1;
                return Json(response, JsonRequestBehavior.AllowGet);
                throw;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Route("Management/ManageReceipt/CreateAutomationReceiptView")]
        public ActionResult ManageAutomationReceipt()
        {
            List<UtilServiceForHouseCat> waterSrv = null;
            List<UtilServiceForHouseCat> fixCostList = null;
            UtilServiceForHouseCat fixedCost = null;
            List<HouseCategory> utilSrvForHouseCat = _houseCategoryServices.GetAll();

            BalanceSheet curentBls = _balanceSheetService.GetCurentActivateBalanceSheet();
            if (null != curentBls)
            {
                ViewBag.curentBls = curentBls;
            }
            else
            {
                ViewBag.curentBls = null;
            }
            if (utilSrvForHouseCat.Count != 0)
            {
                waterSrv = new List<UtilServiceForHouseCat>();
                foreach (var houseCat in utilSrvForHouseCat)
                {
                    if (houseCat.UtilServiceForHouseCats == null)
                    {
                        waterSrv = null;
                        break;
                    }
                    else
                    {
                        var waterUtilSrvForHouseCat =
                            houseCat.UtilServiceForHouseCats.Where(
                                u => u.Status == SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE && u.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).ToList();
                        if (waterUtilSrvForHouseCat.Count != 0)
                        {
                            waterSrv.Add(waterUtilSrvForHouseCat.First());
                        }

                        var fixedCostUtilSrvForHouseCat =
                            houseCat.UtilServiceForHouseCats.Where(
                                u => u.Status == SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE && u.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST).ToList();
                        if (fixedCostUtilSrvForHouseCat.Count != 0)
                        {
                            fixedCost = fixedCostUtilSrvForHouseCat.First();
                        }
                    }
                }
            }

            ViewBag.waterService = waterSrv;
            ViewBag.fixedCost = fixedCost;
            return View("CreateAutomationReceipt");
        }


        [HttpPost]
        [Route("Management/ManageReceipt/CreateAutomationUtilitySevice")]
        public ActionResult CreateAutomationUtilitySevice(AutomationReceiptsTemplateModel automationReceipt)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (null != u)
            {
                if (null == automationReceipt.Title || null == automationReceipt.PublishDate)
                {
                    response.StatusCode = -1;
                    return Json(response);
                }
                // Check balance sheet is open or not
                BalanceSheet blSheet = _balanceSheetService.FindById(automationReceipt.BalanceSheetId);
                if (blSheet != null && blSheet.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSE)
                {
                    response.StatusCode = 4;
                    response.Msg = "Bảng thu chi đã đóng";
                    return Json(response);
                }
                // Allow manager adds receipt for the month that not exist in balancesheet
                // until the balance sheet is open. this receipt is added in this.
                if (blSheet == null)
                {
                    response.StatusCode = 5;
                    response.Msg = "Không tìm thấy bảng thu chi";
                    return Json(response);
                }

                if (blSheet.Receipts.Any(r => r.IsAutomation == SLIM_CONFIG.RECEIPT_TYPE_AUTOMATION))
                {
                    response.StatusCode = 2;
                    response.Msg = "Hóa đơn hàng loạt tháng này đã được tạo";
                    return Json(response);
                }

                // Set receipt common infor
                Receipt receipt = new Receipt();
                receipt.CreateDate = DateTime.Now;
                receipt.LastModified = DateTime.Now;
                receipt.PublishDate = DateTime.ParseExact(automationReceipt.PublishDate, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                receipt.Description = automationReceipt.Description;
                receipt.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED;
                receipt.Title = automationReceipt.Title;
                receipt.IsAutomation = SLIM_CONFIG.RECEIPT_TYPE_AUTOMATION;
                receipt.Type = SLIM_CONFIG.TRANSACTION_TYPE_INCOME;
                receipt.BlsId = blSheet.Id;
                receipt.ManagerId = u.Id;

                var waterCost = 0.0;
                var fixedCost = 0.0;

                List<MonthlyResidentExpenseModel> cannotCreateReceipt = new List<MonthlyResidentExpenseModel>();

                foreach (var houseRecord in automationReceipt.ResidentExpenseRecords)
                {
                    House house = _houseServices.FindById(houseRecord.HouseId);
                    if (house != null)
                    {
                        List<UtilServiceForHouseCat> waterUtilSrvForCat =
                            house.HouseCategory.UtilServiceForHouseCats.Where(
                                utilForHouse =>
                                    utilForHouse.Status == SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE &&
                                    utilForHouse.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).ToList();
                        ReceiptDetail receiptDetail = new ReceiptDetail();

                        if (waterUtilSrvForCat.Count != 0)
                        {
                            receipt.HouseId = house.Id;
                            _receiptServices.Add(receipt);

                            // Water Cost
                            List<UtilityServiceRangePrice> rangePrices =
                                waterUtilSrvForCat.First().UtilityService.UtilityServiceRangePrices.ToList();
                            waterCost = CalculateWaterUtiServiceVersion1(houseRecord.Water, rangePrices);

                            receiptDetail.Quantity = houseRecord.Water;
                            if (houseRecord.Water != 0)
                            {
                                receiptDetail.UnitPrice = waterCost / houseRecord.Water; //layvesion
                            }
                            else
                            {
                                receiptDetail.UnitPrice = 0;
                            }
                            receiptDetail.FromNumber = houseRecord.FromNumber;
                            receiptDetail.ToNumber = houseRecord.ToNumber;
                            receiptDetail.UtilityServiceId = waterUtilSrvForCat.First().UtilityService.Id;

                            // range price for house
                            receiptDetail.Total = waterCost;
                            receiptDetail.ReceiptId = receipt.Id;
                            receiptDetail.CreateDate = DateTime.Now;
                            receiptDetail.LastModified = DateTime.Now;
                            _receiptDetailServices.Add(receiptDetail);

                            // Update add transaction
                            Transaction transaction = new Transaction();
                            transaction.BlsId = blSheet.Id;
                            transaction.CreateDate = DateTime.Now;
                            transaction.LastModified = DateTime.Now;
                            transaction.ReceiptDetailId = receiptDetail.Id;
                            transaction.TotalAmount = waterCost;
                            transaction.PaidAmount = 0;
                            _transactionService.Add(transaction);

                            house.WaterMeter = houseRecord.ToNumber;
                            _houseServices.Update(house);
                        }
                        else
                        {
                            cannotCreateReceipt.Add(houseRecord);
                            response.StatusCode = 3;
                            response.Data = house.HouseCategory.Name;
                            response.Msg = new StringBuilder("Không tìm được khung giá nước cho dạng cư trú ").Append(house.HouseCategory.Name).ToString();
                        }
                        List<UtilServiceForHouseCat> fixCostForHouseCat =
                            house.HouseCategory.UtilServiceForHouseCats.Where(
                                utilForHouse =>
                                    utilForHouse.Status == SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE &&
                                    utilForHouse.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
                                .ToList();
                        if (fixCostForHouseCat.Count != 0)
                        {
                            // Fixed Cost
                            receiptDetail = new ReceiptDetail();
                            fixedCost = fixCostForHouseCat.First().UtilityService.UtilityServiceRangePrices.First().Price.Value * house.Area.Value;
                            receiptDetail.UtilityServiceId = fixCostForHouseCat.First().UtilityService.Id;
                            receiptDetail.Total = fixedCost;
                            receiptDetail.ReceiptId = receipt.Id;
                            receiptDetail.CreateDate = DateTime.Now;
                            receiptDetail.LastModified = DateTime.Now;
                            _receiptDetailServices.Add(receiptDetail);

                            // Update add transaction
                            Transaction transaction = new Transaction();
                            transaction.BlsId = blSheet.Id;
                            transaction.CreateDate = DateTime.Now;
                            transaction.LastModified = DateTime.Now;
                            transaction.ReceiptDetailId = receiptDetail.Id;
                            transaction.TotalAmount = fixedCost;
                            transaction.PaidAmount = 0;
                            _transactionService.Add(transaction);
                        }
                    }
                    else
                    {
                        cannotCreateReceipt.Add(houseRecord);
                    }
                }
            }
            return Json(response);
        }


        [HttpGet]
        [Route("Management/ManageReceipt/ViewBatchReceipt")]
        public ActionResult ViewBatchReceipt(long receiptId)
        {
            var receipts = _receiptServices.GetReceiptsByCreateDate(new DateTime(receiptId));
            if (receipts.Count != 0)
            {
                var receipt = receipts.First();
                int status = SLIM_CONFIG.RECEIPT_STATUS_PAID;
                List<Receipt> listReceiptGroupByCreateDate =
                    _receiptServices.GetReceiptsByCreateDate(receipt.CreateDate.Value);

                List<UtilServiceForHouseCat> waterSrv = new List<UtilServiceForHouseCat>();
                List<UtilServiceForHouseCat> fixCostList = null;
                UtilServiceForHouseCat fixedCost = new UtilServiceForHouseCat();

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

                List<ReceiptDetail> receiptDetails =
                    _receiptDetailServices.GetReceiptDetailByReceiptCreateDate(receipt.CreateDate.Value);
                foreach (var utilSrv in receiptDetails.Where(rd => rd.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).Select(rd => rd.UtilityService).ToList())
                {
                    waterSrv.AddRange(utilSrv.UtilServiceForHouseCats.ToList());
                }

                foreach (var utilSrv in receiptDetails.Where(rd => rd.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST).Select(rd => rd.UtilityService).ToList())
                {
                    fixedCost = utilSrv.UtilServiceForHouseCats.ToList().First();
                }


                ViewBag.waterService = waterSrv;
                ViewBag.fixedCost = fixedCost;
                return View("UpdateAutomationReceipt");
            }
            return RedirectToAction("ViewManagerOrderList");
        }


        [HttpPost]
        [Route("Management/ManageReceipt/UpdateAutomationReceipt")]
        public ActionResult UpdateAutomationReceipt(AutomationReceiptsTemplateModel automationReceipt)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (null != u)
            {
                List<Receipt> groupedByCreateDateReceipts =
                        _receiptServices.GetReceiptsByCreateDate(new DateTime(automationReceipt.ReceiptId));
                if (groupedByCreateDateReceipts != null && groupedByCreateDateReceipts.Count != 0)
                {
                    // Check balance sheet is open or not
                    BalanceSheet blSheet = _balanceSheetService.FindById(automationReceipt.BalanceSheetId);
                    if (blSheet != null && blSheet.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSE)
                    {
                        response.StatusCode = 4;
                        response.Msg = "Bảng thu chi đã đóng";
                        return Json(response);
                    }
                    // Allow manager adds receipt for the month that not exist in balancesheet
                    // until the balance sheet is open. this receipt is added in this.
                    if (blSheet == null)
                    {
                        response.StatusCode = 5;
                        response.Msg = "Không tìm thấy bảng thu chi";
                        return Json(response);
                    }

                    foreach (var receiptItem in groupedByCreateDateReceipts)
                    {
                        receiptItem.PublishDate = DateTime.ParseExact(automationReceipt.PublishDate,
                            AmsConstants.DateFormat,
                            CultureInfo.CurrentCulture);
                        receiptItem.LastModified = DateTime.Now;
                        receiptItem.Title = automationReceipt.Title;
                        receiptItem.Description = automationReceipt.Description;
                        _receiptServices.Update(receiptItem);
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


        [HttpPost]
        [Route("Management/ManageReceipt/UpdateHouseConsumption")]
        public ActionResult UpdateHouseConsumption(MonthlyReceiptModel houseConsumption)
        {
            MessageViewModels response = new MessageViewModels();

            Receipt receipt = _receiptServices.FindById(houseConsumption.ReceiptId);
            if (receipt != null)
            {
                List<ReceiptDetail> receiptDetails = receipt.ReceiptDetails.Where(rd => rd.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).ToList();
                if (receiptDetails.Count != 0)
                {
                    ReceiptDetail receiptDetail = _receiptDetailServices.FindById(receiptDetails.First().Id);

                    int quantity = houseConsumption.ToNumber - receiptDetail.FromNumber.Value;
                    receiptDetail.Quantity = quantity;
                    receiptDetail.ToNumber = houseConsumption.ToNumber;

                    List<UtilServiceForHouseCat> waterUtilSrvForCat =
                            receipt.House.HouseCategory.UtilServiceForHouseCats.Where(
                                utilForHouse =>
                                    utilForHouse.Status == SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE &&
                                    utilForHouse.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).ToList();
                    if (waterUtilSrvForCat.Count != 0)
                    {
                        List<UtilityServiceRangePrice> rangePrices =
                            waterUtilSrvForCat.First().UtilityService.UtilityServiceRangePrices.ToList();
                        receiptDetail.Total = CalculateWaterUtiServiceVersion1(quantity, rangePrices);
                        if (quantity != 0)
                        {
                            receiptDetail.UnitPrice = receiptDetail.Total / receiptDetail.Quantity;
                        }
                        else
                        {
                            receiptDetail.UnitPrice = 0;
                        }
                        var transactions = receiptDetail.Transactions.Where(tr => tr.BalanceSheet.Id == receiptDetail.Receipt.BlsId).ToList();
                        _receiptDetailServices.Update(receiptDetail);

                        House house = _houseServices.FindById(receipt.HouseId.Value);
                        house.WaterMeter = receiptDetail.ToNumber;
                        _houseServices.Update(house);

                        //Update total amount of transaction when user receipt is unpublish
                        UpdateTotalAmountInTransaction(transactions, receiptDetail.Total.Value);
                    }
                    else
                    {
                        response.StatusCode = -1;
                        response.Msg = "Không có mức giá điện.";
                    }
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy hóa đơn";
            }
            return Json(response);
        }


        [HttpPost]
        [Route("Management/ManageReceipt/DeleteHouseConsumption")]
        public ActionResult UpdateHouseConsumption(List<int> deleteItems)
        {
            MessageViewModels response = new MessageViewModels();

            if (deleteItems != null && deleteItems.Count != 0)
            {

                foreach (var itemId in deleteItems)
                {
                    Receipt receipt = null;
                    List<ReceiptDetail> receiptDetails = null;
                    receipt = _receiptServices.FindById(itemId);
                    int houseId = 0;
                    if (receipt != null && receipt.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
                    {
                        receiptDetails = receipt.ReceiptDetails.ToList();
                        receipt.ReceiptDetails = null;
                        houseId = receipt.HouseId.Value;

                        _receiptServices.Delete(receipt);
                        foreach (var detail in receiptDetails)
                        {

                            _transactionService.DeleteRangeByReceiptId(detail.Id);
                            int detailId = detail.Id;
                            _receiptDetailServices.DeleteById(detailId);
                            if (detail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                            {
                                House house = _houseServices.FindById(houseId);
                                house.WaterMeter = detail.FromNumber.Value;
                                _houseServices.Update(house);
                            }
                        }
                    }
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy hóa đơn";
            }
            return Json(response);
        }


        [HttpPost]
        [Route("Management/ManageReceipt/UpdateAutomationReceiptStatus")]
        public ActionResult UpdateAutomationReceiptStatus(long receiptId, int mode)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (null != u)
            {
                List<Receipt> receiptGroupByCreateDate = _receiptServices.GetReceiptsByCreateDate(new DateTime(receiptId));
                if (receiptGroupByCreateDate != null && receiptGroupByCreateDate.Count != 0)
                {
                    try
                    {
                        if (mode == SLIM_CONFIG.MODE_DELETE)
                        {
                            foreach (var r in receiptGroupByCreateDate)
                            {
                                List<ReceiptDetail> temp3 = r.ReceiptDetails.ToList();
                                //This is very inportant to delete this table
                                r.ReceiptDetails = null;
                                if (r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
                                {
                                    int toNumber = 0;
                                    int houseId = r.HouseId.Value;
                                    _receiptServices.Delete(r);
                                    foreach (var detail in temp3)
                                    {
                                        foreach (var trans in detail.Transactions)
                                        {
                                            _transactionService.DeleteById(trans.Id);
                                        }
                                        _receiptDetailServices.DeleteById(detail.Id);
                                        if (detail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                                        {
                                            toNumber = detail.ToNumber.Value;
                                            House house = _houseServices.FindById(houseId);
                                            house.WaterMeter = toNumber;
                                            _houseServices.Update(house);
                                        }

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

        [HttpGet]
        [Route("Management/ManageReceipt/CreateManualReceiptView")]
        public ActionResult ManageManualReceipt()
        {
            List<Block> block = _blockServices.GetAllBlocks();
            BalanceSheet curentBls = _balanceSheetService.GetCurentActivateBalanceSheet();
            if (null != curentBls)
            {
                ViewBag.curentBls = curentBls;
            }
            else
            {
                ViewBag.curentBls = null;
            }
            if (block != null && block.Count != 0)
            {
                List<House> floor = _houseServices.GetFloorHasResidentInBlock(block[0].Id);
                List<House> rooms = _houseServices.GetActiveRoomsInFloor(block[0].Id, floor[0].Floor);
                ViewBag.block = block;
                ViewBag.firstBlockFloor = floor;
                ViewBag.rooms = rooms;
            }
            ViewBag.userId = User.Identity.GetUserId();
            return View("CreateManualReceipt");
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
                        // Check balance sheet is open or not
                        BalanceSheet blSheet = _balanceSheetService.FindById(receipt.BalanceSheetId);
                        if (blSheet != null && blSheet.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSE)
                        {
                            response.StatusCode = 4;
                            response.Msg = "Bảng thu chi đã đóng";
                            return Json(response);
                        }
                        // Allow manager adds receipt for the month that not exist in balancesheet
                        // until the balance sheet is open. this receipt is added in this.
                        if (blSheet == null)
                        {
                            response.StatusCode = 5;
                            response.Msg = "Không tìm thấy bảng thu chi";
                            return Json(response);
                        }

                        // Check  waterTransaction category and fixedCost category is existed
                        List<UtilServiceCategory> allTransCategory = _utilityServiceCateoryService.GetAllMandatory();
                        UtilServiceCategory hdRequestTransCat = null;
                        foreach (var transCat in allTransCategory)
                        {
                            if (transCat.Name.Trim().Equals(AmsConstants.UtilityServiceHelpdeskRequest))
                            {
                                hdRequestTransCat = transCat;
                            }
                        }
                        if (hdRequestTransCat == null)
                        {
                            hdRequestTransCat = new UtilServiceCategory();
                            hdRequestTransCat.Name = AmsConstants.UtilityServiceHelpdeskRequest;
                            hdRequestTransCat.Status = SLIM_CONFIG.TRANS_CAT_STATUS_DENY_REMOVE;
                            _utilityServiceCateoryService.Add(hdRequestTransCat);
                        }

                        Receipt eReceipt = new Receipt();
                        eReceipt.HouseId = house.Id;
                        eReceipt.CreateDate = DateTime.Now;
                        eReceipt.LastModified = DateTime.Now;
                        eReceipt.PublishDate = DateTime.ParseExact(receipt.PublishDate, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                        eReceipt.Type = SLIM_CONFIG.TRANSACTION_TYPE_INCOME;
                        eReceipt.ManagerId = u.Id;
                        eReceipt.Description = receipt.ReceiptDesc;
                        eReceipt.Title = receipt.ReceiptTitle;
                        eReceipt.IsAutomation = SLIM_CONFIG.RECEIPT_TYPE_MANUAL;
                        eReceipt.BlsId = blSheet.Id;

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
                            item.UtilSrvCatId = hdRequestTransCat.Id;
                            item.CreateDate = DateTime.Now;
                            item.Type = SLIM_CONFIG.UTILITY_SERVICE_TYPE_HD_REQUEST;
                            _serviceChargeSevices.Add(item);

                            detail = new ReceiptDetail();
                            detail.Quantity = i.Quantity;
                            detail.ReceiptId = eReceipt.Id;

                            detail.UtilityServiceId = item.Id;
                            detail.UnitPrice = item.Price;
                            detail.Total = i.Quantity * item.Price;
                            _receiptDetailServices.Add(detail);

                            // Update add transaction
                            Transaction transaction = new Transaction();
                            transaction.BlsId = blSheet.Id;
                            transaction.CreateDate = DateTime.Now;
                            transaction.LastModified = DateTime.Now;
                            transaction.ReceiptDetailId = detail.Id;
                            transaction.TotalAmount = detail.Total;
                            transaction.PaidAmount = 0;
                            _transactionService.Add(transaction);
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
                        // Check balance sheet is open or not
                        if (receipt.BalanceSheet != null && receipt.BalanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSE)
                        {
                            response.StatusCode = 4;
                            response.Msg = "Bảng thu chi đã đóng";
                            return Json(response);
                        }

                        // Allow manager adds receipt for the month that not exist in balancesheet
                        // until the balance sheet is open. this receipt is added in this.
                        if (receipt.BalanceSheet == null)
                        {
                            response.StatusCode = 5;
                            response.Msg = "Không tìm thấy bảng thu chi";
                            return Json(response);
                        }

                        // Check  waterTransaction category and fixedCost category is existed
                        List<UtilServiceCategory> allTransCategory = _utilityServiceCateoryService.GetAllMandatory();
                        UtilServiceCategory hdRequestTransCat = null;
                        foreach (var transCat in allTransCategory)
                        {
                            if (transCat.Name.Trim().Equals(AmsConstants.UtilityServiceHelpdeskRequest))
                            {
                                hdRequestTransCat = transCat;
                            }
                        }
                        if (hdRequestTransCat == null)
                        {
                            hdRequestTransCat = new UtilServiceCategory();
                            hdRequestTransCat.Name = AmsConstants.UtilityServiceWater;
                            hdRequestTransCat.Status = SLIM_CONFIG.TRANS_CAT_STATUS_DENY_REMOVE;
                            _utilityServiceCateoryService.Add(hdRequestTransCat);
                        }

                        // Prepare date for update receipt
                        UtilityService item = null;
                        ReceiptDetail detail = null;
                        bool receiptIsDeleted = false;
                        List<int> listId = new List<int>();
                        if (null == receiptModel.ListItem)
                        {
                            List<ReceiptDetail> receiptDetails = receipt.ReceiptDetails.ToList();
                            foreach (var recDetail in receiptDetails)
                            {
                                // delete transaction
                                var transactions =
                                    recDetail.Transactions.Where(trans => trans.BlsId == receipt.BalanceSheet.Id)
                                        .ToList();
                                if (transactions.Count != 0)
                                {
                                    listId.Add(transactions.First().Id);
                                }
                            }

                            int receiptId = receipt.Id;
                            receipt.ReceiptDetails = null;
                            _receiptServices.DeleteById(receiptId);

                            receiptIsDeleted = true;

                            foreach (var transId in listId)
                            {
                                _transactionService.DeleteById(transId);

                            }
                            foreach (var recDetail in receiptDetails)
                            {
                                _receiptDetailServices.DeleteById(recDetail.Id);

                                _transactionService.DeleteRangeByReceiptId(recDetail.Id);
                                _serviceChargeSevices.DeleteById(recDetail.UtilityServiceId.Value);
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
                                        fee.LastModified = DateTime.Now;
                                        fee.UtilSrvCatId = hdRequestTransCat.Id;
                                        _serviceChargeSevices.Add(fee);

                                        ReceiptDetail newRecDetail = new ReceiptDetail();
                                        newRecDetail.Quantity = newRecDetailModel.Quantity;

                                        newRecDetail.UtilityServiceId = fee.Id;
                                        newRecDetail.UnitPrice = newRecDetailModel.UnitPrice;
                                        newRecDetail.ReceiptId = receipt.Id;
                                        newRecDetail.CreateDate = DateTime.Now;
                                        newRecDetail.LastModified = DateTime.Now;
                                        newRecDetail.Total = newRecDetailModel.Quantity * fee.Price;

                                        _receiptDetailServices.Add(newRecDetail);

                                        // Update add transaction
                                        if (receipt.BalanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN)
                                        {
                                            Transaction transaction = new Transaction();
                                            transaction.CreateDate = DateTime.Now;
                                            transaction.LastModified = DateTime.Now;
                                            transaction.ReceiptDetailId = newRecDetail.Id;
                                            transaction.TotalAmount = newRecDetail.Total;
                                            transaction.BlsId = receipt.BalanceSheet.Id;
                                            transaction.PaidAmount = 0;
                                            _transactionService.Add(transaction);
                                        }

                                        receiptModel.ListItem.RemoveAt(index);
                                        index--;
                                    }
                                    else if (recDetail.Id == newRecDetailModel.RecDetailId)
                                    {
                                        ReceiptDetail updateReceiptDetail =
                                            _receiptDetailServices.FindById(newRecDetailModel.RecDetailId);
                                        updateReceiptDetail.Quantity = newRecDetailModel.Quantity;
                                        updateReceiptDetail.UnitPrice = newRecDetailModel.UnitPrice;
                                        updateReceiptDetail.LastModified = DateTime.Now;
                                        updateReceiptDetail.Total = newRecDetailModel.UnitPrice * newRecDetailModel.Quantity;


                                        UtilityService updateServiceFee =
                                            _serviceChargeSevices.FindById(recDetail.UtilityServiceId.Value);
                                        updateServiceFee.Name = newRecDetailModel.Name;
                                        updateServiceFee.Price = newRecDetailModel.UnitPrice;
                                        updateServiceFee.LastModified = DateTime.Now;


                                        var transactions = updateReceiptDetail.Transactions.Where(tr => tr.BalanceSheet.Id == updateReceiptDetail.Receipt.BlsId).ToList();

                                        _serviceChargeSevices.Update(updateServiceFee); //AnTT
                                        _receiptDetailServices.Update(updateReceiptDetail);

                                        //Update total amount of transaction when user receipt is unpublish
                                        UpdateTotalAmountInTransaction(transactions, updateReceiptDetail.Total.Value);

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
                                    ReceiptDetail recDetail = _receiptDetailServices.FindById(deletedRecDetailId.Id);
                                    int transactionid = recDetail.Transactions.OrderBy(trans => trans.CreateDate).First().Id;

                                    _receiptDetailServices.Delete(recDetail);
                                    _serviceChargeSevices.DeleteById(deletedRecDetailId.UtilityService.Id);

                                    // delete transaction
                                    Transaction transaction = _transactionService.FindById(transactionid);
                                    _transactionService.Delete(transaction);
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
                            receipt.PublishDate = DateTime.ParseExact(receiptModel.PublishDate, AmsConstants.DateFormat,
                                CultureInfo.CurrentCulture);
                            receipt.Title = receiptModel.ReceiptTitle;
                            receipt.Description = receiptModel.ReceiptDesc;
                            receipt.LastModified = DateTime.Now;
                            _receiptServices.Update(receipt);
                            if (receipt.PublishDate.Value.Date == DateTime.Today)
                            {
                                receipt.Status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID;
                                response.Data = new {publishDate = receipt.PublishDate.Value.ToString(AmsConstants.DateFormat)};
                            }
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
                        receipt.PublishDate = DateTime.Now;
                        data = new
                        {
                            status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID,
                            publishDate = receipt.PublishDate.Value.ToString(AmsConstants.DateTimeFormat),
                        };
                        _receiptServices.Update(receipt);
                    }
                    else if (Status == SLIM_CONFIG.RECEIPT_STATUS_PAID)
                    {
                        receipt.Status = SLIM_CONFIG.RECEIPT_STATUS_PAID;
                        receipt.PaymentDate = DateTime.Now;
                        data = new
                        {
                            paymentDate = receipt.PaymentDate.Value.ToString(AmsConstants.DateTimeFormat),
                            status = SLIM_CONFIG.RECEIPT_STATUS_UNPAID
                        };
                        _receiptServices.Update(receipt);

                        // Update in balance sheet
                        if (receipt.BalanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN)
                        {
                            foreach (var receiptDetail in receipt.ReceiptDetails)
                            {
                                if (receiptDetail.Transactions.ToList().Count != 0)
                                {
                                    Transaction transaction =
                                        _transactionService.FindById(receiptDetail.Transactions.First().Id);
                                    transaction.PaidAmount = transaction.TotalAmount.Value;
                                    transaction.LastModified = DateTime.Now;
                                    _transactionService.Update(transaction);
                                }
                            }
                        }
                        else
                        {
                            BalanceSheet curBls = _balanceSheetService.GetCurentActivateBalanceSheet();
                            foreach (var receiptDetail in receipt.ReceiptDetails)
                            {
                                if (receiptDetail.Transactions.ToList().Count != 0)
                                {
                                    List<Transaction> currentBlsTransc = receiptDetail.Transactions.Where(trans => trans.BlsId == curBls.Id).ToList();
                                    if (currentBlsTransc.Count != 0)
                                    {
                                        Transaction newTransaction =
                                            _transactionService.FindById(currentBlsTransc.First().Id);
                                        newTransaction.PaidAmount = newTransaction.TotalAmount;
                                        newTransaction.LastModified = DateTime.Now;
                                        _transactionService.Update(newTransaction);

                                    }
                                    else
                                    {
                                        response.StatusCode = -1;
                                        return Json(response);
                                    }
                                }
                            }
                        }
                    }
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
        public ActionResult GetRoomAndFloor(int blockId, string floorName)
        {
            MessageViewModels response = new MessageViewModels();
            List<House> floor = _houseServices.GetFloorHasResidentInBlock(blockId);
            List<string> floorStr = new List<string>();
            List<string> roomStr = new List<string>();
            List<string> roomIdStr = new List<string>();

            if (floor != null && floor.Count > 0)
            {
                foreach (var f in floor)
                {
                    floorStr.Add(f.Floor);
                }

                List<House> rooms = null;
                if (floorName.IsNullOrWhiteSpace())
                {
                    rooms = _houseServices.GetActiveRoomsInFloor(blockId, floor[0].Floor);
                }
                else
                {
                    rooms = _houseServices.GetActiveRoomsInFloor(blockId, floorName);
                }

                if (rooms != null && rooms.Count > 0)
                {
                    foreach (var room in rooms)
                    {
                        roomStr.Add(room.HouseName);
                        roomIdStr.Add(room.Id.ToString());
                    }
                }
                response.Data = new { Floor = floorStr, Room = roomStr, RoomId = roomIdStr };
            }
            else
            {
                response.Data = new { Floor = floorStr, Room = roomStr, RoomId = roomIdStr };
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
                            .Append("Time_")
                            .Append(now.Hour)
                            .Append("_")
                            .Append(now.Minute)
                            .Append("_")
                            .Append(now.Second)
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

        [HttpGet]
        [Route("Management/ManageReceipt/ViewDownloadRecordTemplate")]
        public ActionResult ViewDownloadWaterRecordTemplate()
        {
            BalanceSheet currentActiveBls = _balanceSheetService.GetCurentActivateBalanceSheet();
            ViewBag.currentActiveBls = currentActiveBls;
            return View("DownloadWaterConsumptionTemplate");
        }

        [HttpGet]
        [Route("Management/ManageReceipt/DownloadRecordTemplate")]
        public ActionResult DownloadTemplateFile(string forMonth)
        {
            if (forMonth != null)
            {

                List<MonthlyResidentExpenseOutput> lastMonthRecordList = new List<MonthlyResidentExpenseOutput>();
                MonthlyResidentExpenseOutput record = new MonthlyResidentExpenseOutput();
                record.ForMonth = forMonth;
                lastMonthRecordList.Add(record);

                List<House> houseThisMonth = _houseServices.GetAllOwnedHousesThisMonth();
                foreach (var house in houseThisMonth)
                {
                    record = new MonthlyResidentExpenseOutput();
                    record.Block = house.Block.BlockName;
                    record.Floor = house.Floor;
                    record.HouseName = house.HouseName;
                    record.FromNumber = house.WaterMeter == null ? "0" : house.WaterMeter.ToString();
                    lastMonthRecordList.Add(record);
                }
                CsvFileDescription outputFileDescription = new CsvFileDescription
                {
                    SeparatorChar = ',', // tab delimited
                    FirstLineHasColumnNames = true, // no column names in first record
                    FileCultureName = "fr-FR" // use formats used in The Netherlands
                };
                CsvContext cc = new CsvContext();
                cc.Write(
                    lastMonthRecordList,
                    Server.MapPath(new StringBuilder(AmsConstants.CsvFilePath).Append("products2.csv").ToString()),
                    outputFileDescription);

                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(new StringBuilder().Append(AmsConstants.CsvFilePath).Append("products2.csv").ToString()).ToString());
                string fileName = "myfile.csv";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return View("DownloadWaterConsumptionTemplate");
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

        //        private double calculateWaterUtiService(double consumption, House house)
        //        {
        //            // Orderby in this case for the user in registration book will be ordered to the top
        //            List<ResidentGroupByTypeViewModel> kindOdUserInHouse = house.Users.Where(r => r.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER || r.RoleId == SLIM_CONFIG.USER_ROLE_RESIDENT)
        //                .OrderBy(r => r.ResidentType).GroupBy(u => u.ResidentType).Select(u => new ResidentGroupByTypeViewModel(u.First(), u.Count())).ToList();
        //            double totalWaterPrice = 0;
        //            if (kindOdUserInHouse.Count != 0)
        //            {
        //                if (kindOdUserInHouse.First().User.ResidentType == SLIM_CONFIG.RESIDENT_IN_REGISTRATION_BOOK)
        //                {
        //                    totalWaterPrice = calculateWaterUtiServiceVersion1(consumption, rangePrices.Where(rp => rp.Type == SLIM_CONFIG.RESIDENT_IN_REGISTRATION_BOOK).ToList(),
        //                        kindOdUserInHouse.First().Count);
        //                }
        //                else if (kindOdUserInHouse.First().User.ResidentType == SLIM_CONFIG.RESIDENT_HAS_KT3)
        //                {
        //                    totalWaterPrice = calculateWaterUtiServiceVersion1(consumption, rangePrices.Where(rp => rp.Type == SLIM_CONFIG.RESIDENT_HAS_KT3).ToList(),
        //                        kindOdUserInHouse.First().Count);
        //                }
        //                else
        //                {
        //                    totalWaterPrice = calculateWaterUtiServiceVersion1(consumption, rangePrices.Where(rp => rp.Type == SLIM_CONFIG.RESIDENT_OTHER).ToList(),
        //                        kindOdUserInHouse.First().Count);
        //                }
        //            }
        //            return totalWaterPrice;
        //        }

        private ReceiptRangePriceByHouseModel GetThisReceiptInRangePrice(List<UtilityServiceRangePrice> rangePrices, House house)
        {

            // Orderby in this case for the user in registration book will be ordered to the top
            List<ResidentGroupByTypeViewModel> kindOdUserInHouse = house.Users.Where(r => r.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER || r.RoleId == SLIM_CONFIG.USER_ROLE_RESIDENT)
                .OrderBy(r => r.ResidentType).GroupBy(u => u.ResidentType).Select(u => new ResidentGroupByTypeViewModel(u.First(), u.Count())).ToList();
            ReceiptRangePriceByHouseModel rangePriceModel = new ReceiptRangePriceByHouseModel();
            if (kindOdUserInHouse.Count != 0)
            {
                if (kindOdUserInHouse.First().User.ResidentType == SLIM_CONFIG.RESIDENT_IN_REGISTRATION_BOOK)
                {
                    rangePriceModel.RangePrices =
                        rangePrices.Where(rp => rp.Type == SLIM_CONFIG.RESIDENT_IN_REGISTRATION_BOOK).ToList();
                    rangePriceModel.Count = kindOdUserInHouse.First().Count;
                }
                else if (kindOdUserInHouse.First().User.ResidentType == SLIM_CONFIG.RESIDENT_HAS_KT3)
                {
                    rangePriceModel.RangePrices =
                        rangePrices.Where(rp => rp.Type == SLIM_CONFIG.RESIDENT_HAS_KT3).ToList();
                    rangePriceModel.Count = kindOdUserInHouse.First().Count;
                }
                else
                {
                    rangePriceModel.RangePrices =
                        rangePrices.Where(rp => rp.Type == SLIM_CONFIG.RESIDENT_OTHER).ToList();
                    rangePriceModel.Count = kindOdUserInHouse.First().Count;
                }
                return rangePriceModel;
            }
            return null;
        }

        public static double CalculateWaterUtiServiceVersion1(double consumption, List<UtilityServiceRangePrice> rangePrices)
        {
            double total = 0;
            double previous = 0;
            double fromAmount = 0;
            double toAmount = 0;
            foreach (UtilityServiceRangePrice u in rangePrices)
            {
                //                fromAmount = u.FromAmount.Value * numberOfResident;
                //                toAmount = u.ToAmount.Value * numberOfResident;
                fromAmount = u.FromAmount.Value;
                toAmount = u.ToAmount.Value;

                double calculatingPart = 0;
                if (consumption >= toAmount)
                {
                    calculatingPart = toAmount - previous;
                    previous = toAmount;
                }
                else if (consumption >= fromAmount && consumption < toAmount)
                {
                    calculatingPart = consumption - previous;
                }
                total += calculatingPart * u.Price.Value;
            }
            return total;
        }

        private UtilityServicesGroupByTypeModel GetAllAparmentUtilitySevices()
        {
            List<UtilityService> waterSevices = _utilityServiceServices.GetServicesByType(SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER);
            List<UtilityService> fixedCostSevices = _utilityServiceServices.GetServicesByType(SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST);

            UtilityServicesGroupByTypeModel utilServiceGroup = new UtilityServicesGroupByTypeModel();
            utilServiceGroup.WaterSevices = waterSevices;
            utilServiceGroup.FixedCostSevices = fixedCostSevices;
            return utilServiceGroup;
        }

        public void UpdateTotalAmountInTransaction(List<Transaction> listTrans, double totalAmount)
        {
            //Update total amount of transaction when user receipt is unpublish
            if (listTrans.ToList().Count != 0)
            {
                var eTrans = _transactionService.FindById(listTrans.First().Id);
                eTrans.TotalAmount = totalAmount;
                eTrans.PaidAmount = 0;
                eTrans.LastModified = DateTime.Now;
                _transactionService.Update(eTrans);
            }
        }
    }
}
