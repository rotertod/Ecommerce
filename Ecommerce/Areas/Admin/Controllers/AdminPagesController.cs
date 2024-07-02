using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Ecommerce.Models;
using PagedList;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class AdminPagesController : Controller
    {
        private QLToyShopEntities db = new QLToyShopEntities();

        // GET: Admin/AdminPages
        public ActionResult Index(int? page)
        {
            int pageSize = 10; // Số lượng khách hàng trên mỗi trang
            ViewBag.PageSize = pageSize;

            int pageNumber = (page ?? 1);

            var pages = db.Pages.OrderBy(p => p.PageID);

            return View(pages.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/AdminPages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Page page = db.Pages.Find(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            return View(page);
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

        // GET: Admin/AdminPages/Create
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

        // POST: Admin/AdminPages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PageID,PageName,Contents,Thumb,Published,Title,MetaDesc,MetaKey,Alias,CreateDate,Ordering")] Models.Page page)
        {
            if (ModelState.IsValid)
            {
                db.Pages.Add(page);
                db.SaveChanges();

                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm mới thành công";
                return RedirectToAction("Index");
            }

            // Thêm thông báo thất bại vào TempData
            TempData["ErrorMessage"] = "Thêm mới thất bại";
            return View(page);
        }

        // GET: Admin/AdminPages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Page page = db.Pages.Find(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            if (CheckFunctions(2) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền sửa";
                return RedirectToAction("Index");
            }

            return View(page);
        }

        // POST: Admin/AdminPages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PageID,PageName,Contents,Thumb,Published,Title,MetaDesc,MetaKey,Alias,CreateDate,Ordering")] Models.Page page)
        {
            if (ModelState.IsValid)
            {
                db.Entry(page).State = EntityState.Modified;
                db.SaveChanges();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Cập nhật thành công";
                return RedirectToAction("Index");
            }
            // Thêm thông báo thất bại vào TempData
            TempData["ErrorMessage"] = "Cập nhật thất bại";
            return View(page);
        }

        // GET: Admin/AdminPages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Page page = db.Pages.Find(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            if (CheckFunctions(3) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền xóa";
                return RedirectToAction("Index");
            }

            return View(page);
        }

        // POST: Admin/AdminPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Models.Page page = db.Pages.Find(id);
            db.Pages.Remove(page);
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
