using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class ReceiptServices
    {
        public GenericRepository<Receipt> _receiptRepository = new GenericRepository<Receipt>();
        public void Add(Receipt receipt)
        {
            _receiptRepository.Add(receipt);
        }
        public List<Receipt> FindByHouseId(int houseId)
        {
            return _receiptRepository.List.Where(r => r.HouseId == houseId && r.Status != SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED).OrderByDescending(r => r.CreateDate).ToList();
        }
        public Receipt FindById(int id)
        {
            return _receiptRepository.List.Where(r => r.Id == id).First();
        }

    }
    public class ReceiptDetailServices
    {
        public GenericRepository<ReceiptDetail> _receiptDetailRepository = new GenericRepository<ReceiptDetail>();
        public void Add(ReceiptDetail receiptDetail)
        {
            _receiptDetailRepository.Add(receiptDetail);
        }

    }
}