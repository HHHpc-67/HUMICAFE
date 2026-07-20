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

      
        public ActionResult MainPage()
        {
            var model = new HomeViewModel
            {
                Categories = db.Categories.ToList(),
                Products = db.Products.ToList()
            };

            return View(model);
           
        }
    }
}