using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HuniCafe.Models
{
    public class CartItem
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Total
        {
            get
            {
                return Price * Quantity;
            }
        }
    }
}