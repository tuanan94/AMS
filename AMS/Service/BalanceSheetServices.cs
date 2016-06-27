using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class BalanceSheetServices
    {
    }

    public class BalanceSheetService
    {
        GenericRepository<BalanceSheet> _trasactionRepository = new GenericRepository<BalanceSheet>();

        public List<BalanceSheet> GetAllBalanceSheets()
        {
            return _trasactionRepository.List.OrderByDescending(b => b.Id).ToList();
        }
        public bool hasBalanceSheet()
        {
            IEnumerable<BalanceSheet> listTrans = _trasactionRepository.List.ToList();
            return listTrans.Count() == 0 ? false : true;
        }
        public BalanceSheet FindById(int id)
        {
            return _trasactionRepository.FindById(id);
        }
        public void Add(BalanceSheet transaction)
        {
            _trasactionRepository.Add(transaction);
        }
        public void Update(BalanceSheet transaction)
        {
            _trasactionRepository.Update(transaction);
        }

        //        public BalanceSheet CheckBalanceSheetIsExisted(DateTime blsForMonth)
        //        {
        //            IEnumerable<BalanceSheet> listTrans = _trasactionRepository.List.Where(r => r.CreateDate.Value != null && r.CreateDate.Value.Date == blsForMonth).ToList();
        //            return listTrans.Count() == 0 ? null : listTrans.First();
        //        }

        //        public BalanceSheet FindByMonthYear(DateTime monthYear)
        //        {
        //            IEnumerable<BalanceSheet> listTrans = _trasactionRepository.List.Where(r => r.CreateDate.Value != null &&
        //                                                   r.CreateDate.Value.Month == monthYear.Month &&
        //                                                   r.CreateDate.Value.Year == monthYear.Year).ToList();
        //            return listTrans.Count() == 0 ? null : listTrans.First();
        //        }

        public BalanceSheet GetBalanceSheetForMonth(DateTime thisMonth)
        {
            IEnumerable<BalanceSheet> balanceSheets = _trasactionRepository.List.Where(r => r.StartDate.Value.Date.Month == thisMonth.Month
                && r.StartDate.Value.Date.Year == thisMonth.Year).ToList();
            return balanceSheets.Count() == 0 ? null : balanceSheets.ToList().First();
        }
        public BalanceSheet GetCurentActivateBalanceSheet()
        {
            List<BalanceSheet> balanceSheets = _trasactionRepository.List.Where(blSheet => blSheet.Status == SLIM_CONFIG.BALANCE_SHEET_OPEN).ToList();
            return balanceSheets.Count == 0 ? null : balanceSheets.ToList().First();
        }
    }

    public class UtilityServiceCateoryService
    {
        GenericRepository<UtilServiceCategory> _utilServiceCategoryRepository = new GenericRepository<UtilServiceCategory>();

        public UtilServiceCategory FindById(int id)
        {
            return _utilServiceCategoryRepository.FindById(id);
        }

        public List<UtilServiceCategory> GetAllEnable()
        {
            return _utilServiceCategoryRepository.List.Where(utilSrvCat => utilSrvCat.Status == SLIM_CONFIG.TRANS_CAT_STATUS_ENABLE).ToList();
        }
        public List<UtilServiceCategory> GetAllMandatory()
        {
            return _utilServiceCategoryRepository.List.Where(utilSrvCat => utilSrvCat.Status == SLIM_CONFIG.TRANS_CAT_STATUS_DENY_REMOVE).ToList();
        }

        public void Add(UtilServiceCategory utilSrvCat)
        {
            _utilServiceCategoryRepository.Add(utilSrvCat);
        }
        public void Update(UtilServiceCategory utilSrvCat)
        {
            _utilServiceCategoryRepository.Update(utilSrvCat);
        }
        public void Delete(UtilServiceCategory utilSrvCat)
        {
            _utilServiceCategoryRepository.Delete(utilSrvCat);
        }
    }

    public class TransactionService
    {
        GenericRepository<Transaction> _transactionRepository = new GenericRepository<Transaction>();

        public Transaction FindById(int id)
        {
            return _transactionRepository.FindById(id);
        }
        public void Add(Transaction transactionItem)
        {
            _transactionRepository.Add(transactionItem);
        }
        public void Delete(Transaction transactionItem)
        {
            _transactionRepository.Delete(transactionItem);
        }
        public void DeleteById(int id)
        {
            Transaction trans = _transactionRepository.FindById(id);
            if (trans != null)
            {
                _transactionRepository.Delete(trans);
            }
        }
        public void Update(Transaction transactionItem)
        {
            _transactionRepository.Update(transactionItem);
        }
        public List<Transaction> GetByTransType()
        {
            return
                _transactionRepository.List.OrderByDescending(tr => tr.BalanceSheet.StartDate).ToList();
        }

        public List<Transaction> GetAllTransactionHaveSameOrderCreateDate(DateTime orderCreateDate)
        {
            return _transactionRepository.List.Where(
                    trans => trans.ReceiptDetail != null && trans.ReceiptDetail.Receipt.CreateDate.Value == orderCreateDate).ToList();
        }

        public List<Transaction> GetTransOfReceipt(int receiptId)
        {
            return _transactionRepository.List.Where(trans => trans.ReceiptDetail.Receipt.Id == receiptId).ToList();
        }

        public void DeleteRangeByReceiptId(int receitpDetailId)
        {
            _transactionRepository.DeleteRange(_transactionRepository.List.Where(trans => trans.ReceiptDetailId == receitpDetailId).ToList());
        }
    }
}