using HuniCafe.Models;
using HuniCafe.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HuniCafe.Controllers
{
    public class CartController : Controller
    {
        private readonly HuniCafeDB db = new HuniCafeDB();

        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<Cart>;

            if (cart == null)
            {
                cart = new List<Cart>();
            }

            // Tự động kiểm tra lại tính hợp lệ của mã giảm giá khi xem giỏ hàng
            ValidateSessionCoupon(cart);

            return View(cart);
        }

        public ActionResult AddToCart(int? id, int? quantity)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Product");
            }

            var product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var cart = Session["Cart"] as List<Cart>;

            if (cart == null)
            {
                cart = new List<Cart>();
            }

            var item = cart.FirstOrDefault(x => x.ProductID == id);

            int sl = quantity ?? 1;

            if (item != null)
            {
                item.Quantity += sl;
            }
            else
            {
                cart.Add(new Cart
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Image = product.Image,
                    Price = product.Price,
                    Quantity = sl
                });
            }

            Session["Cart"] = cart;

            // Kiểm tra lại Coupon khi giỏ hàng thay đổi
            ValidateSessionCoupon(cart);

            return RedirectToAction("Index", "Cart");
        }

        // Tăng số lượng
        public ActionResult Increase(int id)
        {
            var cart = Session["Cart"] as List<Cart>;

            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductID == id);

                if (item != null)
                {
                    item.Quantity++;
                }

                Session["Cart"] = cart;
                ValidateSessionCoupon(cart);
            }

            return RedirectToAction("Index");
        }

        // Giảm
        public ActionResult Decrease(int id)
        {
            var cart = Session["Cart"] as List<Cart>;

            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductID == id);

                if (item != null)
                {
                    item.Quantity--;

                    if (item.Quantity <= 0)
                    {
                        cart.Remove(item);
                    }
                }

                Session["Cart"] = cart;
                ValidateSessionCoupon(cart);
            }

            return RedirectToAction("Index");
        }

        // Xóa
        public ActionResult Remove(int id)
        {
            var cart = Session["Cart"] as List<Cart>;

            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductID == id);

                if (item != null)
                {
                    cart.Remove(item);
                }

                Session["Cart"] = cart;
                ValidateSessionCoupon(cart);
            }

            return RedirectToAction("Index");
        }

        //ACTION XỬ LÝ CẬP NHẬT SỐ LƯỢNG TRỰC TIẾP (AJAX)
        [HttpPost]
        public JsonResult UpdateQuantity(int id, int quantity)
        {
            var cart = Session["Cart"] as List<Cart>;

            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductID == id);

                if (item != null)
                {
                    if (quantity <= 0)
                    {
                        cart.Remove(item);
                    }
                    else
                    {
                        item.Quantity = quantity;
                    }

                    Session["Cart"] = cart;
                    ValidateSessionCoupon(cart);

                    decimal subTotal = cart.Sum(x => x.Total);
                    decimal discount = Session["DiscountAmount"] != null ? Convert.ToDecimal(Session["DiscountAmount"]) : 0m;
                    decimal total = subTotal - discount;

                    return Json(new
                    {
                        success = true,
                        itemTotal = item.Total,
                        subTotal = subTotal,
                        discount = discount,
                        total = total < 0 ? 0 : total,
                        cartCount = cart.Sum(x => x.Quantity)
                    });
                }
            }

            return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng!" });
        }

        // ACTION XỬ LÝ ÁP DỤNG MÃ GIẢM GIÁ (AJAX)
        [HttpPost]
        public JsonResult ApplyCoupon(string couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                Session["CouponCode"] = null;
                Session["DiscountAmount"] = 0m;
                return Json(new { success = true, message = "Đã hủy áp dụng mã giảm giá." });
            }

            var coupon = db.Coupons.FirstOrDefault(c => c.Code.ToLower() == couponCode.Trim().ToLower() && c.IsActive);

            if (coupon == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không tồn tại hoặc không hợp lệ!" });
            }

            if (DateTime.Now < coupon.StartDate || DateTime.Now > coupon.EndDate)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn hoặc chưa đến đợt!" });
            }

            if (coupon.UsedQuantity >= coupon.Quantity)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng!" });
            }

            var cart = Session["Cart"] as List<Cart>;
            decimal subTotal = (cart != null && cart.Any()) ? cart.Sum(x => x.Total) : 0m;

            if (subTotal < coupon.MinimumOrderValue)
            {
                return Json(new { success = false, message = $"Đơn hàng phải đạt tối thiểu {coupon.MinimumOrderValue:N0} VNĐ để áp dụng mã này!" });
            }

            decimal discount = 0m;
            if (coupon.DiscountType == "Percentage")
            {
                discount = subTotal * (coupon.DiscountValue / 100m);
            }
            else
            {
                discount = coupon.DiscountValue;
            }

            if (discount > subTotal)
            {
                discount = subTotal;
            }

            Session["CouponCode"] = coupon.Code;
            Session["DiscountAmount"] = discount;

            return Json(new { success = true, message = "Áp dụng mã giảm giá thành công!", discount = discount });
        }

        // ACTION XÓA MÃ GIẢM GIÁ
        [HttpPost]
        public JsonResult RemoveCoupon()
        {
            Session["CouponCode"] = null;
            Session["DiscountAmount"] = 0m;
            return Json(new { success = true, message = "Đã gỡ mã giảm giá." });
        }

        [HttpGet]
        public ActionResult Checkout()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var cart = Session["Cart"] as List<Cart>;

            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index");
            }

            ValidateSessionCoupon(cart);

            int userId = (int)Session["UserID"];
            var user = db.Users.Find(userId);

            var model = new CheckoutViewModel
            {
                Cart = cart,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                TotalAmount = cart.Sum(x => x.Total)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var cart = Session["Cart"] as List<Cart>;

            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index");
            }

            model.Cart = cart;
            model.TotalAmount = cart.Sum(x => x.Total);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ValidateSessionCoupon(cart);

            int userId = (int)Session["UserID"];

            decimal subTotal = cart.Sum(x => x.Total);
            decimal discountAmount = Session["DiscountAmount"] != null ? Convert.ToDecimal(Session["DiscountAmount"]) : 0m;
            decimal finalTotal = subTotal - discountAmount;
            if (finalTotal < 0) { finalTotal = 0; }

            // Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu khi tạo Đơn hàng và Cập nhật Coupon
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Order order = new Order
                    {
                        UserID = userId,
                        OrderDate = DateTime.Now,
                        TotalAmount = finalTotal,
                        Status = OrderStatus.Pending,
                        Phone = model.Phone,
                        Address = model.Address
                    };

                    foreach (var item in cart)
                    {
                        order.OrderDetails.Add(new OrderDetail
                        {
                            ProductID = item.ProductID,
                            Quantity = item.Quantity,
                            Price = item.Price
                        });
                    }

                    if (Session["CouponCode"] != null)
                    {
                        string code = Session["CouponCode"].ToString();
                        var coupon = db.Coupons.FirstOrDefault(c => c.Code == code);
                        if (coupon != null)
                        {
                            coupon.UsedQuantity += 1;
                        }
                    }

                    db.Orders.Add(order);
                    db.SaveChanges();

                    transaction.Commit();

                    // Dọn dẹp Session
                    Session.Remove("Cart");
                    Session.Remove("CouponCode");
                    Session.Remove("DiscountAmount");

                    return RedirectToAction("Success");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình xử lý đơn hàng. Vui lòng thử lại!");
                    return View(model);
                }
            }
        }

        public ActionResult Success()
        {
            return View();
        }

        //HELPER METHOD: TỰ ĐỘNG TÍNH LẠI HOẶC HỦY COUPON NẾU GIỎ HÀNG THAY ĐỔI
        private void ValidateSessionCoupon(List<Cart> cart)
        {
            if (Session["CouponCode"] == null) return;

            if (cart == null || !cart.Any())
            {
                Session["CouponCode"] = null;
                Session["DiscountAmount"] = 0m;
                return;
            }

            string code = Session["CouponCode"].ToString();
            var coupon = db.Coupons.FirstOrDefault(c => c.Code.ToLower() == code.ToLower() && c.IsActive);

            decimal subTotal = cart.Sum(x => x.Total);

            if (coupon == null || DateTime.Now < coupon.StartDate || DateTime.Now > coupon.EndDate || coupon.UsedQuantity >= coupon.Quantity || subTotal < coupon.MinimumOrderValue)
            {
                // Hủy mã nếu không còn đủ điều kiện
                Session["CouponCode"] = null;
                Session["DiscountAmount"] = 0m;
            }
            else
            {
                // Tính lại số tiền giảm nếu số lượng sản phẩm thay đổi
                decimal discount = 0m;
                if (coupon.DiscountType == "Percentage")
                {
                    discount = subTotal * (coupon.DiscountValue / 100m);
                }
                else
                {
                    discount = coupon.DiscountValue;
                }

                if (discount > subTotal) { discount = subTotal; }
                Session["DiscountAmount"] = discount;
            }
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