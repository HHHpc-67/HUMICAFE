using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HuniCafe.Models
{
    public class Category
    {
        // Hàm khởi tạo (Constructor) giúp tự động tạo mới danh sách sản phẩm tránh lỗi null
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [StringLength(100)]
        [DataType(DataType.MultilineText)]
        public string CategoryDescription { get; set; }

        // Navigation Property
        public virtual ICollection<Product> Products { get; set; }
    }
}