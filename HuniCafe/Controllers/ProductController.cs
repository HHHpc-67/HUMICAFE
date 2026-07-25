using HuniCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HuniCafe.Models.ViewModel;

namespace HuniCafe.Controllers
{
    public class ProductController : Controller
    {
        //khoi tao
        private readonly HuniCafeDB db = new HuniCafeDB();
        // HIển thị danh sách sản phẩm, có thể lọc theo categoryId nếu được truyền vào
        public ActionResult Index(int? categoryId)
        {
            var products = db.Products.AsQueryable(); // Lấy tất cả sản phẩm từ cơ sở dữ liệu

            if (categoryId.HasValue) // Nếu có categoryId được truyền vào, lọc sản phẩm theo categoryId
            {
                products = products.Where(p => p.CategoryID == categoryId.Value); // Lọc sản phẩm theo categoryId
            }

            var model = new ProductViewModel
            {
                Products = products.ToList(),
                Categories = db.Categories.ToList(),
                SelectedCategory = categoryId
            };

            return View(model);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////// Admin làm phía dưới này 
        ///


        public ActionResult Product_Admin()
        {
            return View(db.Products.ToList());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
    }
}