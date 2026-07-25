using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace HuniCafe.Models.ViewModel
{
    public class ProductViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public int? SelectedCategory { get; set; } // Thêm thuộc tính SelectedCategory để lưu trữ categoryId được chọn
    }
}