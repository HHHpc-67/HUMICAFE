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

            // Lấy số lượng người dùng nhập, nếu không có thì mặc định là 1
            int sl = quantity ?? 1;

            if (item != null)
            {
                item.Quantity += sl; // Cộng dồn số lượng chọn thêm
            }
            else
            {
                cart.Add(new Cart
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Image = product.Image,
                    Price = product.Price,
                    Quantity = sl // Gán số lượng theo người dùng chọn
                });
            }

            Session["Cart"] = cart;

            return RedirectToAction("Index", "Cart"); // Hoặc trả về trang giỏ hàng của cậu
        }


        //nút tăng số lượng 
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



        //giảm
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



        //nút xáo
        
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


        
        //public ActionResult Checkout()
        //{
        //    // Kiểm tra đăng nhập
        //    if (Session["UserID"] == null)
        //    {
        //        return RedirectToAction("Login", "Users");
        //    }

        //    var cart = Session["Cart"] as List<Cart>;

        //    if (cart == null || !cart.Any())
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    return View(cart);
        //}
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


            // validation Gán lại dữ liệu cho ViewModel khi trả về View
            model.Cart = cart;
            model.TotalAmount = cart.Sum(x => x.Total);

            // validation  Nếu dữ liệu không hợp lệ thì quay lại Checkout và khong lưu vào db 
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = (int)Session["UserID"];

            Order order = new Order
            {
                UserID = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(x => x.Total),
                Status = OrderStatus.Pending,

                Phone = model.Phone,
                Address = model.Address,
               
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

            db.Orders.Add(order);
            db.SaveChanges();

            Session.Remove("Cart");

            return RedirectToAction("Success");
        }


        ////tạo orde  
        //public ActionResult PlaceOrder()
        //{
        //    if (Session["UserID"] == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    var cart = Session["Cart"] as List<Cart>;

        //    if (cart == null || !cart.Any())
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    int userId = (int)Session["UserID"];

        //    Order order = new Order
        //    {
        //        UserID = userId,
        //        OrderDate = DateTime.Now,
        //        TotalAmount = cart.Sum(x => x.Total),
        //        Status = OrderStatus.Pending        // dùng class OrderStatus để định nghĩa trạng thái đơn hàng thay vì hardcode Status = "Pending"
        //    };

        //    foreach (var item in cart)
        //    {
        //        order.OrderDetails.Add(new OrderDetail
        //        {
        //            ProductID = item.ProductID,
        //            Quantity = item.Quantity,
        //            Price = item.Price
        //        });
        //    }

        //    db.Orders.Add(order);

        //    db.SaveChanges();

        //    Session.Remove("Cart"); //sau khi thành công thì xóa cart

        //    return RedirectToAction("Success");
        //}



        public ActionResult Success()
        {
            return View();
        }
    }
}