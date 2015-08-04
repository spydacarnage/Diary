using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diary.Models;

namespace Diary.Controllers
{
    public class HomeController : Controller
    {
        DB db = new DB();

        public ActionResult Index(string tag = "All", int page = 1, string search = "")
        {
            ViewBag.Tags = db.Tags.OrderBy(t => t.TagName).ToList();
            ViewBag.Tag = tag;

            var posts = db.BlogPosts.Include(b => b.Tags);
            if (tag.ToUpper() != "ALL")
            {
                posts = posts.Where(b => b.Tags.Any(t => t.TagName == tag));
            }

            if (search != "")
            {
                ViewBag.Search = search;
                posts = posts.Where(b => b.Heading.Contains(search) || b.Post.Contains(search));
            }

            if (page > 1)
            {
                posts = posts.Skip((page - 1) * 10);
            }

            ViewBag.Count = posts.Count();
            ViewBag.Prev = (page > 1 ? page - 1 : 0);
            ViewBag.Next = (posts.Count() > 10 ? page + 1 : 0);

            return View(posts.Take(10));
        }

        public ActionResult About()
        {

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // GET: New
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "ID,PostDate,Heading,Post")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}