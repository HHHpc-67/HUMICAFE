using HuniCafe.Models;
using HuniCafe.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

using System.Web.Mvc;

namespace HuniCafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly HuniCafeDB db = new HuniCafeDB();

      
        public ActionResult MainPage()
        {
            var model = new HomeViewModel
            {
                Categories = db.Categories.ToList(),
                Products = db.Products.ToList()
            };
            return View(model);
        }
        // Action xem chi tiết sản phẩm cho trang khách
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }
    }
}