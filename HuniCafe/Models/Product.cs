using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HuniCafe.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200)]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Display(Name = "Giá bán")]
        public decimal Price { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả sản phẩm")]
        public string Description { get; set; }

        [Display(Name = "Hình ảnh")]
        public string Image { get; set; }
        public string ImagePublicId { get; set; }

        [Display(Name = "Đã xóa")]
        public bool IsDeleted { get; set; } = false;

        // Foreign Key & Navigation Property
        [Display(Name = "Danh mục")]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }// khóa ngoại để liên kết với bảng Category


    }
}