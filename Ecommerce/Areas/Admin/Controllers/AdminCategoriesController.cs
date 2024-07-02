using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Models;
using PagedList;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class AdminCategoriesController : Controller
    {
        private QLToyShopEntities db = new QLToyShopEntities();

        // GET: Admin/AdminCategories
        public ActionResult Index(int? page)
        {
            int pageSize = 10; // Số lượng khách hàng trên mỗi trang
            ViewBag.PageSize = pageSize;

            int pageNumber = (page ?? 1);

            var categories = db.Categories.OrderBy(p => p.CatID);

            return View(categories.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/AdminCategories/Details/5
        public ActionResult Details(int? id)
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

        public bool CheckFunctions(int functionID)
        {
            // kiểm tra quyền
            // 1. Count phân quyền trong database
            Account accountSession = (Account)Session["user"];
            var count = db.Authorizations.Count(m => m.RoleID == accountSession.RoleID && m.FunctionID == functionID);
            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // GET: Admin/AdminCategories/Create
        public ActionResult Create()
        {
            if (CheckFunctions(1) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền thêm";
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Admin/AdminCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CatID,CatName,Description,ParentID,Levels,Ordering,Published,Thumb,Title,Alias,MetaDesc,MetaKey,Cover,SchemaMarkup")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm mới thành công";
                return RedirectToAction("Index");
            }

            // Thêm thông báo thất bại vào TempData
            TempData["ErrorMessage"] = "Thêm mới thất bại";
            return View(category);
        }

        // GET: Admin/AdminCategories/Edit/5
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
            if (CheckFunctions(2) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền sửa";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // POST: Admin/AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CatID,CatName,Description,ParentID,Levels,Ordering,Published,Thumb,Title,Alias,MetaDesc,MetaKey,Cover,SchemaMarkup")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Cập nhật thành công";
                return RedirectToAction("Index");
            }
            // Thêm thông báo thất bại vào TempData
            TempData["ErrorMessage"] = "Cập nhật thất bại";
            return View(category);
        }

        // GET: Admin/AdminCategories/Delete/5
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
            if (CheckFunctions(3) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền xóa";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // POST: Admin/AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();

            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa thành công";
            return RedirectToAction("Index");
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
