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
            }

            return RedirectToAction("Index");
        }

        // 📌 BỔ SUNG: ACTION XỬ LÝ ÁP DỤNG MÃ GIẢM GIÁ (AJAX)
        [HttpPost]
        public JsonResult ApplyCoupon(string couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                Session["CouponCode"] = null;
                Session["DiscountAmount"] = 0m;
                return Json(new { success = true, message = "Đã hủy áp dụng mã giảm giá." });
            }

            // Tìm mã không phân biệt hoa thường và đang kích hoạt
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
                return Json(new { success = false, message = $"Đơn hàng phải đạt tối thiểu {coupon.MinimumOrderValue.ToString("N0")} VNĐ để áp dụng mã này!" });
            }

            // Tính số tiền giảm
            decimal discount = 0m;
            if (coupon.DiscountType == "Percentage")
            {
                discount = subTotal * (coupon.DiscountValue / 100m); // tính phần trăm giảm giá 100m là để tránh lỗi chia cho 0
            }
            else
            {
                discount = coupon.DiscountValue;
            }

            if (discount > subTotal) // đảm bảo không giảm quá số tiền đơn hàng
            {
                discount = subTotal;
            }

            Session["CouponCode"] = coupon.Code;
            Session["DiscountAmount"] = discount;

            return Json(new { success = true, message = "Áp dụng mã giảm giá thành công!" });
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

            if (!ModelState.IsValid) //kiểm tra dữ liệu hợp lệ 
            {
                return View(model);
            }

            int userId = (int)Session["UserID"];

            // 📌 BỔ SUNG: Tính toán tổng tiền thực tế sau khi trừ giảm giá
            decimal subTotal = cart.Sum(x => x.Total);
            decimal discountAmount = Session["DiscountAmount"] != null ? Convert.ToDecimal(Session["DiscountAmount"]) : 0m;
            decimal finalTotal = subTotal - discountAmount;
            if (finalTotal < 0) { finalTotal = 0; }

            Order order = new Order
            {
                UserID = userId,
                OrderDate = DateTime.Now,
                TotalAmount = finalTotal, // Đã trừ tiền giảm giá
                Status = OrderStatus.Pending,
                Phone = model.Phone,
                Address = model.Address
            };

            foreach (var item in cart) // Thêm chi tiết đơn hàng
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            // 📌 BỔ SUNG: Tăng lượt sử dụng của Coupon trong CSDL (nếu có dùng mã)
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

            // Dọn dẹp Session
            Session.Remove("Cart");
            Session.Remove("CouponCode");
            Session.Remove("DiscountAmount");

            return RedirectToAction("Success");
        }

        public ActionResult Success()
        {
            return View();
        }
    }
}