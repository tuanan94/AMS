using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AMS.Constant;
using AMS.Models;
using AMS.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace AMS.Controllers
{
    public class BalanceSheetController : Controller
    {
        readonly UserServices _userServices = new UserServices();
        readonly BalanceSheetService _balanceSheetService = new BalanceSheetService();
        readonly TransactionCategoryService _transCategoryService = new TransactionCategoryService();
        readonly TransactionService _transactionService = new TransactionService();
        readonly ReceiptServices _receiptService = new ReceiptServices();

        [HttpGet]
        [Route("Management/BalanceSheet/View")]
        public ActionResult ViewBalanceSheet(string month)
        {
            DateTime requestMonth = DateTime.Today;
            if (!month.IsNullOrWhiteSpace())
            {
                requestMonth = DateTime.ParseExact(month, "MM-yyyy", CultureInfo.CurrentCulture);
            }

            BalanceSheet thisMonthBS = _balanceSheetService.GetBalanceSheetForMonth(requestMonth);
            if (null == thisMonthBS)
            {
                ViewBag.balanceSheetIsCreated = false;
                return View("BalanceSheet");
            }

            BalanceSheetProcessingModel processedBls = ProcessingBalanceSheet(thisMonthBS, true);

            var incomeTransactionsJson = new JavaScriptSerializer().Serialize(processedBls.IncomeList);
            var expenseTransactionsJson = new JavaScriptSerializer().Serialize(processedBls.ExpenseList);

            ViewBag.thisMonth = thisMonthBS.ForMonth.Value.Date.ToString("MM-yyyy");
            ViewBag.fromDate = thisMonthBS.CreateDate.Value.ToString(AmsConstants.DateFormat);
            ViewBag.closingDate = null;
            if (thisMonthBS.ClosingDate != null)
            {
                ViewBag.closingDate = thisMonthBS.ClosingDate.Value.ToString(AmsConstants.DateFormat);
            }
            ViewBag.lastUpdate = thisMonthBS.LastModified.Value.ToString(AmsConstants.DateFormat);
            ViewBag.description = thisMonthBS.Description;
            ViewBag.balanceSheetId = thisMonthBS.Id;
            ViewBag.status = thisMonthBS.Status;

            ViewBag.balanceSheet = processedBls;
            ViewBag.incomeTransactionsJson = incomeTransactionsJson;
            ViewBag.expenseTransactionsJson = expenseTransactionsJson;

            return View("BalanceSheet");
        }



        [HttpGet]
        [Route("Management/BalanceSheet/ManageTransactionCatView")]
        public ActionResult ViewManageTransactionCat()
        {
            return View("ManageTransactionCategory");
        }

        [HttpGet]
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
                BalanceSheetProcessingModel processedBls = ProcessingBalanceSheet(bs, true);
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
                bsModel.ForMonth = bs.ForMonth.Value.ToString("MM-yyyy");
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
            if (null != bls.ForMonth)
            {
                DateTime forMonth = DateTime.ParseExact(bls.ForMonth, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                if (_balanceSheetService.hasBalanceSheet())
                {
                        BalanceSheet eBls = new BalanceSheet();
                        eBls.ManagerId = Int32.Parse(User.Identity.GetUserId());
                        eBls.CreateDate = DateTime.Now;
                        eBls.LastModified = DateTime.Now;
                        eBls.RedundancyStartMonth = bls.RedundancyStartMonth;
                        eBls.ForMonth = forMonth;
                        eBls.Title = bls.Name;
                        eBls.Status = SLIM_CONFIG.BALANCE_SHEET_OPEN;
                        _balanceSheetService.Add(eBls);
                }//reject if balance sheet has openend already
                else
                {
                    response.StatusCode = -1;
                    response.Msg = "Hóa đơn tháng " + bls.ForMonth + " đã được tạo.";
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
                BalanceSheetProcessingModel processingBls = ProcessingBalanceSheet(bls, false);
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
                newBls.CreateDate = DateTime.Now;
                newBls.LastModified = DateTime.Now;
                newBls.RedundancyStartMonth = bls.RedundancyEndMonth;
                newBls.ForMonth = DateTime.Today.Date.AddDays(1);
                newBls.Title = blsheetModel.NextMonthTitle;
                newBls.Status = SLIM_CONFIG.BALANCE_SHEET_OPEN;
                _balanceSheetService.Add(newBls);
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
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {
                TransactionCategory transItemCatE = new TransactionCategory();
                transItemCatE.Name = transCategory.TransCategoryName;
                _transCategoryService.Add(transItemCatE);
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = AmsConstants.MsgUserNotFound;
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/BalanceSheet/UpdateTransactionType")]
        public ActionResult UpdateTransactionType(TransCategoryModel transCategory)
        {
            MessageViewModels response = new MessageViewModels();

            TransactionCategory transItemCatE = _transCategoryService.FindById(transCategory.TransCategoryId);
            if (null != transItemCatE)
            {
                transItemCatE.Name = transCategory.TransCategoryName;
                _transCategoryService.Update(transItemCatE);
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
                TransactionCategory transCat = _transCategoryService.FindById(transaction.TransCatId);
                if (null != transCat)
                {
                    BalanceSheet balanceSheet = _balanceSheetService.FindById(transaction.BalanceSheetId);
                    Transaction transEntity = new Transaction();

                    if (null == balanceSheet)
                    {
                        response.StatusCode = -1;
                        response.Msg = "Bảng quản lý thu chi đã đóng";
                        return Json(response);
                    }
                    if (balanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSE)
                    {
                        response.StatusCode = -1;
                        response.Msg = "Bảng quản lý thu chi không tồn tại";
                        return Json(response);
                    }

                    transEntity.BalanceSheetId = balanceSheet.Id;
                    transEntity.Type = transaction.TransType;
                    transEntity.CategoryId = transCat.Id;
                    transEntity.TotalAmount = transaction.TransTotalAmount;
                    transEntity.PaidAmount = transaction.TransPaidAmount;
                    transEntity.Description = transaction.TransDesc;
                    transEntity.CreateDate = DateTime.Now;
                    transEntity.LastModified = DateTime.Now;
                    transEntity.Name = transaction.TransTitle;

                    _transactionService.Add(transEntity);
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
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {
                Transaction trans = _transactionService.FindById(transModel.TransId);
                if (null == trans)
                {
                    response.StatusCode = -1;
                    response.Msg = "Không tìm thấy giao dịch";
                    return Json(response);
                }
                if (trans.BalanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSE)
                {
                    response.StatusCode = -1;
                    response.Msg = "Khai báo thu chi đã đóng";
                    return Json(response);
                }
                TransactionCategory transCat = _transCategoryService.FindById(transModel.TransCatId);
                if (null != transCat)
                {
                    trans.CategoryId = transCat.Id;
                    trans.Type = transModel.TransType;
                    trans.TotalAmount = transModel.TransTotalAmount;
                    trans.PaidAmount = transModel.TransPaidAmount;
                    trans.Description = transModel.TransDesc;
                    trans.LastModified = DateTime.Now;
                    trans.Name = transModel.TransTitle;
                    _transactionService.Update(trans);

                    BalanceSheet bls = trans.BalanceSheet;
                    bls.LastModified = DateTime.Now;
                    _balanceSheetService.Update(bls);
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
        [Route("Management/BalanceSheet/DeleteTransactions")]
        public ActionResult DeleteTransactions(List<int> transDeletedList)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {
                Transaction transaction = null;
                foreach (var transId in transDeletedList)
                {
                    transaction = _transactionService.FindById(transId);
                    if (transaction != null && transaction.BalanceSheet.Status != SLIM_CONFIG.BALANCE_SHEET_CLOSE)
                    {
                        _transactionService.Delete(transaction);

                    }
                }
                if (transaction != null)
                {
                    BalanceSheet bls = transaction.BalanceSheet;
                    bls.LastModified = DateTime.Now;
                    _balanceSheetService.Update(bls);
                }

            }
            else
            {
                response.StatusCode = -1;
                response.Msg = AmsConstants.MsgUserNotFound;
            }
            return Json(response);
        }

        [HttpGet]
        [Route("Management/BalanceSheet/GetAllTransactionType")]
        public ActionResult GetTransactionType(int type)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {

                List<TransactionCategory> list = _transCategoryService.GetAll();
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
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = AmsConstants.MsgUserNotFound;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/BalanceSheet/GetAllTransactionTypeFull")]
        public ActionResult GetTransactionTypeFull()
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {

                List<TransactionCategory> list = _transCategoryService.GetAll();
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
            else
            {
                response.StatusCode = -1;
                response.Msg = AmsConstants.MsgUserNotFound;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/BalanceSheet/GetIncomeTransItems")]
        public ActionResult GetIncomeTransItems(int blsId)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {
                BalanceSheet bls = _balanceSheetService.FindById(blsId);
                BalanceSheetProcessingModel processedBls = ProcessingBalanceSheet(bls, false);
                Object obj = new
                {
                    data = processedBls.TotalTransactionList,
                    forecastIncome = processedBls.ForecastIncome,
                    actualIncome = processedBls.ActualIncome,
                    forecastExpense = processedBls.ForecastExpense,
                    actualExpense = processedBls.ActualExpense,
                    redudancyEndMonth = processedBls.RedudancyEndMonth,
                    redudancyStartMonth = processedBls.RedudancyStartMonth,
                    lastUpdate = processedBls.LastModified
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
        public ActionResult GetTransItemDetail(int id)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {

                Transaction transaction = _transactionService.FindById(id);
                if (null != transaction)
                {

                    TransactionModel model = new TransactionModel();
                    model.TransCatName = transaction.TransactionCategory.Name;
                    model.TransCatId = transaction.TransactionCategory.Id;
                    model.TransDesc = transaction.Description;
                    model.TransForMonth = transaction.BalanceSheet.ForMonth.Value.ToString("MM-yyyy");
                    model.TransTitle = transaction.Name;
                    model.TransTotalAmount = transaction.TotalAmount.Value;
                    model.TransPaidAmount = transaction.PaidAmount.Value;
                    model.TransId = transaction.Id;
                    model.TransType = transaction.Type.Value;
                    response.Data = model;

                }
                else
                {
                    response.StatusCode = -1;
                    response.Msg = AmsConstants.MsgUserNotFound;
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = AmsConstants.MsgUserNotFound;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/BalanceSheet/GetDetailTransCat")]
        public ActionResult GetDetailTransCat(int transCat)
        {
            MessageViewModels response = new MessageViewModels();
            TransactionCategory transaction = _transCategoryService.FindById(transCat);
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
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {
                foreach (var transId in transCatDeletedList)
                {
                    TransactionCategory transactionCat = _transCategoryService.FindById(transId);
                    if (transactionCat != null)
                    {
                        _transCategoryService.Delete(transactionCat);
                    }
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = AmsConstants.MsgUserNotFound;
            }
            return Json(response);
        }

        private List<AmountGroupByTransCategory> GroupTransactionByType(BalanceSheet thisMonthBs, int type)
        {
            List<Transaction> thisMonthIncome =
                thisMonthBs.Transactions.Where(tr => tr.Type == type).ToList();
            // Get all income transaction category that existed in one month
            List<Transaction> inMonthIncomeCategory = thisMonthIncome.GroupBy(h => h.CategoryId).Select(h => h.First()).ToList();
            List<AmountGroupByTransCategory> transGroupByCategories = new List<AmountGroupByTransCategory>();
            foreach (var transCat in inMonthIncomeCategory)
            {
                AmountGroupByTransCategory income = new AmountGroupByTransCategory();
                income.Name = transCat.TransactionCategory.Name;
                double total = 0;
                double paid = 0;
                double unpaid = 0;

                List<Transaction> groupedByCatTransaction = thisMonthIncome.Where(tc => transCat.CategoryId == tc.TransactionCategory.Id).ToList();

                foreach (var iTrans in groupedByCatTransaction)
                {
                    total += iTrans.TotalAmount.Value;
                    paid += iTrans.PaidAmount.Value;
                    unpaid = total - paid;
                }
                income.TotalAmount = total;
                income.PaidAmount = paid;
                income.UnpaidAmount = unpaid;
                transGroupByCategories.Add(income);
            }
            return transGroupByCategories;
        }

        private AmountGroupByTransCategory calculateTotalReceipt(Receipt recType, List<Receipt> thismonthPublisedReceipts)
        {

            AmountGroupByTransCategory pubReceip = new AmountGroupByTransCategory();
            string catName = "";
            int id = 0;
            if (recType.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_HD_REQUEST)
            {
                id = 1;
                catName = "Dịch vụ sửa chữa";
            }
            else if (recType.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
            {
                id = 2;
                catName = "Nước";
            }
            
            pubReceip.Name = catName;
            pubReceip.Id = id;

            List<Receipt> groupByTypeReceipts = thismonthPublisedReceipts.Where(r => r.Type == recType.Type).ToList();
            List<ReceiptDetail> listReceiptDetails = null;
            double total = 0;
            foreach (var rec in groupByTypeReceipts)
            {
                listReceiptDetails = rec.ReceiptDetails.ToList();
                foreach (var detail in listReceiptDetails)
                {
                    total += detail.Quantity.Value * detail.UnitPrice.Value;
                }
            }
            pubReceip.TotalAmount = total;
            pubReceip.UnpaidAmount = total;
            return pubReceip;
        }

        private BalanceSheetProcessingModel ProcessingBalanceSheet(BalanceSheet blSheet, bool separatedTransList)
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

                // Get income from receipt
                List<Receipt> publishedReceipt = null;
                if (blSheet.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN)
                {
                    publishedReceipt =
                        _receiptService.GetReceiptInMonthFromOpeningToToday(blSheet)
                            .Where(r => r.Status != SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
                            .ToList();
                }// Get new receipts are created from the date bls is open to today
                else
                {
                    publishedReceipt =
                        _receiptService.GetReceiptInMonthWhileBalanceSheetOpen(blSheet)
                            .Where(r => r.Status != SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED)
                            .ToList();
                }// Get new receipts are created from the date bls is open to closing date

                List<Receipt> publishedReceiptGroupByType = publishedReceipt.GroupBy(r => r.Type).Select(r => r.First()).ToList();

                List<AmountGroupByTransCategory> totalAmountPubReceiptGroupByType = new List<AmountGroupByTransCategory>();
                foreach (var recType in publishedReceiptGroupByType)
                {
                    AmountGroupByTransCategory receiptAmountObj = calculateTotalReceipt(recType, publishedReceipt);
                    totalAmountPubReceiptGroupByType.Add(receiptAmountObj);
                    forecastIncome += receiptAmountObj.TotalAmount;
                }

                List<Receipt> paidReceipt = null;
                if (blSheet.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSE)
                {
                    paidReceipt = _receiptService.GetPaidReceiptWhileBalanceSheetOpen(blSheet).ToList();
                }
                else
                {
                    paidReceipt = _receiptService.GetPaidReceiptOfBalanceSheetFromOpeningToToday(blSheet).ToList();
                }

                List<Receipt> paidReceiptGroupByType = paidReceipt.GroupBy(r => r.Type).Select(r => r.First()).ToList();

                foreach (var recType in paidReceiptGroupByType)
                {
                    // Find this category in published receipt to update paid amount
                    AmountGroupByTransCategory receiptAmountObj = calculateTotalReceipt(recType, paidReceipt);
                    //                        totalPaidIncome += receiptAmountObj.TotalAmount;

                    AmountGroupByTransCategory pubReceipt = null;
                    for (int i = 0; i < totalAmountPubReceiptGroupByType.Count; i++)
                    {
                        pubReceipt = totalAmountPubReceiptGroupByType[i];
                        if (receiptAmountObj.Id == pubReceipt.Id)
                        {
                            pubReceipt.PaidAmount = receiptAmountObj.TotalAmount;
                            pubReceipt.UnpaidAmount = pubReceipt.TotalAmount - receiptAmountObj.TotalAmount;
                            totalAmountPubReceiptGroupByType[i] = pubReceipt;
                            actualIncome += pubReceipt.PaidAmount;
                            break;
                        }
                    }
                }

                

                TransactionModel m = null;
                List<Transaction> list = blSheet.Transactions.ToList();
                foreach (var i in list)
                {
                    m = new TransactionModel();
                    m.TransCatName = i.TransactionCategory.Name;
                    m.TransId = i.Id;
                    m.DT_RowId = new StringBuilder("trans_").Append(i.Id).ToString();
                    m.TransForMonth = i.BalanceSheet.ForMonth.Value.ToString("MM-yyyy");
                    m.TransTitle = i.Name;
                    m.TransTotalAmount = i.TotalAmount.Value;
                    m.TransPaidAmount = i.PaidAmount.Value;
                    m.TransCreateDate = i.CreateDate.Value.ToString(AmsConstants.DateFormat);

                    if (i.BalanceSheet.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN)
                    {
                        m.TransEditable = 1;
                    }
                    else
                    {
                        m.TransEditable = -1;
                    }
                    m.TransType = i.Type.Value;
                    if (i.Type == SLIM_CONFIG.TRANSACTION_TYPE_EXPENSE)
                    {
                        actualExpense += i.PaidAmount.Value;
                        forecastExpense += i.TotalAmount.Value;
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
                        forecastIncome += i.TotalAmount.Value;
                        actualIncome += i.PaidAmount.Value;

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

                // Add income from receipt to balancesheet
                foreach (var receiptIncome in totalAmountPubReceiptGroupByType)
                {
                    m = new TransactionModel();
                    m.TransCatName = receiptIncome.Name;
                    m.TransTitle = receiptIncome.Name;
                    m.TransForMonth = blSheet.ForMonth.Value.ToString("MM-yyyy");
                    m.TransType = SLIM_CONFIG.TRANSACTION_TYPE_INCOME;
                    m.TransTotalAmount = receiptIncome.TotalAmount;
                    m.TransPaidAmount = receiptIncome.PaidAmount;
                    m.TransCreateDate = blSheet.CreateDate.Value.ToString(AmsConstants.DateFormat);
                    m.TransEditable = 0;
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
            redudancyStartMonth = blSheet.RedundancyStartMonth.Value;
            redudancyEndMonth = redudancyStartMonth + actualIncome - actualExpense;
            processedBls.ActualIncome = actualIncome;
            processedBls.ActualExpense = actualExpense;
            processedBls.ForecastExpense = forecastExpense;
            processedBls.ForecastIncome = forecastIncome;
            processedBls.RedudancyStartMonth = redudancyStartMonth;
            processedBls.RedudancyEndMonth = redudancyEndMonth;
            processedBls.LastModified = blSheet.LastModified.Value.ToString(AmsConstants.DateFormat);
            processedBls.TotalTransactionList = totalTransactionList;
            processedBls.IncomeList = incomeList;
            processedBls.ExpenseList = expenseList;
            return processedBls;
        }
    }

}