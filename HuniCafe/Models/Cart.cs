using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HuniCafe.Models
{
    public class Cart
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total
        {
            get
            {
                return Price * Quantity;
            }
        }
        public string Image { get; set; }
    }
}