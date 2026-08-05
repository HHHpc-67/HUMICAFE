using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HuniCafe.Models
{
    public class Order
    {
        // Hàm khởi tạo (Constructor) để tự tạo mới danh sách OrderDetails khi tạo Đơn hàng
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public int OrderID { get; set; }

        public int UserID { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } //pending, done .....

        [ForeignKey("UserID")]
        public virtual Users User { get; set; }


        public string Address { get; set; }

        public string Phone { get; set; }

        // Navigation Property
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public int? CouponID { get; set; }

        public decimal DiscountAmount { get; set; }

        [ForeignKey("CouponID")]
        public virtual Coupon Coupon { get; set; }
    }
}