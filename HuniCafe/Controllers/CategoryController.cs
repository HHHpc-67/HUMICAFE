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
    public class CategoryController : Controller
    {
        private readonly HuniCafeDB db = new HuniCafeDB();

        // 1. DANH SÁCH DANH MỤC
        // GET: Category/CategoryPage
        public ActionResult CategoryPage()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        // 2. THÊM MỚI DANH MỤC
        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName,CategoryDescription,IsActive")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("CategoryPage");
            }

            return View(category);
        }

        // 3. CHỈNH SỬA DANH MỤC
        // GET: Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName,CategoryDescription,IsActive")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CategoryPage");
            }
            return View(category);
        }

        // 4. ẨN / HIỆN DANH MỤC
        public ActionResult ToggleStatus(int id)
        {
            Category category = db.Categories.Find(id);
            if (category != null)
            {
                category.IsActive = !category.IsActive;

                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("CategoryPage");
        }

        // 5. XÓA DANH MỤC
        // GET: Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);

            // Kiểm tra xem danh mục có đang chứa sản phẩm nào không
            if (category.Products != null && category.Products.Count > 0)
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục này vì đang có sản phẩm thuộc về nó! Hãy chuyển trạng thái sang 'Ẩn' thay vì xóa.";
                return RedirectToAction("CategoryPage");
            }

            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("CategoryPage");
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