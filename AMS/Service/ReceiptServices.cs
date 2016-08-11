using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Models;
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
            if (receipt != null) _receiptRepository.Delete(receipt);
        }
        public List<Receipt> GetReceiptByHouseId(int houseId)
        {
            return _receiptRepository.List.Where(r => r.HouseId == houseId
                && DateTime.Today.Date >= r.PublishDate.Value.Date).OrderByDescending(r => r.CreateDate).ToList();
        }
        public List<Receipt> GetReceiptByHouseFromDateToDate(int houseId, DateTime from, DateTime to)
        {
            return _receiptRepository.List.Where(r => r.HouseId == houseId
                && from.Date <= r.PublishDate.Value.Date && to.Date >= r.PublishDate.Value.Date).OrderByDescending(r => r.CreateDate).ToList();
        }
        public List<Receipt> GetAllUnpaidReceip(int houseId)
        {
            return _receiptRepository.List.Where(r => r.HouseId == houseId && 
                (r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPAID || (r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED && r.PublishDate.Value.Date <= DateTime.Today.Date)))
                .OrderByDescending(r => r.CreateDate).ToList();
        }
        public List<Receipt> GetLastActivityReceipt(int houseId)
        {
            return _receiptRepository.List.Where(r => r.HouseId == houseId && !(r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED && r.PublishDate.Value.Date > DateTime.Today.Date))
                .OrderByDescending(r => r.LastModified).Take(10).ToList();
        }
        public Receipt GetAllReceiptFromTheLastPayment(int houseId)
        {
            List<Receipt> allPaidReceipt = _receiptRepository.List.Where(r => r.HouseId == houseId && r.Status == SLIM_CONFIG.RECEIPT_STATUS_PAID).OrderByDescending(r => r.PaymentDate).ToList();
            return allPaidReceipt.Count == 0 ? null : allPaidReceipt.First();
        }
        public List<Receipt> GetAllReceiptFromLastPaidReceipt(int houseId, DateTime lastPaymentDate)
        {
            List<Receipt> allPaidReceipt = _receiptRepository.List.Where(r => r.HouseId == houseId &&
                (r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPAID || r.PaymentDate.Value.Date >= lastPaymentDate.Date)).OrderByDescending(r => r.PaymentDate).ToList();
            return allPaidReceipt;
        }
        //        public List<Receipt> GetMonthlyReceiptByHouseId(int houseId, DateTime month)
        //        {
        //            DateTime firstDateOfThisMounth = new DateTime(month.Year, month.Month, 1);
        //            DateTime endDateOfThisMounth = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
        //            return _receiptRepository.List.Where(r => r.HouseId == houseId && r.PublishDate.Value.Date.Date >= firstDateOfThisMounth.Date
        //                && r.PublishDate.Value.Date.Date <= endDateOfThisMounth.Date).OrderByDescending(r => r.CreateDate).ToList();
        //        }

        //        public List<Receipt> GetAll(int houseId, DateTime month)
        //        {
        //            DateTime firstDateOfThisMounth = new DateTime(month.Year, month.Month, 1);
        //            DateTime endDateOfThisMounth = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
        //            return _receiptRepository.List.Where(r => r.HouseId == houseId && r.PublishDate.Value.Date.Date >= firstDateOfThisMounth.Date
        //                && r.PublishDate.Value.Date.Date <= endDateOfThisMounth.Date).OrderByDescending(r => r.CreateDate).ToList();
        //        }

        public List<Receipt> GetReceiptInMounth(DateTime month)
        {
            DateTime firstDateOfThisMounth = new DateTime(month.Year, month.Month, 1);
            DateTime endDateOfThisMounth = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
            return _receiptRepository.List.Where(r => r.PublishDate.Value.Date.Date >= firstDateOfThisMounth.Date && r.PublishDate.Value.Date.Date <= endDateOfThisMounth.Date).OrderByDescending(r => r.CreateDate).ToList();
        }

        //        public List<Receipt> GetReceiptInMonthFromOpeningToToday(BalanceSheet blSheet)
        //        {
        //            //            DateTime firstDateOfThisMounth = new DateTime(month.Year, month.Month, 1);
        //            //            DateTime endDateOfThisMounth = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
        //            //            return _receiptRepository.List.Where(r => r.PublishDate.Value.Date.Date >= firstDateOfThisMounth.Date && r.PublishDate.Value.Date.Date <= endDateOfThisMounth.Date).OrderByDescending(r => r.CreateDate).ToList();
        //
        //            return _receiptRepository.List.Where(r => r.PublishDate.Value.Date.Date >= blSheet.ForMonth.Value.Date &&
        //                r.PublishDate.Value.Date.Date <= DateTime.Today.Date).OrderByDescending(r => r.CreateDate).ToList();
        //        }

        //        public List<Receipt> GetReceiptInMonthWhileBalanceSheetOpen(BalanceSheet blSheet)
        //        {
        //            //            DateTime firstDateOfThisMounth = new DateTime(month.Year, month.Month, 1);
        //            //            DateTime endDateOfThisMounth = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
        //            //            return _receiptRepository.List.Where(r => r.PublishDate.Value.Date.Date >= firstDateOfThisMounth.Date && r.PublishDate.Value.Date.Date <= endDateOfThisMounth.Date).OrderByDescending(r => r.CreateDate).ToList();
        //
        //            return _receiptRepository.List.Where(r => r.PublishDate.Value.Date.Date >= blSheet.ForMonth.Value.Date &&
        //                r.PublishDate.Value.Date.Date <= blSheet.ClosingDate.Value.Date).OrderByDescending(r => r.CreateDate).ToList();
        //        }
        //        public List<Receipt> GetPaidReceiptOfBalanceSheetFromOpeningToToday(BalanceSheet bls)
        //        {
        //            return _receiptRepository.List.Where(r => r.PaymentDate != null && bls.ForMonth.Value.Date <= r.PaymentDate.Value.Date
        //                && DateTime.Today.Date >= r.PaymentDate.Value.Date.Date
        //                ).OrderByDescending(r => r.CreateDate).ToList();
        //        }
        //        public List<Receipt> GetPaidReceiptWhileBalanceSheetOpen(BalanceSheet bls)
        //        {
        //            return _receiptRepository.List.Where(r => r.PaymentDate != null && bls.ForMonth.Value.Date <= r.PaymentDate.Value.Date
        //                && bls.ClosingDate.Value.Date >= r.PaymentDate.Value.Date.Date
        //                ).OrderByDescending(r => r.CreateDate).ToList();
        //        }
        public List<Receipt> GetPublishedReceiptInMounth(DateTime month)
        {
            DateTime firstDateOfThisMounth = new DateTime(month.Year, month.Month, 1);
            DateTime endDateOfThisMounth = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
            return _receiptRepository.List.Where(r => r.Status != SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED
                && r.PublishDate.Value.Date.Date >= firstDateOfThisMounth.Date
                && r.PublishDate.Value.Date.Date <= endDateOfThisMounth.Date).ToList();
        }
        public List<Receipt> GetPaidReceiptInMounth(DateTime month)
        {
            DateTime firstDateOfThisMounth = new DateTime(month.Year, month.Month, 1);
            DateTime endDateOfThisMounth = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
            return _receiptRepository.List.Where(r => r.Status == SLIM_CONFIG.RECEIPT_STATUS_PAID
                && r.PublishDate.Value.Date.Date >= firstDateOfThisMounth.Date
                && r.PublishDate.Value.Date.Date <= endDateOfThisMounth.Date).ToList();
        }

        public List<Receipt> GetBatchReceiptByMonth(DateTime month)
        {
            return _receiptRepository.List.Where(r => r.PublishDate.Value.Date == month.Date.Date
                && r.PublishDate.Value.Month == month.Date.Month && r.IsBatch == SLIM_CONFIG.RECEIPT_TYPE_AUTOMATION).ToList();
        }

        public List<Receipt> GetAllReceipts()
        {
            return _receiptRepository.List.OrderByDescending(r => r.CreateDate.Value).ToList();
        }
        public List<Receipt> GetAllReceiptsForHouseGroupByCreateDate()
        {
            return _receiptRepository.List.Where(r => r.House != null).OrderByDescending(r => r.CreateDate.Value).GroupBy(r => r.CreateDate.Value).Select(r => r.First()).ToList();
        }
        public bool CheckAllAutomationReceiptIsPaid(DateTime createDate)
        {
            List<Receipt> receipts = _receiptRepository.List.Where(r => r.CreateDate == createDate).GroupBy(r => r.Status).Select(r => r.First()).ToList();
            if (receipts.Count != 0)
            {
                foreach (var r in receipts)
                {
                    if (r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPAID || r.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPAID)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public List<Receipt> GetReceiptsByCreateDate(DateTime createDate)
        {
            return _receiptRepository.List.Where(r => r.CreateDate == createDate).ToList();
        }
        public Receipt FindById(int id)
        {
            return _receiptRepository.FindById(id);
        }
        public Receipt GetLastAutomationReceiptOfHouse(int houseId)
        {
            List<Receipt> listReceipt = _receiptRepository.List.Where(
                r => r.HouseId == houseId && r.IsBatch == SLIM_CONFIG.RECEIPT_TYPE_AUTOMATION
                     && r.ReceiptDetails.Where(rd => rd.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                         .Count() != 0).OrderByDescending(r => r.PublishDate).ToList();
            return listReceipt.Count == 0 ? null : listReceipt.First();
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

        public List<ReceiptDetail> GetReceiptDetailByReceiptCreateDate(DateTime receiptCreateDate)
        {
            return
                _receiptDetailRepository.List.Where(rd => rd.Receipt.CreateDate.Value == receiptCreateDate)
                    .GroupBy(rd => rd.UtilityServiceId)
                    .Select(rd => rd.First())
                    .ToList();
        }
        public List<ReceiptDetail> GetReceiptDetailInBalanceSheetGroupByUtlSrvCat(int blsId)
        {
            return
                _receiptDetailRepository.List.Where(rd => rd.Receipt.BalanceSheet.Id == blsId)
                    .GroupBy(rd => rd.UtilityService.UtilServiceCategory.Id).Select(rd => rd.First()).ToList();
        }

        public List<ReceiptDetail> Get12ReceiptFromThisReceipt(int houseId, DateTime publishDate)
        {
            return
                _receiptDetailRepository.List.Where(rd => rd.Receipt.HouseId == houseId && rd.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER && rd.Receipt.PublishDate.Value.Date <= publishDate.Date)
                .OrderBy(rd => rd.Receipt.PublishDate).Take(12).ToList();
        }
        //        public  GetReceiptsByUtilCatInMonth(int blsId, int utilSrvCatId)
        //        {
        //            return _receiptDetailRepository.List.Where(
        //                rd => rd.Receipt.BlsId == blsId && rd.UtilityServiceId == utilSrvCatId)
        //                .GroupBy(rd => rd.Receipt.Title)
        //                .Select(rd => new {Receipt = rd.First(), totalAmount = rd.Sum(rdTotal => rdTotal.Total), totalPaid = rd.Sum(rdPaid => rdPaid.Transactions.)}).ToList();
        //        } 

    }
}