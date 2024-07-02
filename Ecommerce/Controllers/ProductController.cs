using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using System.Web.UI;
using System.Net;

namespace Ecommerce.Controllers
{
    public class ProductController : Controller
    {
        QLToyShopEntities db = new QLToyShopEntities();

        // GET: Product
        public ActionResult Index(int? page, int? category, string sort)
        {
            try
            {
                int pageSize = 12; // Số lượng trên mỗi trang
                ViewBag.PageSize = pageSize;

                int pageNumber = (page ?? 1);

                var products = db.Products.Include(p => p.Category);

                // Lọc sản phẩm theo danh mục
                if (category.HasValue && category.Value != 0)
                {
                    products = products.Where(p => p.Category.CatID == category.Value);
                }

                // Sắp xếp sản phẩm
                switch (sort)
                {
                    case "NameAZ":
                        products = products.OrderBy(p => p.ProductName);
                        break;
                    case "NameZA":
                        products = products.OrderByDescending(p => p.ProductName);
                        break;
                    case "Low":
                        products = products.OrderBy(p => p.Price);
                        break;
                    case "High":
                        products = products.OrderByDescending(p => p.Price);
                        break;
                    default:
                        // Default sorting, e.g., by ProductID
                        products = products.OrderBy(p => p.ProductID);
                        break;
                }

                ViewData["DanhMuc"] = new SelectList(db.Categories, "CatID", "CatName");

                return View(products.ToPagedList(pageNumber, pageSize));
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Lấy sản phẩm cùng danh mục
            var relatedProducts = db.Products.Where(p => p.CatID == product.CatID && p.ProductID != product.ProductID).ToList();

            // Truyền danh sách sản phẩm cùng danh mục vào view
            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
    }
}