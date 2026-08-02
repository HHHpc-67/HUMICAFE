using HuniCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HuniCafe.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HuniCafeDB db = new HuniCafeDB();

        // 1. Trang thông tin tài khoản
        public ActionResult CustomerProfile()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.Users.FirstOrDefault(u => u.UserID == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View(user);
        }

        // 2. Trang danh sách đơn hàng của khách hàng
        public ActionResult MyOrders()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            var orders = db.Orders.Where(o => o.UserID == userId)
                                  .OrderByDescending(o => o.OrderDate)
                                  .ToList();

            return View(orders);
        }

        // 3. Trang đổi mật khẩu (GET)
        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }

        // POST: Đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.Users.FirstOrDefault(u => u.UserID == userId);

            if (user != null)
            {
                if (user.Password != currentPassword)
                {
                    ViewBag.Error = "Mật khẩu hiện tại không chính xác!";
                    return View();
                }

                if (newPassword != confirmPassword)
                {
                    ViewBag.Error = "Mật khẩu xác nhận không trùng khớp!";
                    return View();
                }

                user.Password = newPassword;
                db.SaveChanges();

                ViewBag.Success = "Đổi mật khẩu thành công!";
            }

            return View();
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