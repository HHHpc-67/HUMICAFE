using HuniCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HuniCafe.Controllers
{
    public class UsersController : Controller
    {
        private HuniCafeDB db = new HuniCafeDB();

        // GET: Users/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string account, string password)
        {
            if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(password))
            {
                // Kiểm tra tài khoản khớp với Email HOẶC Username
                var user = db.Users.FirstOrDefault(u =>
                    (u.Email == account || u.Username == account) && u.Password == password
                );

                if (user != null)
                {
                    Session["UserID"] = user.UserID;
                    Session["FullName"] = user.FullName;
                    Session["Role"] = user.Role;

                    if (user.Role == "Admin")
                    {
                        // Chuyển hướng sang Action Admin trong UsersController
                        return RedirectToAction("Admin", "Users");
                    }
                    else
                    {
                        return RedirectToAction("MainPage", "Home");
                    }
                }

                ViewBag.Error = "Tài khoản hoặc Mật khẩu không chính xác!";
            }
            else
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
            }

            return View();
        }

        // GET: Users/Admin (Trang Quản trị)
        public ActionResult Admin()
        {
            // Kiểm tra phân quyền: Phải là Admin mới được vào
            if (Session["Role"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            return View(); 
        }

        // GET: Users/Logout
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa sạch Session đăng nhập
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