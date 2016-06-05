using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Constant;
using AMS.Models;
using AMS.Service;
using Microsoft.AspNet.Identity;

namespace AMS.Controllers
{
    public class BalanceSheetController : Controller
    {
        readonly UserServices _userServices = new UserServices();
        readonly TransactionService _transactionService = new TransactionService();
        readonly TransItemCategoryService _transItemCategoryService = new TransItemCategoryService();
        readonly TransItemService _transItemService = new TransItemService();

        [HttpGet]
        [Route("Management/BalanceSheet/View")]
        public ActionResult ViewBalanceSheet()
        {
            return View("BalanceSheet");
        }

        [HttpGet]
        [Route("Management/BalanceSheet/ManageIncomeView")]
        public ActionResult ViewManageIncome()
        {
            return View("ManageIncome");
        }

        [HttpGet]
        [Route("Management/BalanceSheet/ManageLiabilities")]
        public ActionResult ViewManageLiabilities()
        {
            return View("BalanceSheet");
        }

        [HttpPost]
        [Route("Management/BalanceSheet/AddTransactionType")]
        public ActionResult AddTransactionType(TransItemCatModel transItemCat)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {
                if (transItemCat.TransItemType == SLIM_CONFIG.TRANSACTION_TYPE_EXPENSE ||
                    transItemCat.TransItemType == SLIM_CONFIG.TRANSACTION_TYPE_INCOME)
                {
                    TransactionItemCategory transItemCatE = new TransactionItemCategory();
                    transItemCatE.Name = transItemCat.TransItemCatName;
                    transItemCatE.Type = transItemCat.TransItemType;
                    _transItemCategoryService.Add(transItemCatE);
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
        [Route("Management/BalanceSheet/AddTransactionItem")]
        public ActionResult AddTransactionItem(TransItemModel transItem)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {
                TransactionItemCategory transCat = _transItemCategoryService.FindById(transItem.TransItemCatId);
                if (null != transCat)
                {

                    DateTime transInMonth = DateTime.ParseExact(transItem.TransItemForMonth, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                    DateTime today = DateTime.Now;
                    DateTime firstDateOfThisMounth = new DateTime(today.Year, today.Month, 1);
                    Transaction existedTrans = _transactionService.FindByMonthYear(transInMonth);
                    TransactionItem transItemE = new TransactionItem();

                    if (null != existedTrans && existedTrans.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSED)
                    {
                        response.StatusCode = -1;
                        response.Msg = "Khai báo thu chi đã đóng";
                        return Json(response);
                    }
                    else if (null != existedTrans && existedTrans.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN)
                    {
                        transItemE.TransactionId = existedTrans.Id;
                    }
                    else if (null == existedTrans && transInMonth.AddMonths(3) <= firstDateOfThisMounth && firstDateOfThisMounth.AddMonths(1) >= transInMonth)
                    {
                        Transaction newTransaction = new Transaction();
                        newTransaction.ForMonth = transInMonth;
                        newTransaction.Status = SLIM_CONFIG.BALANCE_SHEET_OPEN;
                        newTransaction.CreateDate = DateTime.Now;
                        newTransaction.LastModified = DateTime.Now;
                        newTransaction.TypeId = SLIM_CONFIG.TRANSACTION_TYPE_INCOME;
                        _transactionService.Add(newTransaction);
                        transItemE.TransactionId = newTransaction.Id;
                    }
                    transItemE.CategoryId = transCat.Id;
                    transItemE.TotalAmount = transItem.TransItemTotalAmount;
                    transItemE.PaidAmount = transItem.TransItemPaidAmount;
                    transItemE.Description = transItem.TransItemDesc;
                    transItemE.CreateDate = DateTime.Now;
                    transItemE.LastModified = DateTime.Now;
                    transItemE.Name = transItem.TransItemTitle;

                    _transItemService.Add(transItemE);
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
        public ActionResult UpdateTransactionItem(TransItemModel transItem)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {
                TransactionItem oldTransItem = _transItemService.FindById(transItem.TransItemId);

                if (null == oldTransItem)
                {
                    response.StatusCode = -1;
                    response.Msg = AmsConstants.MsgUserNotFound;
                    return Json(response);
                }
                else if (oldTransItem.Transaction.Status == SLIM_CONFIG.BALANCE_SHEET_CLOSED)
                {
                    response.StatusCode = -1;
                    response.Msg = "Khai báo thu chi đã đóng";
                    return Json(response);
                }
                TransactionItemCategory transCat = _transItemCategoryService.FindById(transItem.TransItemCatId);
                if (null != transCat)
                {

                    DateTime transInMonth = DateTime.ParseExact(transItem.TransItemForMonth, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                    DateTime today = DateTime.Now;
                    DateTime firstDateOfThisMounth = new DateTime(today.Year, today.Month, 1);
                    Transaction existedTrans = _transactionService.FindByMonthYear(transInMonth);

                    if (null != existedTrans && existedTrans.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN)
                    {
                        oldTransItem.TransactionId = existedTrans.Id;
                    }
                    else if (null == existedTrans && transInMonth.AddMonths(3) <= firstDateOfThisMounth || firstDateOfThisMounth.AddMonths(1) >= transInMonth)
                    {
                        Transaction newTransaction = new Transaction();
                        newTransaction.ForMonth = transInMonth;
                        newTransaction.Status = SLIM_CONFIG.BALANCE_SHEET_OPEN;
                        newTransaction.CreateDate = DateTime.Now;
                        newTransaction.LastModified = DateTime.Now;
                        newTransaction.TypeId = SLIM_CONFIG.TRANSACTION_TYPE_INCOME;
                        _transactionService.Add(newTransaction);
                        oldTransItem.TransactionId = newTransaction.Id;
                    }
                    oldTransItem.CategoryId = transCat.Id;
                    oldTransItem.TotalAmount = transItem.TransItemTotalAmount;
                    oldTransItem.PaidAmount = transItem.TransItemPaidAmount;
                    oldTransItem.Description = transItem.TransItemDesc;
                    oldTransItem.LastModified = DateTime.Now;
                    oldTransItem.Name = transItem.TransItemTitle;

                    _transItemService.Update(oldTransItem);
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

        [HttpGet]
        [Route("Management/BalanceSheet/GetAllTransactionType")]
        public ActionResult AddTransactionType(int type)
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {

                List<TransactionItemCategory> list = _transItemCategoryService.GetByTransType(type);
                List<ShortTransItemCatModel> modelList = new List<ShortTransItemCatModel>();
                ShortTransItemCatModel m = null;
                foreach (var i in list)
                {
                    m = new ShortTransItemCatModel();
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
        [Route("Management/BalanceSheet/GetIncomeTransItems")]
        public ActionResult GetIncomeTransItems()
        {
            MessageViewModels response = new MessageViewModels();
            User u = _userServices.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (u != null)
            {

                List<TransactionItem> list = _transItemService.GetByTransType(SLIM_CONFIG.TRANSACTION_TYPE_INCOME);
                List<TransItemModel> modelList = new List<TransItemModel>();
                TransItemModel m = null;
                foreach (var i in list)
                {
                    m = new TransItemModel();
                    m.TransItemCatName = i.TransactionItemCategory.Name;
                    m.TransItemId = i.Id;
                    m.TransItemForMonth = i.Transaction.ForMonth.Value.ToString("MM-yyyy");
                    m.TransItemTitle = i.Name;
                    m.TransItemTotalAmount = i.TotalAmount.Value;
                    m.TransItemPaidAmount = i.PaidAmount.Value;
                    m.TransItemCreateDate = i.CreateDate.Value.ToString(AmsConstants.DateFormat);
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

                TransactionItem transcItem = _transItemService.FindById(id);
                if (null != transcItem)
                {
                    if (transcItem.Transaction.Status != SLIM_CONFIG.BALANCE_SHEET_CLOSED)
                    {
                        TransItemModel model = new TransItemModel();
                        model.TransItemCatName = transcItem.TransactionItemCategory.Name;
                        model.TransItemCatId = transcItem.TransactionItemCategory.Id;
                        model.TransItemDesc = transcItem.Description;
                        model.TransItemForMonth = transcItem.Transaction.ForMonth.Value.ToString("MM-yyyy");
                        model.TransItemTitle = transcItem.Name;
                        model.TransItemTotalAmount = transcItem.TotalAmount.Value;
                        model.TransItemPaidAmount = transcItem.PaidAmount.Value;
                        model.TransItemId = transcItem.Id;

                        response.Data = model;
                    }
                    else
                    {
                        response.StatusCode = 5;
                        response.Msg = "Bảng thu chi đã đóng";
                    }
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
    }
}