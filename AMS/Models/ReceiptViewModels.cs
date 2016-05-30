using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class ReceiptViewModels
    {
    }

    public class ReceiptModel
    {
        public int ReceiptId { get; set; }
        public string ReceiptTitle { get; set; }
        public string ReceiptDesc { get; set; }
        public string ReceiptHouseName { get; set; }
        public int Creator { get; set; }
        public List<OrderItemModel> ListItem { get; set; }
    }

    public class OrderItemModel
    {
        public string Name { get; set; }
        public float UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}