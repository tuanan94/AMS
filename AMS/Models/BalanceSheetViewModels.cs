using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class BalanceSheetViewModels
    {
    }

    public class TransCategoryModel
    {
        public int TransCategoryId { get; set; }
        public string TransCategoryName { get; set; }
        public string DT_RowId { get; set; }
    }

    public class ShortTransCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TransactionModel
    {
        public int BalanceSheetId { get; set; }
        public int ReceiptId { get; set; }
        public int TransactionId { get; set; }
        public string UtilSrvCatName { get; set; }
        public int UtilSrvCatId { get; set; }
        public int UtilSrvId { get; set; }
        public int TransType { get; set; }
        public string TransStartDate { get; set; }
        public double TransTotalAmount { get; set; }
        public double TransPaidAmount { get; set; }
        public double TransPaidInMonthAmount { get; set; }
        public double TransUnpaidAmount { get; set; }
        public string TransTitle { get; set; }
        public string TransDesc { get; set; }
        public string TransCreateDate { get; set; }
        public string DT_RowId { get; set; }
        public int TransEditable { get; set; }
    }

    public class AmountGroupByTransCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double TotalAmount { get; set; }
        public double PaidAmount { get; set; }
        public double UnpaidAmount { get; set; }
    }

    public class BalanceSheetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double TotalIncome { get; set; }
        public double TotalExpense { get; set; }
        public double TotalIncomeInCash { get; set; }
        public double TotalExpenseInCash { get; set; }
        public double RedundancyStartMonth { get; set; }
        public double RedundancyEndMonth { get; set; }
        public string StartDate { get; set; }
        public string CreateDate { get; set; }
        public string Creator { get; set; }
        public int Status { get; set; }
        public string DT_RowId { get; set; }

    }
    public class CloseBalanceSheetModel
    {
        public int ThisMonthId { get; set; }
        public string NextMonthTitle { get; set; }
    }

    public class BalanceSheetProcessingModel
    {
        public double ForecastIncome { get; set; }
        public double ActualIncome { get; set; }
        public double ActualExpense { get; set; }
        public double ForecastExpense { get; set; }
        public double RedudancyStartMonth { get; set; }
        public double RedudancyEndMonth { get; set; }
        public string LastModified { get; set; }
        public List<TransactionModel> TotalTransactionList;
        public List<TransactionModel> IncomeList;
        public List<TransactionModel> ExpenseList;
    }
    public class UtilSrvCatTransaction
    {
        public Transaction Transaction { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPaid { get; set; }

        public UtilSrvCatTransaction(Transaction transaction, double totalAmount, double totalPaid)
        {
            Transaction = transaction;
            TotalAmount = totalAmount;
            TotalPaid = totalPaid;
        }
    }
    
}