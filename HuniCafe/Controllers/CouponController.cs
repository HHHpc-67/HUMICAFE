using HuniCafe.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HuniCafe.Controllers
{
    public class UuDaiController : Controller
    {
        private HuniCafeDB db = new HuniCafeDB();

        // 1. ADMIN - DANH SÁCH MÃ GIẢM GIÁ
        public ActionResult CouponAdmin(string searchString, string statusFilter)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            var coupons = db.Coupons.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                coupons = coupons.Where(c => c.Code.Contains(searchString) || c.Description.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                var now = DateTime.Now.Date;
                switch (statusFilter)
                {
                    case "active":
                        coupons = coupons.Where(c => c.IsActive && now >= c.StartDate && now <= c.EndDate && c.UsedQuantity < c.Quantity);
                        break;
                    case "inactive":
                        coupons = coupons.Where(c => !c.IsActive || now < c.StartDate || now > c.EndDate || c.UsedQuantity >= c.Quantity);
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

            ViewBag.SearchString = searchString;
            ViewBag.StatusFilter = statusFilter;

            return View(coupons.OrderByDescending(c => c.CreatedDate).ToList());
        }

        // 2. ADMIN - THÊM MỚI MÃ GIẢM GIÁ
        public ActionResult Create()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,Description,DiscountType,DiscountValue,MinimumOrderValue,Quantity,StartDate,EndDate,IsActive")] Coupon coupon)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
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

        // 3. ADMIN - CHỈNH SỬA MÃ GIẢM GIÁ
        public ActionResult Edit(int? id)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Description,DiscountType,DiscountValue,MinimumOrderValue,Quantity,UsedQuantity,StartDate,EndDate,IsActive")] Coupon coupon)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                if (db.Coupons.Any(c => c.Code == coupon.Code && c.ID != coupon.ID))
                {
                    ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại!");
                    return View(coupon);
                }

                var existingCoupon = db.Coupons.Find(coupon.ID);
                if (existingCoupon == null)
                {
                    return HttpNotFound();
                }

                existingCoupon.Code = coupon.Code;
                existingCoupon.Description = coupon.Description;
                existingCoupon.DiscountType = coupon.DiscountType;
                existingCoupon.DiscountValue = coupon.DiscountValue;
                existingCoupon.MinimumOrderValue = coupon.MinimumOrderValue;
                existingCoupon.Quantity = coupon.Quantity;
                existingCoupon.UsedQuantity = coupon.UsedQuantity;
                existingCoupon.IsActive = coupon.IsActive;

                if (coupon.StartDate != DateTime.MinValue) existingCoupon.StartDate = coupon.StartDate;
                if (coupon.EndDate != DateTime.MinValue) existingCoupon.EndDate = coupon.EndDate;

                db.SaveChanges();

                TempData["Success"] = "Cập nhật mã giảm giá thành công!";
                return RedirectToAction("CouponAdmin");
            }

            return View(coupon);
        }

        // 4. ADMIN - XÓA MÃ GIẢM GIÁ
        public ActionResult Delete(int? id)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            Coupon coupon = db.Coupons.Find(id);
            if (coupon != null)
            {
                db.Coupons.Remove(coupon);
                db.SaveChanges();
                TempData["Success"] = "Xóa mã giảm giá thành công!";
            }

            // Đã sửa: Chuyển hướng về CouponAdmin thay vì Index
            return RedirectToAction("CouponAdmin");
        }

        // 5. KHÁCH HÀNG - XEM DANH SÁCH MÃ ƯU ĐÃI KHẢ DỤNG
        public ActionResult UuDai(string searchString)
        {
            var now = DateTime.Now;

            var activeCoupons = db.Coupons.Where(c => c.IsActive && c.StartDate <= now && c.EndDate >= now && c.UsedQuantity < c.Quantity);

            if (!string.IsNullOrEmpty(searchString))
            {
                activeCoupons = activeCoupons.Where(c => c.Code.Contains(searchString) || c.Description.Contains(searchString));
            }

            ViewBag.SearchString = searchString;

            return View(activeCoupons.OrderBy(c => c.MinimumOrderValue).ToList());
        }

        // 6. KHÁCH HÀNG - ÁP DỤNG MÃ GIẢM GIÁ TRONG GIỎ HÀNG (AJAX)
        [HttpPost]
        public JsonResult ApplyCoupon(string couponCode)
        {
            if (string.IsNullOrEmpty(couponCode))
            {
                Session["CouponCode"] = null;
                Session["DiscountAmount"] = 0;
                return Json(new { success = true, message = "Đã bỏ áp dụng mã." });
            }

            var coupon = db.Coupons.FirstOrDefault(c => c.Code == couponCode);

            if (coupon == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không tồn tại!" });
            }

            if (!coupon.IsActive)
            {
                return Json(new { success = false, message = "Mã giảm giá đã bị vô hiệu hóa!" });
            }

            var now = DateTime.Now;
            if (now < coupon.StartDate || now > coupon.EndDate)
            {
                return Json(new { success = false, message = "Mã giảm giá chưa đến đợt hoặc đã hết hạn sử dụng!" });
            }

            if (coupon.UsedQuantity >= coupon.Quantity)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng!" });
            }

            List<Cart> cart = Session["Cart"] as List<Cart>;
            decimal totalAmount = 0;
            if (cart != null)
            {
                totalAmount = cart.Sum(x => x.Price * x.Quantity);
            }

            if (totalAmount < coupon.MinimumOrderValue)
            {
                return Json(new { success = false, message = "Đơn hàng của bạn chưa đủ điều kiện tối thiểu để áp dụng mã này!" });
            }

            decimal discountAmount = CalculateDiscount(coupon, totalAmount);

            Session["CouponCode"] = coupon.Code;
            Session["DiscountAmount"] = discountAmount;

            return Json(new { success = true, message = "Áp dụng mã giảm giá thành công!" });
        }

        // 7. HÀM PHỤ TÍNH TIỀN GIẢM GIÁ
        private decimal CalculateDiscount(Coupon coupon, decimal orderTotal)
        {
            decimal discount = 0;

            if (coupon.DiscountType == "Percentage")
            {
                // Đã sửa: Thay 100 bằng 100m để tính đúng số thập phân
                discount = orderTotal * (coupon.DiscountValue / 100m);
            }
            else if (coupon.DiscountType == "Fixed")
            {
                discount = coupon.DiscountValue;
            }

            if (discount > orderTotal)
            {
                discount = orderTotal;
            }

            return discount;
        }

        // 8. BỎ MÃ GIẢM GIÁ
        [HttpPost]
        public JsonResult RemoveCoupon()
        {
            Session["CouponCode"] = null;
            Session["DiscountAmount"] = null;
            return Json(new { success = true, message = "Đã gỡ mã giảm giá." });
        }

        // 9. API KIỂM TRA MÃ GIẢM GIÁ CHO TRANG CHECKOUT
        [HttpPost]
        public JsonResult ValidateCoupon(string code, decimal orderTotal)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá!" });
            }

            var coupon = db.Coupons.FirstOrDefault(c => c.Code == code);

            if (coupon == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không tồn tại!" });
            }

            if (!coupon.IsActive)
            {
                return Json(new { success = false, message = "Mã giảm giá đã bị ngưng hoạt động!" });
            }

            var now = DateTime.Now;
            if (now < coupon.StartDate || now > coupon.EndDate)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn sử dụng hoặc chưa đến đợt!" });
            }

            if (coupon.UsedQuantity >= coupon.Quantity)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng!" });
            }

            if (orderTotal < coupon.MinimumOrderValue)
            {
                return Json(new { success = false, message = $"Mã này áp dụng cho đơn hàng tối thiểu {coupon.MinimumOrderValue:N0} VNĐ!" });
            }

            decimal discount = CalculateDiscount(coupon, orderTotal);

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
                    finalTotal = orderTotal - discount
                }
            });
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