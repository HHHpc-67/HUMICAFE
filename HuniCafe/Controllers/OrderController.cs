using HuniCafe.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HuniCafe.Controllers
{
    public class OrderController : Controller
    {
        private readonly HuniCafeDB db = new HuniCafeDB();

        // 1. ADMIN: Xem toàn bộ đơn hàng
        public ActionResult AdminOrders(string status)
        {
            var orders = db.Orders.Include(o => o.User).AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.Status == status);
            }

            ViewBag.CurrentStatus = status;
            return View(orders.OrderByDescending(o => o.OrderDate).ToList());
        }

        // 2. ADMIN: Cập nhật trạng thái đơn
        [HttpPost]
        public ActionResult UpdateStatus(int orderId, string status)
        {
            var order = db.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                db.SaveChanges();
            }
            return RedirectToAction("AdminOrders");
        }

        // 3. KHÁCH HÀNG: Lịch sử đơn hàng cá nhân
        public ActionResult MyOrders()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            int userId = (int)Session["UserID"];

            var orders = db.Orders
                           .Where(x => x.UserID == userId)
                           .OrderByDescending(x => x.OrderDate)
                           .ToList();

            return View(orders);
        }

        // 4. CHUNG: Xem chi tiết đơn hàng
        public ActionResult Details(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var order = db.Orders
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}