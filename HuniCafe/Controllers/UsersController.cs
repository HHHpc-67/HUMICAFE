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
                    Session["User"] = user; // BỔ SUNG DÒNG NÀY: Lưu nguyên đối tượng user vào Session

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

        // GET: Users/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Users model, string confirmPassword)
        {
            // Loại bỏ kiểm tra Validation các trường không có trên Form
            ModelState.Remove("UserID");
            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                // Kiểm tra xem Username hoặc Email đã tồn tại trong DB chưa
                var checkUser = db.Users.FirstOrDefault(u => u.Username == model.Username || u.Email == model.Email);

                if (checkUser != null)
                {
                    if (checkUser.Username == model.Username)
                    {
                        ViewBag.Error = "Tên đăng nhập này đã được sử dụng!";
                    }
                    else
                    {
                        ViewBag.Error = "Email này đã được sử dụng!";
                    }
                    return View(model);
                }

                // Kiểm tra khớp mật khẩu
                if (model.Password != confirmPassword)
                {
                    ViewBag.Error = "Mật khẩu xác nhận không trùng khớp!";
                    return View(model);
                }

                // Gán Role mặc định
                model.Role = "Customer";

                // Lưu vào Database
                db.Users.Add(model);
                db.SaveChanges();

                // Chuyển hướng sang trang Login
                return RedirectToAction("Login", "Users");
            }

            ViewBag.Error = "Vui lòng kiểm tra lại thông tin nhập!";
            return View(model);
        }
    }
}