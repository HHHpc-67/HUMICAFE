using HuniCafe.Models;
using HuniCafe.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace HuniCafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly HuniCafeDB db = new HuniCafeDB();
        List<Product> featuredProducts;

        public ActionResult MainPage()
        {

            // Nếu chưa có đơn hàng thì lấy 4 sản phẩm mới nhất
            if (!db.OrderDetails.Any())
            {
                featuredProducts = db.Products
                                     .OrderByDescending(p => p.ProductID)
                                     .Take(5)
                                     .ToList();
            }
            else
            {
                featuredProducts = db.OrderDetails
                    .GroupBy(od => od.ProductID)
                    .OrderByDescending(g => g.Sum(x => x.Quantity))
                    .Select(g => g.FirstOrDefault().Product)
                    .Take(4)
                    .ToList();
            }


            var model = new HomeViewModel
            {
                Categories = db.Categories.ToList(),
                Products = db.Products.ToList(),
                FeaturedProducts = featuredProducts
            };

            return View(model);
           
        }
    }
}