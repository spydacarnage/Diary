using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Diary.Models;

namespace Diary.Controllers
{
    public class HashTagsController : Controller
    {
        private BlogContext db = new BlogContext();

        public class HashTagCount
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }

        // GET: HashTags
        public ActionResult Index(bool popular = false)
        {
            var tags = db.HashTags
                .Select(h => new HashTagCount { Name = h.Name, Count = h.Posts.Count });

            if (popular)
            {
                tags = tags.OrderByDescending(c => c.Count);
            }
            else
            {
                tags = tags.OrderBy(c => c.Name);
            }

            return View(tags);
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
