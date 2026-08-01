using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HuniCafe.Models.ViewModel
{
    public class CheckoutViewModel
    {
        public List<Cart> Cart { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Note { get; set; }

        public decimal TotalAmount { get; set; }
    }
}