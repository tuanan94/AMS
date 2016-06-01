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
        public void Update(Receipt receipt)
        {
            _receiptRepository.Update(receipt);
        }
        public void Delete(Receipt receipt)
        {
            _receiptRepository.Delete(receipt);
        }
        public void DeleteById(int id)
        {
            Receipt receipt = _receiptRepository.FindById(id);
            if(receipt != null) _receiptRepository.Delete(receipt);
        }
        public List<Receipt> GetReceiptByHouseId(int houseId)
        {
            return _receiptRepository.List.Where(r => r.HouseId == houseId && r.Status != SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED).OrderByDescending(r => r.CreateDate).ToList();
        }

        public List<Receipt> GetAllReceipts()
        {
            return _receiptRepository.List.OrderByDescending(r => r.CreateDate).ToList();
        }

        public Receipt FindById(int id)
        {
            return _receiptRepository.List.Where(r => r.Id == id).First();
        }

    }
    public class ReceiptDetailServices
    {
        public GenericRepository<ReceiptDetail> _receiptDetailRepository = new GenericRepository<ReceiptDetail>();

        public ReceiptDetail FindById(int id)
        {
           return _receiptDetailRepository.FindById(id);
        }

        public void Add(ReceiptDetail receiptDetail)
        {
            _receiptDetailRepository.Add(receiptDetail);
        }
        public void Update(ReceiptDetail receiptDetail)
        {
            _receiptDetailRepository.Update(receiptDetail);
        }

        public void Delete(ReceiptDetail receiptDetail)
        {
            _receiptDetailRepository.Delete(receiptDetail);
        }

        public void DeleteById(int id)
        {
            ReceiptDetail e = _receiptDetailRepository.FindById(id);
            if (e != null)
            {
            _receiptDetailRepository.Delete(e);
            }
        }
    }
}