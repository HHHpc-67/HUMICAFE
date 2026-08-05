using HuniCafe.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace HuniCafe.Controllers
{
    public class UuDaiController : Controller
    {
        private HuniCafeDB db = new HuniCafeDB();
        //Admin - Quản lý mã giảm giá
        // GET: UuDai - Danh sách mã giảm giá
        public ActionResult CouponAdmin(string searchString, string statusFilter)
        {
            if (Session["Role"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            var coupons = db.Coupons.AsQueryable();

            // Tìm kiếm theo mã hoặc mô tả
            if (!string.IsNullOrEmpty(searchString))
            {
                coupons = coupons.Where(c => c.Code.Contains(searchString) ||
                                           c.Description.Contains(searchString));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(statusFilter))
            {
                var now = DateTime.Now.Date;
                switch (statusFilter)
                {
                    case "active":
                        coupons = coupons.Where(c => c.IsActive &&
                                                   now >= c.StartDate &&
                                                   now <= c.EndDate &&
                                                   c.UsedQuantity < c.Quantity);
                        break;
                    case "inactive":
                        coupons = coupons.Where(c => !c.IsActive ||
                                                   now < c.StartDate ||
                                                   now > c.EndDate ||
                                                   c.UsedQuantity >= c.Quantity);
                        break;
                    case "expired":
                        coupons = coupons.Where(c => now > c.EndDate);
                        break;
                    case "not_started":
                        coupons = coupons.Where(c => now < c.StartDate);
                        break;
                    case "used_up":
                        coupons = coupons.Where(c => c.UsedQuantity >= c.Quantity);
                        break;
                }
            }

            coupons = coupons.OrderByDescending(c => c.CreatedDate);

            ViewBag.SearchString = searchString;
            ViewBag.StatusFilter = statusFilter;

            return View(coupons.ToList());
        }

        // GET: UuDai/Create
        public ActionResult Create()
        {
            if (Session["Role"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }
            return View();
        }

        // POST: UuDai/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,Description,DiscountType,DiscountValue,MinimumOrderValue,Quantity,StartDate,EndDate,IsActive")] Coupon coupon)
        {
            if (Session["Role"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                if (db.Coupons.Any(c => c.Code == coupon.Code))
                {
                    ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại!");
                    return View(coupon);
                }

                coupon.CreatedDate = DateTime.Now;
                coupon.UsedQuantity = 0;
                db.Coupons.Add(coupon);
                db.SaveChanges();

                TempData["Success"] = "Tạo mã giảm giá mới thành công!";
                return RedirectToAction("CouponAdmin");
            }

            return View(coupon);
        }

        // GET: UuDai/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Role"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }

            return View(coupon);
        }

        // POST: UuDai/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Description,DiscountType,DiscountValue,MinimumOrderValue,Quantity,UsedQuantity,StartDate,EndDate,IsActive")] Coupon coupon)
        {
            if (Session["Role"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                // 1. Kiểm tra trùng mã với các Voucher khác
                if (db.Coupons.Any(c => c.Code == coupon.Code && c.ID != coupon.ID))
                {
                    ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại!");
                    return View(coupon);
                }

                // 2. Tìm bản ghi gốc trong CSDL
                var existingCoupon = db.Coupons.Find(coupon.ID);
                if (existingCoupon == null)
                {
                    return HttpNotFound();
                }

                // 3. Cập nhật các trường thông tin
                existingCoupon.Code = coupon.Code;
                existingCoupon.Description = coupon.Description;
                existingCoupon.DiscountType = coupon.DiscountType;
                existingCoupon.DiscountValue = coupon.DiscountValue;
                existingCoupon.MinimumOrderValue = coupon.MinimumOrderValue;
                existingCoupon.Quantity = coupon.Quantity;
                existingCoupon.UsedQuantity = coupon.UsedQuantity;
                existingCoupon.IsActive = coupon.IsActive;

                // 4. Kiểm tra và gán ngày tháng chuẩn xác (Tránh bị MinValue 0001-01-01 gây lỗi SQL datetime)
                existingCoupon.StartDate = coupon.StartDate != DateTime.MinValue ? coupon.StartDate : existingCoupon.StartDate;
                existingCoupon.EndDate = coupon.EndDate != DateTime.MinValue ? coupon.EndDate : existingCoupon.EndDate;

                // 5. Lưu vào Database
                db.SaveChanges();

                TempData["Success"] = "Cập nhật mã giảm giá thành công!";
                return RedirectToAction("CouponAdmin");
            }

            return View(coupon);
        }

        // GET: UuDai/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Role"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }

            return View(coupon);
        }

        // POST: UuDai/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Role"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            Coupon coupon = db.Coupons.Find(id);
            db.Coupons.Remove(coupon);
            db.SaveChanges();

            TempData["Success"] = "Xóa mã giảm giá thành công!";
            return RedirectToAction("Index");
        }

        // GET: UuDai - Trang xem ưu đãi cho Khách
        public ActionResult UuDai(string searchString)
        {
            var activeCoupons = db.Coupons
                .Where(c => c.IsActive && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now && c.UsedQuantity < c.Quantity);

            if (!string.IsNullOrEmpty(searchString))
            {
                activeCoupons = activeCoupons.Where(c => c.Code.Contains(searchString) ||
                                                        c.Description.Contains(searchString));
            }

            activeCoupons = activeCoupons.OrderBy(c => c.MinimumOrderValue);

            ViewBag.SearchString = searchString;
            return View(activeCoupons.ToList());
        }
        [HttpPost]
        public JsonResult ApplyCoupon(string couponCode)
        {
            if (string.IsNullOrEmpty(couponCode))
            {
                Session["CouponCode"] = null;
                Session["DiscountAmount"] = 0m;
                return Json(new { success = true, message = "Đã bỏ áp dụng mã." });
            }

            var coupon = db.Coupons.FirstOrDefault(c => c.Code == couponCode && c.IsActive);

            // 1. Kiểm tra mã tồn tại
            if (coupon == null)
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ!" });

            // 2. Kiểm tra hạn sử dụng
            if (DateTime.Now < coupon.StartDate || DateTime.Now > coupon.EndDate)
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn hoặc chưa đến đợt!" });

            // 3. Kiểm tra số lượng
            if (coupon.UsedQuantity >= coupon.Quantity)
                return Json(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng!" });

            // Tính tổng tiền giỏ hàng hiện tại
            List<Cart> cart = Session["Cart"] as List<Cart>;
            decimal totalAmount = cart != null ? cart.Sum(x => x.Price * x.Quantity) : 0; // 📌 Đã sửa UnitPrice -> Price

            // 4. Kiểm tra đơn hàng tối thiểu
            if (totalAmount < coupon.MinimumOrderValue)
                return Json(new { success = false, message = $"Đơn hàng phải từ {coupon.MinimumOrderValue.ToString("N0")} VNĐ để áp dụng mã này!" }); // 📌 Đã sửa cú pháp ToString("N0")

            // 5. Tính số tiền giảm
            decimal discount = 0;
            if (coupon.DiscountType == "Percentage")
            {
                discount = totalAmount * (coupon.DiscountValue / 100m);
            }
            else
            {
                discount = coupon.DiscountValue;
            }

            // Lưu thông tin giảm giá vào Session
            Session["CouponCode"] = coupon.Code;
            Session["DiscountAmount"] = discount;

            return Json(new { success = true, message = "Áp dụng mã giảm giá thành công!" });
        }

        // POST: UuDai/ValidateCoupon - API AJAX Kiểm tra mã khi thanh toán
        [HttpPost]
        public JsonResult ValidateCoupon(string code, decimal orderTotal)
        {
            try
            {
                var coupon = db.Coupons.FirstOrDefault(c => c.Code == code);

                if (coupon == null)
                {
                    return Json(new { success = false, message = "Mã giảm giá không tồn tại!" });
                }

                if (!coupon.IsActive)
                {
                    return Json(new { success = false, message = "Mã giảm giá đã bị vô hiệu hóa!" });
                }

                if (DateTime.Now.Date < coupon.StartDate || DateTime.Now.Date > coupon.EndDate)
                {
                    return Json(new { success = false, message = "Mã giảm giá chưa có hiệu lực hoặc đã hết hạn!" });
                }

                if (coupon.UsedQuantity >= coupon.Quantity)
                {
                    return Json(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng!" });
                }

                if (orderTotal < coupon.MinimumOrderValue)
                {
                    return Json(new { success = false, message = $"Đơn hàng tối thiểu phải từ {coupon.MinimumOrderValue:N0} VNĐ!" });
                }

                decimal discount = CalculateDiscount(coupon, orderTotal);
                decimal finalTotal = orderTotal - discount;

                return Json(new
                {
                    success = true,
                    message = "Áp dụng mã giảm giá thành công!",
                    coupon = new
                    {
                        id = coupon.ID,
                        code = coupon.Code,
                        description = coupon.Description,
                        discountType = coupon.DiscountType,
                        discountValue = coupon.DiscountValue,
                        discount = discount,
                        finalTotal = finalTotal
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        private decimal CalculateDiscount(Coupon coupon, decimal orderTotal)
        {
            if (!IsValidCoupon(coupon) || orderTotal < coupon.MinimumOrderValue)
                return 0;

            decimal discount = 0;
            if (coupon.DiscountType == "Percentage")
            {
                discount = orderTotal * (coupon.DiscountValue / 100);
            }
            else if (coupon.DiscountType == "Fixed")
            {
                discount = coupon.DiscountValue;
            }

            return Math.Min(discount, orderTotal);
        }

        private bool IsValidCoupon(Coupon coupon)
        {
            var now = DateTime.Now.Date;
            return coupon.IsActive &&
                   now >= coupon.StartDate &&
                   now <= coupon.EndDate &&
                   coupon.UsedQuantity < coupon.Quantity;
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