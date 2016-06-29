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

    //public class BalanceSheetService
    //{
    //    GenericRepository<BalanceSheet> _trasactionRepository = new GenericRepository<BalanceSheet>();

    //    public List<BalanceSheet> GetAllBalanceSheets()
    //    {
    //        return _trasactionRepository.List.OrderByDescending(b => b.Id).ToList();
    //    }
    //    public bool hasBalanceSheet()
    //    {
    //        IEnumerable<BalanceSheet> listTrans = _trasactionRepository.List.ToList();
    //        return listTrans.Count() == 0 ? false : true;
    //    }
    //    public BalanceSheet FindById(int id)
    //    {
    //        return _trasactionRepository.FindById(id);
    //    }
    //    public void Add(BalanceSheet transaction)
    //    {
    //        _trasactionRepository.Add(transaction);
    //    }
    //    public void Update(BalanceSheet transaction)
    //    {
    //        _trasactionRepository.Update(transaction);
    //    }

    //    public BalanceSheet CheckBalanceSheetIsExisted(DateTime blsForMonth)
    //    {
    //        IEnumerable<BalanceSheet> listTrans = _trasactionRepository.List.Where(r => r.ForMonth.Value != null && r.ForMonth.Value.Date == blsForMonth ).ToList();
    //        return listTrans.Count() == 0 ? null : listTrans.First();
    //    }

    //    public BalanceSheet FindByMonthYear(DateTime monthYear)
    //    {
    //        IEnumerable<BalanceSheet> listTrans = _trasactionRepository.List.Where(r => r.ForMonth.Value != null &&
    //                                               r.ForMonth.Value.Month == monthYear.Month &&
    //                                               r.ForMonth.Value.Year == monthYear.Year).ToList();
    //        return listTrans.Count() == 0 ? null : listTrans.First();
    //    }

    //    public BalanceSheet GetBalanceSheetForMonth(DateTime thisMonth)
    //    {
    //        IEnumerable<BalanceSheet> balanceSheets = _trasactionRepository.List.Where(r => r.ForMonth.Value.Date.Month == thisMonth.Month
    //            && r.ForMonth.Value.Date.Year == thisMonth.Year).ToList();
    //        return balanceSheets.Count() == 0 ? null : balanceSheets.ToList().First();
    //    }
    //}

    //public class TransactionCategoryService
    //{
    //    GenericRepository<TransactionCategory> _transCatRepository = new GenericRepository<TransactionCategory>();

    //    public TransactionCategory FindById(int id)
    //    {
    //        return _transCatRepository.FindById(id);
    //    }
    //    public List<TransactionCategory> GetAll()
    //    {
    //        return _transCatRepository.List.ToList();
    //    }
    //    public void Add(TransactionCategory transaction)
    //    {
    //        _transCatRepository.Add(transaction);
    //    }
    //    public void Update(TransactionCategory transaction)
    //    {
    //        _transCatRepository.Update(transaction);
    //    }
    //    public void Delete(TransactionCategory transaction)
    //    {
    //        _transCatRepository.Delete(transaction);
    //    }
    //}

    //public class TransactionService
    //{
    //    GenericRepository<Transaction> _transactionRepository = new GenericRepository<Transaction>();

    //    public Transaction FindById(int id)
    //    {
    //        return _transactionRepository.FindById(id);
    //    }
    //    public void Add(Transaction transactionItem)
    //    {
    //        _transactionRepository.Add(transactionItem);
    //    }
    //    public void Delete(Transaction transactionItem)
    //    {
    //        _transactionRepository.Delete(transactionItem);
    //    }
    //    public void Update(Transaction transactionItem)
    //    {
    //        _transactionRepository.Update(transactionItem);
    //    }
    //    public List<Transaction> GetByTransType()
    //    {
    //        return
    //            _transactionRepository.List.OrderByDescending(tr => tr.BalanceSheet.ForMonth).ToList();
    //    }
    //}
}