using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HuniCafe.Models.ViewModel
{
    public class CheckoutViewModel
    {
        public List<Cart> Cart { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email")]
      
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 số và bắt đầu từ số 0")]
        public string Phone { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        public string Address { get; set; }

        public string Note { get; set; }

        public decimal TotalAmount { get; set; }
    }
}