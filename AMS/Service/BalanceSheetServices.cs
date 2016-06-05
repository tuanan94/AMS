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

    public class TransactionService
    {
        GenericRepository<Transaction> _trasactionRepository = new GenericRepository<Transaction>();

        public Transaction FindById(int id)
        {
            return _trasactionRepository.FindById(id);
        }
        public void Add(Transaction transaction)
        {
            _trasactionRepository.Add(transaction);
        }
        public void Update(Transaction transaction)
        {
            _trasactionRepository.Update(transaction);
        }
        public Transaction FindByMonthYear(DateTime monthYear)
        {
            IEnumerable<Transaction> listTrans = _trasactionRepository.List.Where(r => r.ForMonth.Value != null &&
                                                   r.ForMonth.Value.Month == monthYear.Month &&
                                                   r.ForMonth.Value.Year == monthYear.Year).ToList();
            return listTrans.Count() == 0 ? null : listTrans.First();
        }
    }

    public class TransItemCategoryService
    {
        GenericRepository<TransactionItemCategory> _transItemCatRepository = new GenericRepository<TransactionItemCategory>();

        public TransactionItemCategory FindById(int id)
        {
            return _transItemCatRepository.FindById(id);
        }
        public List<TransactionItemCategory> GetByTransType(int type)
        {
            return _transItemCatRepository.List.Where(e => e.Type == type).ToList();
        }
        public void Add(TransactionItemCategory transaction)
        {
            _transItemCatRepository.Add(transaction);
        }
        public void Update(TransactionItemCategory transaction)
        {
            _transItemCatRepository.Update(transaction);
        }
    }

    public class TransItemService
    {
        GenericRepository<TransactionItem> _transItemRepository = new GenericRepository<TransactionItem>();

        public TransactionItem FindById(int id)
        {
            return _transItemRepository.FindById(id);
        }
        public void Add(TransactionItem transactionItem)
        {
            _transItemRepository.Add(transactionItem);
        }
        public void Update(TransactionItem transactionItem)
        {
            _transItemRepository.Update(transactionItem);
        }
        public List<TransactionItem> GetByTransType(int type)
        {
            return
                _transItemRepository.List.Where(
                    tr => tr.TransactionItemCategory.Type == type).ToList();
        }
    }
}