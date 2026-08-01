using HuniCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace HuniCafe.Controllers
{
    public class OrderController : Controller
    {
        private readonly HuniCafeDB db = new HuniCafeDB();

        public ActionResult MyOrders()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserID"];

            var orders = db.Orders
                           .Where(x => x.UserID == userId)
                           .OrderByDescending(x => x.OrderDate)
                           .ToList();

            return View(orders);
        }




        public ActionResult Details(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserID"];

            var order = db.Orders
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o => o.OrderID == id && o.UserID == userId);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }
    }
}