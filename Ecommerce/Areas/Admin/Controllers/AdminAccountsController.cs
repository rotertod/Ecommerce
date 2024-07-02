using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Models;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class AdminAccountsController : Controller
    {
        private QLToyShopEntities db = new QLToyShopEntities();

        // GET: Admin/Accounts
        public ActionResult Index()
        {
            var accounts = db.Accounts.Include(a => a.Role);
            return View(accounts.ToList());
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

        // GET: Admin/Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (CheckFunctions(5) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền xem chi tiết";
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Admin/Accounts/Create
        public ActionResult Create()
        {
            if (CheckFunctions(1) == false || CheckFunctions(5) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền thêm";
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName");
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountID,Phone,Email,Password,Salt,Active,FullName,RoleID,LastLogin,CreateDate")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm mới thành công";
                return RedirectToAction("Index");
            }

            // Thêm thông báo thất bại vào TempData
            TempData["ErrorMessage"] = "Thêm mới thất bại";

            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", account.RoleID);
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            if (CheckFunctions(2) == false || CheckFunctions(5) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền sửa";
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", account.RoleID);
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountID,Phone,Email,Password,Salt,Active,FullName,RoleID,LastLogin,CreateDate")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Chỉnh sửa thành công";
                return RedirectToAction("Index");
            }
            // Thêm thông báo thất bại vào TempData
            TempData["ErrorMessage"] = "Chỉnh sửa thất bại";
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", account.RoleID);
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            if (CheckFunctions(3) == false)
            {
                // Thêm thông báo thất bại vào TempData
                TempData["ErrorMessage"] = "Bạn không có quyền xóa";
                return RedirectToAction("Index");
            }

            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
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
