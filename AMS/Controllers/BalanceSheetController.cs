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
            DateTime requestMonth = new DateTime();
            if (null == month)
            {
                requestMonth = DateTime.Today;
            }
            else
            {
                requestMonth = DateTime.ParseExact(month, "MM-yyyy", CultureInfo.CurrentCulture);
            }

            double totalIncome = 0;
            double totalPaidIncome = 0;

            double totalExpense = 0;
            double totalPaidExpense = 0;


            /*Start process balance sheet*/
             
            BalanceSheet thisMonthBS = _balanceSheetService.GetBalanceSheetForMonth(requestMonth);
            if (null == thisMonthBS)
            {
                ViewBag.balanceSheetIsCreated = false;
                return View("BalanceSheet");
            }


            List<Transaction> thisMonthIncome =
                thisMonthBS.Transactions.Where(tr => tr.Type == SLIM_CONFIG.TRANSACTION_TYPE_INCOME).ToList();
            foreach (var iTrans in thisMonthIncome)
            {
                totalIncome += iTrans.TotalAmount.Value;
                totalPaidIncome += iTrans.PaidAmount.Value;
            }

            // Get all income transaction category that existed in one month
            List<Transaction> inMonthIncomeCategory = thisMonthIncome.GroupBy(h => h.CategoryId).Select(h => h.First()).ToList();
            List<AmountGroupByTransCategory> listIncomeGroupByCategories = new List<AmountGroupByTransCategory>();
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
                listIncomeGroupByCategories.Add(income);
            }

            List<Transaction> thisMonthExpense =
                thisMonthBS.Transactions.Where(tr => tr.Type == SLIM_CONFIG.TRANSACTION_TYPE_EXPENSE).ToList();
            foreach (var e in thisMonthExpense)
            {
                totalExpense += e.TotalAmount.Value;
                totalPaidExpense += e.PaidAmount.Value;
            }

            // Get all expense transaction category that existed in one month
            List<Transaction> inMonthExpensseCategory = thisMonthExpense.GroupBy(h => h.CategoryId).Select(h => h.First()).ToList();
            List<AmountGroupByTransCategory> listExpenseGroupByCategories = new List<AmountGroupByTransCategory>();
            foreach (var transCat in inMonthExpensseCategory)
            {
                AmountGroupByTransCategory expense = new AmountGroupByTransCategory();
                expense.Name = transCat.TransactionCategory.Name;
                double total = 0;
                double paid = 0;
                double unpaid = 0;
                List<Transaction> groupedByCatTransaction = thisMonthExpense.Where(tc => transCat.CategoryId == tc.TransactionCategory.Id).ToList();
                foreach (var eTrans in groupedByCatTransaction)
                {
                    total += eTrans.TotalAmount.Value;
                    paid += eTrans.PaidAmount.Value;
                    unpaid = total - paid;
                }
                expense.TotalAmount = total;
                expense.PaidAmount = paid;
                expense.UnpaidAmount = unpaid;
                listExpenseGroupByCategories.Add(expense);
            }

            
            ///Start process receipt
            /// 
            List<Receipt> publishedReceipt = _receiptService.GetReceiptInMounth(requestMonth).Where(r => r.Status != SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED).ToList();
            List<Receipt> publishedReceiptGroupByType = publishedReceipt.GroupBy(r => r.Type).Select(r => r.First()).ToList();

            List<AmountGroupByTransCategory> totalAmountPubReceiptGroupByType = new List<AmountGroupByTransCategory>();
            foreach (var recType in publishedReceiptGroupByType)
            {
                AmountGroupByTransCategory pubReceip = new AmountGroupByTransCategory();
                string catName = "";
                int id = 0;
                if (recType.Type == SLIM_CONFIG.RECEIPT_TYPE_HD_REQUEST)
                {
                    id = 1;
                    catName = "Dịch vụ sửa chữa";
                }
                else if (recType.Type == SLIM_CONFIG.RECEIPT_TYPE_WATER)
                {
                    id = 2;
                    catName = "Nước";
                }
                else if (recType.Type == SLIM_CONFIG.RECEIPT_TYPE_ELECTRIC)
                {
                    id = 3;
                    catName = "Điện";
                }
                else if (recType.Type == SLIM_CONFIG.RECEIPT_TYPE_HOUSE_RENT)
                {
                    id = 4;
                    catName = "Cho thuê nhà";
                }

                pubReceip.Name = catName;
                pubReceip.Id = id;

                List<Receipt> groupByTypeReceipts = publishedReceipt.Where(r => r.Type == recType.Type).ToList();
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
                totalIncome += total;
                pubReceip.TotalAmount = total;
                pubReceip.UnpaidAmount = total;
                totalAmountPubReceiptGroupByType.Add(pubReceip);
            }

            List<Receipt> paidReceipt = _receiptService.GetReceiptInMounth(requestMonth).Where(r => r.Status == SLIM_CONFIG.RECEIPT_STATUS_PAID).ToList();
            List<Receipt> paidReceiptGroupByType = paidReceipt.GroupBy(r => r.Type).Select(r => r.First()).ToList();

            foreach (var recType in paidReceiptGroupByType)
            {
                AmountGroupByTransCategory pubReceip = new AmountGroupByTransCategory();
                string catName = "";
                int id = 0;
                if (recType.Type == SLIM_CONFIG.RECEIPT_TYPE_HD_REQUEST)
                {
                    id = 1;
                }
                else if (recType.Type == SLIM_CONFIG.RECEIPT_TYPE_WATER)
                {
                    id = 2;
                }
                else if (recType.Type == SLIM_CONFIG.RECEIPT_TYPE_ELECTRIC)
                {
                    id = 3;
                }
                else if (recType.Type == SLIM_CONFIG.RECEIPT_TYPE_HOUSE_RENT)
                {
                    id = 4;
                }

                pubReceip.Name = catName;
                List<Receipt> groupByTypeReceipts = paidReceipt.Where(r => r.Type == recType.Type).ToList();
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
                totalPaidIncome += total;

                // Find this category in published receipt to update paid amount
                AmountGroupByTransCategory pubReceipt = null;
                for (int i = 0; i < totalAmountPubReceiptGroupByType.Count; i++)
                {
                    pubReceipt = totalAmountPubReceiptGroupByType[i];
                    if (id == pubReceipt.Id)
                    {
                        pubReceipt.PaidAmount = total;
                        pubReceipt.UnpaidAmount = pubReceipt.TotalAmount - total;
                        totalAmountPubReceiptGroupByType[i] = pubReceipt;
                        break;
                    }
                }
            }

            if (requestMonth.Date.Month == DateTime.Today.Date.Month &&
                requestMonth.Date.Year == DateTime.Today.Date.Year)
            {
                ViewBag.balanceSheetStatus = 1;
            }
            else
            {
                ViewBag.balanceSheetStatus = -1;
            }

            List<AmountGroupByTransCategory> incomeTransactions = new List<AmountGroupByTransCategory>();
            incomeTransactions.AddRange(listIncomeGroupByCategories);
            incomeTransactions.AddRange(totalAmountPubReceiptGroupByType);

            var incomeTransactionsJson = new JavaScriptSerializer().Serialize(incomeTransactions);
            var expenseTransactionsJson = new JavaScriptSerializer().Serialize(listExpenseGroupByCategories);

            ViewBag.thisMonth = requestMonth.ToString("MM-yyyy");
            ViewBag.createDate = thisMonthBS.CreateDate.Value.ToString(AmsConstants.DateFormat);
            ViewBag.lastUpdate = thisMonthBS.LastModified.Value.ToString(AmsConstants.DateFormat);
            ViewBag.description = thisMonthBS.Description;
            ViewBag.balanceSheetId = thisMonthBS.Id;

            ViewBag.totalIncome = totalIncome;
            ViewBag.totalPaidIncome = totalPaidIncome;
            ViewBag.totalUnpaidIncome = totalIncome - totalPaidIncome;

            ViewBag.totalExpense = totalExpense;
            ViewBag.totalPaidExpense = totalPaidExpense;
            ViewBag.totalUnpaidExpense = totalExpense - totalPaidExpense;

            ViewBag.listExpenseGroupByCategories = listExpenseGroupByCategories;
            ViewBag.incomeTransactions = incomeTransactions;

            ViewBag.incomeTransactionsJson = incomeTransactionsJson;
            ViewBag.expenseTransactionsJson = expenseTransactionsJson;

            return View("BalanceSheet");
        }

        [HttpGet]
        [Route("Management/BalanceSheet/ManageIncomeView")]
        public ActionResult ViewManageIncome()
        {
            return View("ManageTransaction");
        }

        [HttpGet]
        [Route("Management/BalanceSheet/ManageTransactionCatView")]
        public ActionResult ViewManageTransactionCat()
        {
            return View("ManageTransactionCategory");
        }

        [HttpGet]
        [Route("Management/BalanceSheet/ManageLiabilities")]
        public ActionResult ViewManageLiabilities()
        {

            return View("BalanceSheet");
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
                    DateTime transInMonth = DateTime.ParseExact(transaction.TransForMonth, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                    DateTime today = DateTime.Now;
                    DateTime firstDateOfThisMounth = new DateTime(today.Year, today.Month, 1);
                    BalanceSheet existedTrans = _balanceSheetService.FindByMonthYear(transInMonth);
                    Transaction transEntity = new Transaction();

                    if (null != existedTrans && (existedTrans.ForMonth != firstDateOfThisMounth))
                    {
                        response.StatusCode = -1;
                        response.Msg = "Khai báo thu chi đã đóng";
                        return Json(response);
                    }
                    else if (null != existedTrans && (existedTrans.ForMonth == firstDateOfThisMounth))
                    {
                        transEntity.TransactionId = existedTrans.Id;
                    }
                    else if (null == existedTrans && transInMonth == firstDateOfThisMounth)
                    {
                        BalanceSheet newBalanceSheet = new BalanceSheet();
                        newBalanceSheet.ForMonth = transInMonth;
                        newBalanceSheet.CreateDate = DateTime.Now;
                        newBalanceSheet.LastModified = DateTime.Now;
                        _balanceSheetService.Add(newBalanceSheet);
                        transEntity.TransactionId = newBalanceSheet.Id;
                    }
                    transEntity.Type = transaction.TransType;
                    transEntity.CategoryId = transCat.Id;
                    transEntity.TotalAmount = transaction.TransTotalAmount;
                    transEntity.PaidAmount = transaction.TransPaidAmount;
                    transEntity.Description = transaction.TransDesc;
                    transEntity.CreateDate = DateTime.Now;
                    transEntity.LastModified = DateTime.Now;
                    transEntity.Name = transaction.TransTitle;

                    _transactionService.Add(transEntity);
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
                DateTime today = DateTime.Now;
                DateTime firstDateOfThisMounth = new DateTime(today.Year, today.Month, 1);

                if (null == trans)
                {
                    response.StatusCode = -1;
                    response.Msg = "Không tìm thấy giao dịch";
                    return Json(response);
                }
                if (trans.BalanceSheet.ForMonth != firstDateOfThisMounth)
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
                foreach (var transId in transDeletedList)
                {
                    Transaction transaction = _transactionService.FindById(transId);
                    DateTime today  = DateTime.Today;
                    if (transaction != null && transaction.BalanceSheet.ForMonth.Value.Month == today.Month && transaction.BalanceSheet.ForMonth.Value.Year == today.Year)
                    {
                        _transactionService.Delete(transaction);
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
        public ActionResult GetIncomeTransItems()
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {

                List<Transaction> list = _transactionService.GetByTransType();
                List<TransactionModel> modelList = new List<TransactionModel>();
                TransactionModel m = null;
                DateTime now = DateTime.Today.Date;
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

                    if (i.BalanceSheet.ForMonth.Value.Date.Month == now.Month &&
                        i.BalanceSheet.ForMonth.Value.Year == now.Year)
                    {
                        m.TransEditable = 1;
                    }
                    else
                    {
                        m.TransEditable = -1;
                    }
                    
                    m.TransType = i.Type.Value;
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
                    //                    }
                    //                    else
                    //                    {
                    //                        response.StatusCode = 5;
                    //                        response.Msg = "Bảng thu chi đã đóng";
                    //                    }
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
    }

}