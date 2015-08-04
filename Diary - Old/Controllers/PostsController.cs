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
    public class PostsController : Controller
    {
        private BlogContext db = new BlogContext();

        // GET: Posts
        public ActionResult Index()
        {
            ViewBag.Categories = db.Categories.OrderBy(c => c.Name);

            var events = db.Posts
                .Where(p => p.Categories.Any(c => c.Name == "Event")).AsEnumerable();

            List<Post> allEvents = new List<Post>();
            allEvents.AddRange(
                events
                    .Where(e => e.EventDate > DateTime.Today && e.EventDate < DateTime.Today.AddMonths(3))
                    .OrderBy(e => e.PostDate)
                    .ToList());

            ViewBag.Events = allEvents.Take(3);

            return View(db.Posts.ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        public ActionResult Create()
        {
            var post = new Post() { PostDate = DateTime.Today, Categories = new List<Category>() };
            PopulateAssignedCategories(post);
            return View(post);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,PostDate,Heading,Body")] Post post, string[] selectedCategories)
        {
            if (ModelState.IsValid)
            {
                if (selectedCategories != null)
                {
                    post.Categories = new List<Category>();
                    foreach(var category in selectedCategories)
                    {
                        var categoryToAdd = db.Categories.Find(int.Parse(category));
                        post.Categories.Add(categoryToAdd);
                    }
                }
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            PopulateAssignedCategories(post);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,PostDate,Heading,Body")] Post post, string[] selectedCategories)
        {
            if (ModelState.IsValid)
            {
                var postToUpdate = db.Posts.Include(p => p.Categories).Where(p => p.ID == post.ID).Single();
                postToUpdate.PostDate = post.PostDate;
                postToUpdate.Heading = post.Heading;
                postToUpdate.Body = post.Body;

                UpdateAssignedCategories(selectedCategories, postToUpdate);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulateAssignedCategories(Post post)
        {
            var allCategories = db.Categories;
            var postCategories = new HashSet<int>(post.Categories.Select(c => c.ID));
            var viewModel = new List<AssignedCategoryData>();

            foreach (var category in allCategories)
            {
                viewModel.Add(new AssignedCategoryData
                {
                    ID = category.ID,
                    Name = category.Name,
                    Assigned = postCategories.Contains(category.ID)
                });
            }
            ViewBag.Categories = viewModel;
        }

        private void UpdateAssignedCategories(string[] selectedCategories, Post postToUpdate)
        {
            if (selectedCategories == null)
            {
                postToUpdate.Categories = new List<Category>();
                return;
            }

            var selectedCategoriesHS = new HashSet<string>(selectedCategories);
            var postCategories = new HashSet<int>(postToUpdate.Categories.Select(c => c.ID));

            foreach(var category in db.Categories)
            {
                if (selectedCategoriesHS.Contains(category.ID.ToString()))
                {
                    if (!postCategories.Contains(category.ID))
                    {
                        postToUpdate.Categories.Add(category);
                    }
                }
                else
                {
                    if (postCategories.Contains(category.ID))
                    {
                        postToUpdate.Categories.Remove(category);
                    }
                }
            }
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

    public class AssignedCategoryData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
    }
}
