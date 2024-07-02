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
    public class AdminTinTucsController : Controller
    {
        private QLToyShopEntities db = new QLToyShopEntities();

        // GET: Admin/AdminTinTucs
        public ActionResult Index(int? page)
        {
            int pageSize = 10; // Số lượng tin tức trên mỗi trang
            ViewBag.PageSize = pageSize;

            int pageNumber = (page ?? 1);

            var posts = db.TinTucs.OrderBy(p => p.PostID);

            return View(posts.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/AdminTinTucs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
            {
                return HttpNotFound();
            }
            return View(tinTuc);
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

        // GET: Admin/AdminTinTucs/Create
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

        // POST: Admin/AdminTinTucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostID,Title,SContents,Contents,Thumb,Published,Alias,CreatedDate,Author,AccountID,Tags,CatID,isHot,isNewFeed,MetaKey,MetaDesc,Views")] TinTuc tinTuc)
        {
            if (ModelState.IsValid)
            {
                if (tinTuc.Author == null)
                {
                    tinTuc.Author = "Admin";
                }
                tinTuc.CreatedDate = DateTime.Now;
                db.TinTucs.Add(tinTuc);
                db.SaveChanges();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm mới thành công";
                return RedirectToAction("Index");
            }

            // Thêm thông báo thất bại vào TempData
            TempData["ErrorMessage"] = "Thêm mới thất bại";
            return View(tinTuc);
        }

        // GET: Admin/AdminTinTucs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
            {
                return HttpNotFound();
            }
            if (CheckFunctions(2) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền sửa";
                return RedirectToAction("Index");
            }

            return View(tinTuc);
        }

        // POST: Admin/AdminTinTucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostID,Title,SContents,Contents,Thumb,Published,Alias,CreatedDate,Author,AccountID,Tags,CatID,isHot,isNewFeed,MetaKey,MetaDesc,Views")] TinTuc tinTuc)
        {
            if (ModelState.IsValid)
            {
                tinTuc.CreatedDate = DateTime.Now;

                db.Entry(tinTuc).State = EntityState.Modified;
                db.SaveChanges();

                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Cập nhật thành công";
                return RedirectToAction("Index");
            }
            // Thêm thông báo thất bại vào TempData
            TempData["ErrorMessage"] = "Cập nhật thất bại";
            return View(tinTuc);
        }

        // GET: Admin/AdminTinTucs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
            {
                return HttpNotFound();
            }
            if (CheckFunctions(3) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền xóa";
                return RedirectToAction("Index");
            }

            return View(tinTuc);
        }

        // POST: Admin/AdminTinTucs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TinTuc tinTuc = db.TinTucs.Find(id);
            db.TinTucs.Remove(tinTuc);
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
