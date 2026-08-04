using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HuniCafe.Models
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
            IsActive = true; // Mặc định khi tạo mới thì danh mục sẽ được hiển thị
        }
        [Key]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string CategoryDescription { get; set; }

        [Display(Name = "Trạng thái hiển thị")]
        public bool IsActive { get; set; }

        // Navigation Property
        public virtual ICollection<Product> Products { get; set; }
    }
}