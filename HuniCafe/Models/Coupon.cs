using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HuniCafe.Models
{
    [Table("Coupons")]
    public class Coupon
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        [StringLength(20)]
        [Display(Name = "Mã giảm giá")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [StringLength(200)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Loại giảm giá")]
        public string DiscountType { get; set; } // "Percentage" hoặc "Fixed"

        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm")]
        [Display(Name = "Giá trị giảm")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đơn hàng tối thiểu")]
        [Display(Name = "Đơn hàng tối thiểu")]
        public decimal MinimumOrderValue { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Display(Name = "Số lượng mã")]
        public int Quantity { get; set; }

        [Display(Name = "Đã sử dụng")]
        public int UsedQuantity { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; }
    }
}