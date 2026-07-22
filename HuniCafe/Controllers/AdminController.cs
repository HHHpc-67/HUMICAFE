using HuniCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HuniCafe.Controllers
{
    public class AdminController : Controller
    {
        private HuniCafeDB db = new HuniCafeDB();

        // GET: Admin/Login
        [HttpGet]
        public ActionResult Login()
        {
            // Kiểm tra đồng bộ Session["User"] hoặc Session["AdminUser"]
            if (Session["User"] != null || Session["AdminUser"] != null)
            {
                return RedirectToAction("Admin");
            }
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            username = username?.Trim();
            password = password?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!";
                return View();
            }

            // Kiểm tra trực tiếp dữ liệu từ bảng AdminAccounts trong SQL
            var admin = db.AdminAccounts
                          .FirstOrDefault(a => a.Username == username &&
                                               a.Password == password &&
                                               a.IsActive == true);

            if (admin != null)
            {
                // Giữ lại Session cũ của bạn
                Session["AdminUser"] = admin.Username;
                Session["AdminName"] = admin.FullName;

                // THÊM 2 DÒNG NÀY ĐỂ TRANG CHỦ (LAYOUT CLIENT) NHẬN DẠNG ĐƯỢC
                Session["User"] = !string.IsNullOrEmpty(admin.FullName) ? admin.FullName : admin.Username;
                Session["Role"] = "Admin";

                return RedirectToAction("Admin");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            return View();
        }

        // Trang Admin Dashboard
        public ActionResult Admin()
        {
            if (Session["User"] == null && Session["AdminUser"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // GET: Admin/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("MainPage", "Home");
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