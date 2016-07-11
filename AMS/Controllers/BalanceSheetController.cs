using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AMS.Constant;
using AMS.Filter;
using AMS.Models;
using AMS.Service;
using Antlr.Runtime.Tree;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace AMS.Controllers
{
    public class BalanceSheetController : Controller
    {
        readonly UserServices _userServices = new UserServices();
        readonly BalanceSheetService _balanceSheetService = new BalanceSheetService();
        readonly UtilityServiceCateoryService _transCategoryServiceCateoryService = new UtilityServiceCateoryService();
        readonly TransactionService _transactionService = new TransactionService();
        readonly ReceiptServices _receiptService = new ReceiptServices();
        readonly ReceiptDetailServices _receiptDetailServices = new ReceiptDetailServices();
        private UtilityServiceCateoryService _utilityServiceCateoryService = new UtilityServiceCateoryService();
        private UtilityServiceServices _utilityServiceServices = new UtilityServiceServices();

        [HttpGet]
        [Route("Management/BalanceSheet/View")]
        public ActionResult ViewBalanceSheet(string month)
        {
            DateTime requestMonth = DateTime.Today;
            if (!month.IsNullOrWhiteSpace())
            {
                requestMonth = DateTime.ParseExact(month, AmsConstants.MonthYearFormat, CultureInfo.CurrentCulture);
            }

            BalanceSheet thisMonthBS = _balanceSheetService.GetBalanceSheetForMonth(requestMonth);
            if (null == thisMonthBS)
            {
                ViewBag.balanceSheetIsCreated = false;
                return View("BalanceSheet");
            }

            BalanceSheetProcessingModel processedBls = ProcessingBalanceSheetVersion2(thisMonthBS, true);

            var incomeTransactionsJson = new JavaScriptSerializer().Serialize(processedBls.IncomeList);
            var expenseTransactionsJson = new JavaScriptSerializer().Serialize(processedBls.ExpenseList);

            ViewBag.thisMonth = thisMonthBS.StartDate.Value.Date.ToString(AmsConstants.MonthYearFormat);
            ViewBag.fromDate = thisMonthBS.StartDate.Value.ToString(AmsConstants.DateFormat);
            ViewBag.closingDate = null;
            if (thisMonthBS.ClosingDate != null)
            {
                ViewBag.closingDate = thisMonthBS.ClosingDate.Value.ToString(AmsConstants.DateFormat);
            }
            ViewBag.lastUpdate = thisMonthBS.LastModified.Value.ToString(AmsConstants.DateTimeFormat);
            ViewBag.description = thisMonthBS.Description;
            ViewBag.balanceSheetId = thisMonthBS.Id;
            ViewBag.status = thisMonthBS.Status;

            ViewBag.balanceSheet = processedBls;
            ViewBag.incomeTransactionsJson = incomeTransactionsJson;
            ViewBag.expenseTransactionsJson = expenseTransactionsJson;

            return View("BalanceSheet");
        }



        [HttpGet]
        [AdminAuthorize]
        [Route("Management/BalanceSheet/ManageTransactionCatView")]
        public ActionResult ViewManageTransactionCat()
        {
            return View("ManageTransactionCategory");
        }

        [HttpGet]
        [ManagerAuthorize]
        [Route("Management/BalanceSheet/ManageBalanceSheetView")]
        public ActionResult ViewManageBalanceSheetView()
        {
            bool thereIsNoBls = false;
            if (_balanceSheetService.hasBalanceSheet())
            {
                thereIsNoBls = true;
            }
            ViewBag.thereIsNoBls = thereIsNoBls;
            return View("ManageBalanceSheet");
        }

        [HttpGet]
        [Route("Management/BalanceSheet/GetListBalanceSheet")]
        public ActionResult GetListBalanceSheetList()
        {
            List<BalanceSheet> balanceSheets = _balanceSheetService.GetAllBalanceSheets();
            List<BalanceSheetModel> bsModelList = new List<BalanceSheetModel>();
            BalanceSheetModel bsModel = null;
            foreach (var bs in balanceSheets)
            {
                BalanceSheetProcessingModel processedBls = GetTransactionGroupByUtilSrvCat(bs, true);
                bsModel = new BalanceSheetModel();
                bsModel.TotalExpense = processedBls.ForecastExpense;
                bsModel.TotalIncome = processedBls.ForecastIncome;
                bsModel.TotalExpenseInCash = processedBls.ActualExpense;
                bsModel.TotalIncomeInCash = processedBls.ActualIncome;
                bsModel.CreateDate = bs.CreateDate.Value.ToString(AmsConstants.DateFormat);
                bsModel.Creator = bs.User.Fullname;
                bsModel.Name = bs.Title;
                bsModel.Id = bs.Id;
                bsModel.Status = bs.Status.Value;
                bsModel.StartDate = bs.CreateDate.Value.ToString(AmsConstants.MonthYearFormat);
                bsModel.DT_RowId = new StringBuilder("bls_").Append(bs.Id).ToString();
                bsModelList.Add(bsModel);
            }
            return Json(bsModelList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/BalanceSheet/OpenMonthlyBalanceSheet")]
        public ActionResult OpenMonthlyBalanceSheet(BalanceSheetModel bls)
        {
            MessageViewModels response = new MessageViewModels();
            if (null != bls.StartDate)
            {
                DateTime forMonth = DateTime.ParseExact(bls.StartDate, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                if (!_balanceSheetService.hasBalanceSheet())
                {
                    BalanceSheet eBls = new BalanceSheet();
                    eBls.ManagerId = Int32.Parse(User.Identity.GetUserId());
                    eBls.CreateDate = DateTime.Now;
                    eBls.LastModified = DateTime.Now;
                    eBls.RedundancyStartMonth = bls.RedundancyStartMonth;
                    eBls.StartDate = forMonth;
                    eBls.Title = bls.Name;
                    eBls.Status = SLIM_CONFIG.BALANCE_SHEET_OPEN;
                    _balanceSheetService.Add(eBls);


                }//reject if balance sheet has openend already
                else
                {
                    response.StatusCode = -1;
                    response.Msg = "Hóa đơn tháng " + bls.StartDate + " đã được tạo.";
                } // return balance sheet is opened
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy hóa đơn";
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/BalanceSheet/CloseBalanceSheet")]
        public ActionResult OpenMonthlyBalanceSheet(CloseBalanceSheetModel blsheetModel)
        {
            MessageViewModels response = new MessageViewModels();
            BalanceSheet bls = _balanceSheetService.FindById(blsheetModel.ThisMonthId);
            if (bls != null && bls.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN)
            {
                BalanceSheetProcessingModel processingBls = GetTransactionGroupByUtilSrvCat(bls, false);
                bls.ClosingDate = DateTime.Today.Date;
                bls.RedundancyEndMonth = processingBls.RedudancyEndMonth;
                bls.RedundancyStartMonth = processingBls.RedudancyStartMonth;
                bls.TotalExpense = processingBls.ForecastExpense;
                bls.TotalIncome = processingBls.ForecastIncome;
                bls.TotalExpenseInCash = processingBls.ActualExpense;
                bls.TotalIncomeInCash = processingBls.ActualIncome;
                bls.LastModified = DateTime.Now;
                bls.Status = SLIM_CONFIG.BALANCE_SHEET_CLOSE;

                _balanceSheetService.Update(bls);

                BalanceSheet newBls = new BalanceSheet();
                newBls.ManagerId = Int32.Parse(User.Identity.GetUserId());
                newBls.StartDate = DateTime.Today.Date.AddDays(1);
                newBls.LastModified = DateTime.Now;
                newBls.RedundancyStartMonth = bls.RedundancyEndMonth;
                newBls.CreateDate = DateTime.Now;
                newBls.Title = blsheetModel.NextMonthTitle;
                newBls.Status = SLIM_CONFIG.BALANCE_SHEET_OPEN;
                _balanceSheetService.Add(newBls);

                // if receipt detail is not paid total in the old trasaction it will be create a new one in this month
                var unpaidTransaction =
                    bls.Transactions.Where(
                        trans => trans.ReceiptDetail.Transactions.Sum(allTrans => allTrans.PaidAmount) < trans.ReceiptDetail.Total).ToList();
                foreach (var trans in unpaidTransaction)
                {
                    Transaction newTrans = new Transaction();
                    newTrans.ReceiptDetailId = trans.ReceiptDetailId;
                    newTrans.PaidAmount = 0;
                    double totalInPast = trans.ReceiptDetail.Transactions.Sum(allTrans => allTrans.PaidAmount).Value;
                    newTrans.TotalAmount = trans.ReceiptDetail.Total - totalInPast;
                    newTrans.CreateDate = DateTime.Now;
                    newTrans.LastModified = DateTime.Now;
                    newTrans.BlsId = newBls.Id;
                    _transactionService.Add(newTrans);
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
        [Route("Management/BalanceSheet/AddTransactionType")]
        public ActionResult AddTransactionType(TransCategoryModel transCategory)
        {
            MessageViewModels response = new MessageViewModels();
            UtilServiceCategory utilSrvCat = new UtilServiceCategory();
            utilSrvCat.Name = transCategory.TransCategoryName;
            utilSrvCat.Status = SLIM_CONFIG.TRANS_CAT_STATUS_ENABLE;
            utilSrvCat.CreateDate = DateTime.Now;
            utilSrvCat.LastModified = DateTime.Now;
            _transCategoryServiceCateoryService.Add(utilSrvCat);
            return Json(response);
        }

        [HttpPost]
        [Route("Management/BalanceSheet/UpdateTransactionType")]
        public ActionResult UpdateTransactionType(TransCategoryModel transCategory)
        {
            MessageViewModels response = new MessageViewModels();

            UtilServiceCategory transItemCatE = _transCategoryServiceCateoryService.FindById(transCategory.TransCategoryId);
            if (null != transItemCatE)
            {
                transItemCatE.Name = transCategory.TransCategoryName;
                transItemCatE.LastModified = DateTime.Now;
                _transCategoryServiceCateoryService.Update(transItemCatE);
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = AmsConstants.MsgUserNotFound;
            }


            return Json(response);
        }

        [HttpPost]
        [Route("Management/BalanceSheet/AddTransactionItem")]
        public ActionResult AddTransactionItem(TransactionModel transaction)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {
                UtilServiceCategory utilSrvCat = _transCategoryServiceCateoryService.FindById(transaction.UtilSrvCatId);
                if (null != utilSrvCat)
                {
                    BalanceSheet balanceSheet = _balanceSheetService.FindById(transaction.BalanceSheetId);

                    if (null == balanceSheet)
                    {
                        response.StatusCode = 2;
                        response.Msg = "Bảng quản lý thu chi đã đóng";
                        return Json(response);
                    }
                    if (balanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSE)
                    {
                        response.StatusCode = 3;
                        response.Msg = "Bảng quản lý thu chi không tồn tại";
                        return Json(response);
                    }


                    UtilityService utiSrv = new UtilityService();
                    utiSrv.Name = transaction.TransTitle;
                    utiSrv.CreateDate = DateTime.Now;
                    utiSrv.LastModified = DateTime.Now;
                    utiSrv.Price = transaction.TransTotalAmount;
                    utiSrv.UtilSrvCatId = utilSrvCat.Id;
                    utiSrv.Type = SLIM_CONFIG.UTILITY_SERVICE_TYPE_NOT_FOR_RESIDENT;
                    _utilityServiceServices.Add(utiSrv);

                    Receipt receipt = new Receipt();
                    receipt.CreateDate = DateTime.Now;
                    receipt.LastModified = DateTime.Now;
                    receipt.Description = transaction.TransDesc;
                    receipt.ManagerId = u.Id;
                    receipt.Title = transaction.TransTitle;
                    receipt.BlsId = balanceSheet.Id;
                    receipt.Type = transaction.TransType;
                    _receiptService.Add(receipt);

                    ReceiptDetail receiptDetail = new ReceiptDetail();
                    receiptDetail.CreateDate = DateTime.Now;
                    receiptDetail.LastModified = DateTime.Now;
                    receiptDetail.Quantity = 1;
                    receiptDetail.UnitPrice = transaction.TransTotalAmount;
                    receiptDetail.Total = transaction.TransTotalAmount;
                    receiptDetail.ReceiptId = receipt.Id;
                    receiptDetail.UtilityServiceId = utiSrv.Id;
                    _receiptDetailServices.Add(receiptDetail);

                    Transaction eTransaction = new Transaction();
                    eTransaction.BlsId = balanceSheet.Id;
                    eTransaction.CreateDate = DateTime.Now;
                    eTransaction.LastModified = DateTime.Now;
                    eTransaction.TotalAmount = transaction.TransTotalAmount;
                    eTransaction.PaidAmount = transaction.TransPaidAmount;
                    eTransaction.BlsId = balanceSheet.Id;
                    eTransaction.ReceiptDetailId = receiptDetail.Id;
                    _transactionService.Add(eTransaction);

                    balanceSheet.LastModified = DateTime.Now;
                    _balanceSheetService.Update(balanceSheet);
                }
                else
                {
                    response.StatusCode = -1;
                    response.Msg = "Tìm thấy loại chuyển nhượng";
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = AmsConstants.MsgUserNotFound;
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/BalanceSheet/UpdateTransactionItem")]
        public ActionResult UpdateTransactionItem(TransactionModel transModel)
        {
            MessageViewModels response = new MessageViewModels();
            Receipt receipt = _receiptService.FindById(transModel.ReceiptId);
            if (null == receipt || receipt.ReceiptDetails.Count == 0)
            {
                response.StatusCode = 2;
                response.Msg = "Không tìm thấy giao dịch";
                return Json(response);
            }

            //                if (receipt.BalanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSE)
            //                {
            //                    response.StatusCode = 3;
            //                    response.Msg = "Khai báo thu chi đã đóng";
            //                    return Json(response);
            //                }
            UtilServiceCategory utilSrvCat = _transCategoryServiceCateoryService.FindById(transModel.UtilSrvCatId);
            if (null != utilSrvCat)
            {

                //                    List<ReceiptDetail> receiptDetails = receipt.ReceiptDetails.ToList();
                if (receipt.BalanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN)
                {
                    foreach (var receiptDetail in receipt.ReceiptDetails.ToList())
                    {
                        var utilSrv = _utilityServiceServices.FindById(receiptDetail.UtilityServiceId.Value);
                        if (utilSrv.Price != transModel.TransTotalAmount || utilSrv.Name != transModel.TransTitle ||
                            utilSrv.UtilSrvCatId != transModel.UtilSrvCatId)
                        {
                            utilSrv.LastModified = DateTime.Now;
                            utilSrv.Price = transModel.TransTotalAmount;
                            utilSrv.Name = transModel.TransTitle;
                            utilSrv.UtilSrvCatId = transModel.UtilSrvCatId;
                            _utilityServiceServices.Update(utilSrv);
                        }

                        if (receiptDetail.Total != transModel.TransTotalAmount)
                        {
                            var rd = _receiptDetailServices.FindById(receiptDetail.Id);
                            rd.LastModified = DateTime.Now;
                            rd.Total = transModel.TransTotalAmount;
                            rd.UnitPrice = transModel.TransTotalAmount;
                            _receiptDetailServices.Update(rd);
                        }
                        List<Transaction> transList =
                            receiptDetail.Transactions.Where(t => t.BlsId == receipt.BlsId).ToList();
                        foreach (var tran in transList)
                        {
                            Transaction eTrans = _transactionService.FindById(tran.Id);
                            eTrans.TotalAmount = transModel.TransTotalAmount;
                            eTrans.PaidAmount = transModel.TransPaidAmount;
                            eTrans.LastModified = DateTime.Now;
                            _transactionService.Update(eTrans);
                        }
                    }

                    receipt.LastModified = DateTime.Now;
                    receipt.Title = transModel.TransTitle;
                    receipt.Description = transModel.TransDesc;
                    _receiptService.Update(receipt);

                    BalanceSheet bls = _balanceSheetService.FindById(receipt.BalanceSheet.Id);
                    bls.LastModified = DateTime.Now;
                    _balanceSheetService.Update(bls);
                }
                else
                {
                    BalanceSheet curBls = _balanceSheetService.GetCurentActivateBalanceSheet();
                    foreach (var receiptDetail in receipt.ReceiptDetails.ToList())
                    {
                        double totalPaidAmountInPreviousBls = 0;
                        foreach (var oldTrans in receiptDetail.Transactions.ToList())
                        {
                            if (oldTrans.BlsId != curBls.Id)
                            {
                                totalPaidAmountInPreviousBls += oldTrans.PaidAmount.Value;
                            }
                        }
                        List<Transaction> currentBlsTransc = receiptDetail.Transactions.Where(trans => trans.BlsId == curBls.Id).ToList();

                        if (currentBlsTransc.Count != 0)
                        {
                            if ((transModel.TransPaidAmount + totalPaidAmountInPreviousBls) <= receiptDetail.Total &&
                                (transModel.TransPaidAmount + totalPaidAmountInPreviousBls) > totalPaidAmountInPreviousBls)
                            {
                                Transaction newTransaction =
                                    _transactionService.FindById(currentBlsTransc.First().Id);
                                newTransaction.PaidAmount = transModel.TransPaidAmount;
                                newTransaction.LastModified = DateTime.Now;
                                _transactionService.Update(newTransaction);
                            }
                            else
                            {
                                response.StatusCode = -1;
                                return Json(response);
                            }
                        }
                        else
                        {
                            response.StatusCode = -1;
                            return Json(response);
                        }
                    }

                    receipt.LastModified = DateTime.Now;
                    receipt.Title = transModel.TransTitle;
                    receipt.Description = transModel.TransDesc;
                    _receiptService.Update(receipt);

                    BalanceSheet bls = _balanceSheetService.FindById(receipt.BalanceSheet.Id);
                    bls.LastModified = DateTime.Now;
                    _balanceSheetService.Update(bls);
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không Tìm thấy loại chuyển nhượng";
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/BalanceSheet/DeleteTransactions")]
        public ActionResult DeleteTransactions(List<int> listTransaction, int balanceSheetId)
        {
            MessageViewModels response = new MessageViewModels();
            Transaction currentTransaction = null;
            ReceiptDetail receiptDetail = null;
            int receiptId = 0;
            int receiptDetailId = 0;
            int blsId = 0;
            BalanceSheet curBalanceSheet = _balanceSheetService.FindById(balanceSheetId);
            if (curBalanceSheet == null || listTransaction == null)
            {
                response.StatusCode = -1;
                return Json(response);
            }
            foreach (var transactionId in listTransaction)
            {
                currentTransaction = _transactionService.FindById(transactionId);

                if (currentTransaction != null && curBalanceSheet.Id == currentTransaction.BalanceSheet.Id && currentTransaction.BalanceSheet.Status != SLIM_CONFIG.BALANCE_SHEET_CLOSE)
                {
                    blsId = currentTransaction.BlsId.Value;
                    receiptId = currentTransaction.ReceiptDetail.ReceiptId.Value;
                    receiptDetailId = currentTransaction.ReceiptDetailId.Value;

                    _transactionService.DeleteById(currentTransaction.Id);
                    _receiptDetailServices.DeleteById(receiptDetailId);
                    _receiptService.DeleteById(receiptId);
                }
            }

            if (blsId != 0)
            {
                BalanceSheet bls = _balanceSheetService.FindById(blsId);
                bls.LastModified = DateTime.Now;
                _balanceSheetService.Update(bls);
            }
            return Json(response);
        }

        [HttpGet]
        [Route("Management/BalanceSheet/GetAllTransactionType")]
        public ActionResult GetTransactionType(int type)
        {
            MessageViewModels response = new MessageViewModels();

            List<UtilServiceCategory> list = _transCategoryServiceCateoryService.GetAllEnable();
            List<ShortTransCategoryModel> modelList = new List<ShortTransCategoryModel>();
            ShortTransCategoryModel m = null;
            foreach (var i in list)
            {
                m = new ShortTransCategoryModel();
                m.Name = i.Name;
                m.Id = i.Id;
                modelList.Add(m);
            }
            response.Data = modelList;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/BalanceSheet/GetAllTransactionTypeFull")]
        public ActionResult GetTransactionTypeFull()
        {
            List<UtilServiceCategory> list = _transCategoryServiceCateoryService.GetAllEnable();
            List<TransCategoryModel> modelList = new List<TransCategoryModel>();
            TransCategoryModel m = null;
            foreach (var i in list)
            {
                m = new TransCategoryModel();
                m.DT_RowId = new StringBuilder("trans_cat_").Append(i.Id).ToString();
                m.TransCategoryId = i.Id;
                m.TransCategoryName = i.Name;
                modelList.Add(m);
            }
            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/BalanceSheet/GetIncomeTransItems")]
        public ActionResult GetIncomeTransItems(int blsId)
        {
            MessageViewModels response = new MessageViewModels();
            BalanceSheet bls = _balanceSheetService.FindById(blsId);
            if (bls != null)
            {
                //                BalanceSheetProcessingModel processedBls = ProcessingBalanceSheet(bls, false);
                BalanceSheetProcessingModel processedBls = GetTransactionGroupByUtilSrvCat(bls, false);
                Object obj = new
                {
                    data = processedBls.TotalTransactionList,
                    forecastIncome = processedBls.ForecastIncome,
                    actualIncome = processedBls.ActualIncome,
                    forecastExpense = processedBls.ForecastExpense,
                    actualExpense = processedBls.ActualExpense,
                    redudancyEndMonth = processedBls.RedudancyEndMonth,
                    redudancyStartMonth = processedBls.RedudancyStartMonth,
                    lastUpdate = processedBls.LastModified,
                    balanceSheetId = bls.Id
                };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = AmsConstants.MsgUserNotFound;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Route("Management/BalanceSheet/BalanceSheetDetailView")]
        public ActionResult ViewManageIncome(int blsId)
        {
            BalanceSheet blSheet = _balanceSheetService.FindById(blsId);

            if (blSheet != null)
            {
                ViewBag.bls = blSheet;
            }

            return View("ManageBalanceSheetDetail");
        }

        [HttpGet]
        [Route("Management/BalanceSheet/GetTransItemDetail")]
        public ActionResult GetTransItemDetail(int transId, int blsId)
        {
            MessageViewModels response = new MessageViewModels();
            //            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            //            if (u != null)
            //            {
            Transaction currentTransaction = _transactionService.FindById(transId);
            BalanceSheet thisBalanceSheet = _balanceSheetService.FindById(blsId);
            ReceiptDetail receiptDetail = null;
            Transaction currentTran = null;
            if (null != currentTransaction && thisBalanceSheet != null)
            {
                TransactionModel model = new TransactionModel();

                double totalPaidAmount = 0;
                double totalPaidThisMonth = 0;
                double totalAmountMonth = 0;

                List<Transaction> currentTrans = null;


                if (currentTransaction.ReceiptDetail.Receipt.House == null)
                {
                    totalPaidThisMonth = currentTransaction.PaidAmount.Value;
                    totalAmountMonth = currentTransaction.TotalAmount.Value;
                    currentTran = currentTransaction;
                }
                else
                {
                    var groupByCatTrans = new List<UtilSrvCatTransaction>();
                    List<Transaction> listTransaction = thisBalanceSheet.Transactions
                        .Where(trans => trans.ReceiptDetail.UtilityService.UtilSrvCatId == currentTransaction.ReceiptDetail.UtilityService.UtilSrvCatId).ToList();
                    groupByCatTrans = listTransaction
                            .GroupBy(trans => new
                            {
                                trans.ReceiptDetail.UtilityService.UtilSrvCatId
                            })
                            .Select(trans => new UtilSrvCatTransaction(trans.First(),
                                trans.Sum(t => t.TotalAmount).Value,
                                trans.Sum(t => t.PaidAmount).Value)).ToList();

                    currentTran = groupByCatTrans.First().Transaction;
                    totalAmountMonth = groupByCatTrans.First().TotalAmount;
                    totalPaidThisMonth = groupByCatTrans.First().TotalPaid;
                }

                receiptDetail = currentTran.ReceiptDetail;
                currentTran.TotalAmount = totalAmountMonth;
                currentTran.PaidAmount = totalPaidThisMonth;

                model.TransPaidAmount = totalPaidAmount;
                model.TransTotalAmount = currentTran.TotalAmount.Value;
                model.TransPaidInMonthAmount = currentTran.PaidAmount.Value;

                model.UtilSrvCatName = receiptDetail.UtilityService.UtilServiceCategory.Name;
                model.UtilSrvCatId = receiptDetail.UtilityService.UtilServiceCategory.Id;
                model.TransDesc = currentTransaction.ReceiptDetail.Receipt.Description;
                model.TransStartDate =
                    currentTransaction.ReceiptDetail.Receipt.BalanceSheet.CreateDate.Value.ToString(AmsConstants.MonthYearFormat);
                model.TransType = currentTransaction.ReceiptDetail.Receipt.Type.Value;

                model.TransTitle = receiptDetail.Receipt.Title;

                if (currentTran.ReceiptDetail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                {
                    model.TransTitle = new StringBuilder("Thu tiền nước sinh hoạt [ ").Append(model.TransStartDate).Append(" ]").ToString();
                }
                else if (currentTran.ReceiptDetail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
                {
                    model.TransTitle = new StringBuilder("Thu tiền phí sinh hoạt [ ").Append(model.TransStartDate).Append(" ]").ToString();
                }
                else if (currentTran.ReceiptDetail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_HD_REQUEST)
                {
                    model.TransTitle = new StringBuilder("Thu tiền dịch vụ sửa chữa [ ").Append(model.TransStartDate).Append(" ]").ToString();
                }

                model.ReceiptId = currentTransaction.ReceiptDetail.Receipt.Id;
                model.BalanceSheetId = currentTransaction.ReceiptDetail.Receipt.BalanceSheet.Id;
                response.Data = model;
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = AmsConstants.MsgUserNotFound;
            }
            //            }
            //            else
            //            {
            //                response.StatusCode = -1;
            //                response.Msg = AmsConstants.MsgUserNotFound;
            //            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/BalanceSheet/GetDetailTransCat")]
        public ActionResult GetDetailTransCat(int transCat)
        {
            MessageViewModels response = new MessageViewModels();
            UtilServiceCategory transaction = _transCategoryServiceCateoryService.FindById(transCat);
            if (null != transaction)
            {
                TransCategoryModel model = new TransCategoryModel();
                model.TransCategoryId = transaction.Id;
                model.TransCategoryName = transaction.Name;
                response.Data = model;
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = AmsConstants.MsgUserNotFound;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/BalanceSheet/DeleteTransCat")]
        public ActionResult DeleteTransactionCategory(List<int> transCatDeletedList)
        {
            MessageViewModels response = new MessageViewModels();
            //            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            //            if (u != null)
            //            {
            if (transCatDeletedList != null)
            {
                foreach (var transId in transCatDeletedList)
                {
                    UtilServiceCategory transactionCat = _transCategoryServiceCateoryService.FindById(transId);
                    if (transactionCat != null)
                    {
                        _transCategoryServiceCateoryService.Delete(transactionCat);
                    }
                }
            }
            else
            {
                response.StatusCode = -1;
            }
            //            }
            //            else
            //            {
            //                response.StatusCode = -1;
            //                response.Msg = AmsConstants.MsgUserNotFound;
            //            }
            return Json(response);
        }

        //        private BalanceSheetProcessingModel ProcessingBalanceSheet(BalanceSheet blSheet, bool separatedTransList)
        //        {
        //            double forecastIncome = 0;
        //            double actualIncome = 0;
        //            double actualExpense = 0;
        //            double forecastExpense = 0;
        //            double redudancyStartMonth = 0;
        //            double redudancyEndMonth = 0;
        //            List<TransactionModel> totalTransactionList = new List<TransactionModel>();
        //            List<TransactionModel> incomeList = new List<TransactionModel>();
        //            List<TransactionModel> expenseList = new List<TransactionModel>();
        //            BalanceSheetProcessingModel processedBls = new BalanceSheetProcessingModel();
        //
        //            if (blSheet != null)
        //            {
        //
        //                // Get income from receipt
        //                List<Receipt> publishedReceipt = null;
        //                if (blSheet.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN)
        //                {
        //                    publishedReceipt =
        //                        _receiptService.GetReceiptInMonthFromOpeningToToday(blSheet)
        //                            .Where(r => r.Status != SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
        //                            .ToList();
        //                }// Get new receipts are created from the date bls is open to today
        //                else
        //                {
        //                    publishedReceipt =
        //                        _receiptService.GetReceiptInMonthWhileBalanceSheetOpen(blSheet)
        //                            .Where(r => r.Status != SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
        //                            .ToList();
        //                }// Get new receipts are created from the date bls is open to closing date
        //
        //                List<Receipt> publishedReceiptGroupByType = publishedReceipt.GroupBy(r => r.Type).Select(r => r.First()).ToList();
        //
        //                List<AmountGroupByTransCategory> totalAmountPubReceiptGroupByType = new List<AmountGroupByTransCategory>();
        //                foreach (var recType in publishedReceiptGroupByType)
        //                {
        //                    AmountGroupByTransCategory receiptAmountObj = calculateTotalReceipt(recType, publishedReceipt);
        //                    totalAmountPubReceiptGroupByType.Add(receiptAmountObj);
        //                    forecastIncome += receiptAmountObj.TotalAmount;
        //                }
        //
        //                List<Receipt> paidReceipt = null;
        //                if (blSheet.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSE)
        //                {
        //                    paidReceipt = _receiptService.GetPaidReceiptWhileBalanceSheetOpen(blSheet).ToList();
        //                }
        //                else
        //                {
        //                    paidReceipt = _receiptService.GetPaidReceiptOfBalanceSheetFromOpeningToToday(blSheet).ToList();
        //                }
        //
        //                List<Receipt> paidReceiptGroupByType = paidReceipt.GroupBy(r => r.Type).Select(r => r.First()).ToList();
        //
        //                foreach (var recType in paidReceiptGroupByType)
        //                {
        //                    // Find this category in published receipt to update paid amount
        //                    AmountGroupByTransCategory receiptAmountObj = calculateTotalReceipt(recType, paidReceipt);
        //                    //                        totalPaidIncome += receiptAmountObj.TotalAmount;
        //
        //                    AmountGroupByTransCategory pubReceipt = null;
        //                    for (int i = 0; i < totalAmountPubReceiptGroupByType.Count; i++)
        //                    {
        //                        pubReceipt = totalAmountPubReceiptGroupByType[i];
        //                        if (receiptAmountObj.Id == pubReceipt.Id)
        //                        {
        //                            pubReceipt.PaidAmount = receiptAmountObj.TotalAmount;
        //                            pubReceipt.UnpaidAmount = pubReceipt.TotalAmount - receiptAmountObj.TotalAmount;
        //                            totalAmountPubReceiptGroupByType[i] = pubReceipt;
        //                            actualIncome += pubReceipt.PaidAmount;
        //                            break;
        //                        }
        //                    }
        //                }
        //
        //                
        //
        //                TransactionModel m = null;
        //                List<Transaction> list = blSheet.Transactions.ToList();
        //                foreach (var i in list)
        //                {
        //                    m = new TransactionModel();
        //                    m.UtilSrvCatName = i.TransactionCategory.Name;
        //                    m.ReceiptId = i.Id;
        //                    m.DT_RowId = new StringBuilder("trans_").Append(i.Id).ToString();
        //                    m.TransStartDate = i.BalanceSheet.ForMonth.Value.ToString("MM-yyyy");
        //                    m.TransTitle = i.Name;
        //                    m.TransTotalAmount = i.TotalAmount.Value;
        //                    m.TransPaidAmount = i.PaidAmount.Value;
        //                    m.TransCreateDate = i.CreateDate.Value.ToString(AmsConstants.DateFormat);
        //
        //                    if (i.BalanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN)
        //                    {
        //                        m.TransEditable = 1;
        //                    }
        //                    else
        //                    {
        //                        m.TransEditable = -1;
        //                    }
        //                    m.TransType = i.Type.Value;
        //                    if (i.Type == SLIM_CONFIG.TRANSACTION_TYPE_EXPENSE)
        //                    {
        //                        actualExpense += i.PaidAmount.Value;
        //                        forecastExpense += i.TotalAmount.Value;
        //                        if (separatedTransList)
        //                        {
        //                            expenseList.Add(m);
        //                        }
        //                        else
        //                        {
        //                        totalTransactionList.Add(m);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        forecastIncome += i.TotalAmount.Value;
        //                        actualIncome += i.PaidAmount.Value;
        //
        //                        if (separatedTransList)
        //                        {
        //                            incomeList.Add(m);
        //                        }
        //                        else
        //                        {
        //                        totalTransactionList.Add(m);
        //                        }
        //                    }
        //                    
        //                }
        //
        //                // Add income from receipt to balancesheet
        //                foreach (var receiptIncome in totalAmountPubReceiptGroupByType)
        //                {
        //                    m = new TransactionModel();
        //                    m.UtilSrvCatName = receiptIncome.Name;
        //                    m.TransTitle = receiptIncome.Name;
        //                    m.TransStartDate = blSheet.ForMonth.Value.ToString("MM-yyyy");
        //                    m.TransType = SLIM_CONFIG.TRANSACTION_TYPE_INCOME;
        //                    m.TransTotalAmount = receiptIncome.TotalAmount;
        //                    m.TransPaidAmount = receiptIncome.PaidAmount;
        //                    m.TransCreateDate = blSheet.CreateDate.Value.ToString(AmsConstants.DateFormat);
        //                    m.TransEditable = 0;
        //                    if (separatedTransList)
        //                    {
        //                        incomeList.Add(m);
        //                    }
        //                    else
        //                    {
        //                        totalTransactionList.Add(m);
        //                    }
        //                }
        //            }
        //            redudancyStartMonth = blSheet.RedundancyStartMonth.Value;
        //            redudancyEndMonth = redudancyStartMonth + actualIncome - actualExpense;
        //            processedBls.ActualIncome = actualIncome;
        //            processedBls.ActualExpense = actualExpense;
        //            processedBls.ForecastExpense = forecastExpense;
        //            processedBls.ForecastIncome = forecastIncome;
        //            processedBls.RedudancyStartMonth = redudancyStartMonth;
        //            processedBls.RedudancyEndMonth = redudancyEndMonth;
        //            processedBls.LastModified = blSheet.LastModified.Value.ToString(AmsConstants.DateFormat);
        //            processedBls.TotalTransactionList = totalTransactionList;
        //            processedBls.IncomeList = incomeList;
        //            processedBls.ExpenseList = expenseList;
        //            return processedBls;
        //        }


        private BalanceSheetProcessingModel ProcessingBalanceSheetVersion2(BalanceSheet blSheet, bool separatedTransList)
        {
            double forecastIncome = 0;
            double actualIncome = 0;
            double actualExpense = 0;
            double forecastExpense = 0;
            double redudancyStartMonth = 0;
            double redudancyEndMonth = 0;
            List<TransactionModel> totalTransactionList = new List<TransactionModel>();
            List<TransactionModel> incomeList = new List<TransactionModel>();
            List<TransactionModel> expenseList = new List<TransactionModel>();
            BalanceSheetProcessingModel processedBls = new BalanceSheetProcessingModel();

            if (blSheet != null)
            {
                TransactionModel m = null;
                var receiptDetails =
                    blSheet.Transactions.GroupBy(trans => new
                    {
                        trans.ReceiptDetail.UtilityService.UtilSrvCatId,
                        trans.ReceiptDetail.Receipt.Type,
                        trans.ReceiptDetail.UtilityService.UtilServiceCategory.Status
                    }).Select(trans =>
                                new
                                {
                                    transaction = trans.First(),
                                    totalAmount = trans.Sum(t => t.TotalAmount),
                                    paidAmont = trans.Sum(t => t.PaidAmount)
                                }).ToList();
                var list = receiptDetails;

                foreach (var item in list)
                {
                    Transaction transaction = item.transaction;

                    m = new TransactionModel();
                    m.UtilSrvCatName = transaction.ReceiptDetail.UtilityService.UtilServiceCategory.Name;
                    m.DT_RowId = new StringBuilder("trans_").Append(transaction.Id).ToString();

                    m.TransStartDate = transaction.ReceiptDetail.Receipt.BalanceSheet.StartDate.Value.ToString(AmsConstants.MonthYearFormat);
                    m.TransTitle = transaction.ReceiptDetail.Receipt.Title;

                    if (transaction.ReceiptDetail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                    {
                        m.TransTitle = new StringBuilder("Thu tiền nước sinh hoạt [ ").Append(m.TransStartDate).Append(" ]").ToString();
                    }
                    else if (transaction.ReceiptDetail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
                    {
                        m.TransTitle = new StringBuilder("Thu tiền phí sinh hoạt [ ").Append(m.TransStartDate).Append(" ]").ToString();
                    }
                    else if (transaction.ReceiptDetail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_HD_REQUEST)
                    {
                        m.TransTitle = new StringBuilder("Thu tiền dịch vụ sửa chữa [ ").Append(m.TransStartDate).Append(" ]").ToString();
                    }

                    m.TransTotalAmount = item.totalAmount.Value;
                    m.TransPaidAmount = item.paidAmont.Value;

                    m.TransCreateDate = transaction.CreateDate.Value.ToString(AmsConstants.DateFormat);
                    m.UtilSrvCatId = transaction.ReceiptDetail.UtilityService.UtilSrvCatId.Value;
                    m.UtilSrvId = transaction.ReceiptDetail.UtilityService.Id;

                    m.ReceiptId = transaction.ReceiptDetail.Receipt.Id;
                    m.BalanceSheetId = transaction.BalanceSheet.Id;
                    m.TransactionId = transaction.Id;

                    m.TransType = transaction.ReceiptDetail.Receipt.Type.Value;
                    if (m.TransType == SLIM_CONFIG.TRANSACTION_TYPE_EXPENSE)
                    {
                        actualExpense += item.paidAmont.Value;
                        forecastExpense += item.totalAmount.Value;
                        if (separatedTransList)
                        {
                            expenseList.Add(m);
                        }
                        else
                        {
                            totalTransactionList.Add(m);
                        }
                    }
                    else
                    {
                        forecastIncome += item.totalAmount.Value;
                        actualIncome += item.paidAmont.Value;
                        if (separatedTransList)
                        {
                            incomeList.Add(m);
                        }
                        else
                        {
                            totalTransactionList.Add(m);
                        }
                    }
                }
            }
            redudancyStartMonth = blSheet.RedundancyStartMonth.Value;
            redudancyEndMonth = redudancyStartMonth + actualIncome - actualExpense;
            processedBls.ActualIncome = actualIncome;
            processedBls.ActualExpense = actualExpense;
            processedBls.ForecastExpense = forecastExpense;
            processedBls.ForecastIncome = forecastIncome;
            processedBls.RedudancyStartMonth = redudancyStartMonth;
            processedBls.RedudancyEndMonth = redudancyEndMonth;
            processedBls.LastModified = blSheet.LastModified.Value.ToString(AmsConstants.DateTimeFormat);
            processedBls.TotalTransactionList = totalTransactionList;
            processedBls.IncomeList = incomeList;
            processedBls.ExpenseList = expenseList;
            return processedBls;
        }

        private BalanceSheetProcessingModel GetTransactionGroupByUtilSrvCat(BalanceSheet blSheet, bool separatedTransList)
        {
            double forecastIncome = 0;
            double actualIncome = 0;
            double actualExpense = 0;
            double forecastExpense = 0;
            double redudancyStartMonth = 0;
            double redudancyEndMonth = 0;
            List<TransactionModel> totalTransactionList = new List<TransactionModel>();
            List<TransactionModel> incomeList = new List<TransactionModel>();
            List<TransactionModel> expenseList = new List<TransactionModel>();
            BalanceSheetProcessingModel processedBls = new BalanceSheetProcessingModel();
            BalanceSheet activeBalanceSheet = _balanceSheetService.GetCurentActivateBalanceSheet();
            if (blSheet != null)
            {
                TransactionModel m = null;
                var utilSrvCatInTrans =
                    blSheet.Transactions.Select(trans => trans.ReceiptDetail.UtilityService.UtilSrvCatId).Distinct().ToList();

                foreach (var item in utilSrvCatInTrans)
                {
                    var groupByCatTrans = new List<UtilSrvCatTransaction>();
                    List<Transaction> listTransaction = blSheet.Transactions.Where(
                               trans => trans.ReceiptDetail.UtilityService.UtilSrvCatId == item).ToList();
                    if (listTransaction.First().ReceiptDetail.UtilityService.Type !=
                        SLIM_CONFIG.UTILITY_SERVICE_TYPE_HD_REQUEST)
                    {
                        groupByCatTrans = listTransaction
                             .GroupBy(trans => new
                             {
                                 trans.ReceiptDetail.Receipt.Type,
                                 trans.ReceiptDetail.Receipt.Title,
                             })
                             .Select(trans => new UtilSrvCatTransaction(trans.First(),
                                 trans.Sum(t => t.TotalAmount).Value,
                                 trans.Sum(t => t.PaidAmount).Value)).ToList();
                    }
                    else
                    {
                        groupByCatTrans = listTransaction
                            .GroupBy(trans => new
                            {
                                trans.ReceiptDetail.Receipt.BlsId,
                                trans.ReceiptDetail.UtilityService.UtilSrvCatId
                            })
                            .Select(trans => new UtilSrvCatTransaction(trans.First(),
                                trans.Sum(t => t.TotalAmount).Value,
                                trans.Sum(t => t.PaidAmount).Value)).ToList();
                        groupByCatTrans.First().Transaction.ReceiptDetail.Receipt.Title = AmsConstants.TransactionNameForFixedCostBill;
                    }

                    foreach (var trans in groupByCatTrans)
                    {
                        m = ParseData(activeBalanceSheet, trans.Transaction, trans.TotalAmount, trans.TotalPaid);

                        if (m.TransType == SLIM_CONFIG.TRANSACTION_TYPE_EXPENSE)
                        {
                            actualExpense += trans.TotalPaid;
                            forecastExpense += trans.TotalAmount;
                            if (separatedTransList)
                            {
                                expenseList.Add(m);
                            }
                            else
                            {
                                totalTransactionList.Add(m);
                            }
                        }
                        else
                        {
                            forecastIncome += trans.TotalAmount;
                            actualIncome += trans.TotalPaid;
                            if (separatedTransList)
                            {
                                incomeList.Add(m);
                            }
                            else
                            {
                                totalTransactionList.Add(m);
                            }
                        }
                    }
                }
            }
            redudancyStartMonth = blSheet.RedundancyStartMonth.Value;
            redudancyEndMonth = redudancyStartMonth + actualIncome - actualExpense;
            processedBls.ActualIncome = actualIncome;
            processedBls.ActualExpense = actualExpense;
            processedBls.ForecastExpense = forecastExpense;
            processedBls.ForecastIncome = forecastIncome;
            processedBls.RedudancyStartMonth = redudancyStartMonth;
            processedBls.RedudancyEndMonth = redudancyEndMonth;
            processedBls.LastModified = blSheet.LastModified.Value.ToString(AmsConstants.DateTimeFormat);
            processedBls.TotalTransactionList = totalTransactionList;
            processedBls.IncomeList = incomeList;
            processedBls.ExpenseList = expenseList;
            return processedBls;
        }
        private TransactionModel ParseData(BalanceSheet activeBalanceSheet, Transaction transaction, double totalAmount, double paidAmount)
        {
            TransactionModel m = new TransactionModel();

            m.UtilSrvCatName = transaction.ReceiptDetail.UtilityService.UtilServiceCategory.Name;
            m.DT_RowId = new StringBuilder("trans_").Append(transaction.Id).ToString();

            // get it own bls start date
            m.TransStartDate = transaction.ReceiptDetail.Receipt.BalanceSheet.StartDate.Value.ToString(AmsConstants.MonthYearFormat);
            m.TransTitle = transaction.ReceiptDetail.Receipt.Title;

            if (transaction.ReceiptDetail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
            {
                m.TransTitle = new StringBuilder("Thu tiền nước sinh hoạt [ ").Append(m.TransStartDate).Append(" ]").ToString();
            }
            else if (transaction.ReceiptDetail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
            {
                m.TransTitle = new StringBuilder("Thu tiền phí sinh hoạt [ ").Append(m.TransStartDate).Append(" ]").ToString();
            }
            else if (transaction.ReceiptDetail.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_HD_REQUEST)
            {
                m.TransTitle = new StringBuilder("Thu tiền dịch vụ sửa chữa [ ").Append(m.TransStartDate).Append(" ]").ToString();
            }

            m.TransTotalAmount = totalAmount;
            m.TransPaidAmount = paidAmount;

            m.TransCreateDate = transaction.CreateDate.Value.ToString(AmsConstants.DateFormat);
            m.UtilSrvCatId = transaction.ReceiptDetail.UtilityService.UtilSrvCatId.Value;
            m.UtilSrvId = transaction.ReceiptDetail.UtilityService.Id;

            m.ReceiptId = transaction.ReceiptDetail.Receipt.Id;
            m.BalanceSheetId = transaction.BalanceSheet.Id;
            m.TransactionId = transaction.Id;
            m.TransType = transaction.ReceiptDetail.Receipt.Type.Value;

            if (transaction.ReceiptDetail.UtilityService.Type != SLIM_CONFIG.UTILITY_SERVICE_TYPE_NOT_FOR_RESIDENT)
            {
                m.TransEditable = -1;
            }
            else if (transaction.ReceiptDetail.Receipt.BalanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSE)
            {
                if (transaction.BalanceSheet.Id == activeBalanceSheet.Id)
                {
                    m.TransEditable = 1;
                }
                else
                {
                    m.TransEditable = -1;
                }
            }
            else if (transaction.BalanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN)
            {
                m.TransEditable = 1;
            }
            else
            {
                m.TransEditable = -1;
            }
            return m;
        }
    }
}