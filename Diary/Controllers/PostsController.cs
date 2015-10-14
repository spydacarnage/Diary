using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Diary.Models;
using Newtonsoft.Json;

namespace Diary.Controllers
{
    public class PostsController : Controller
    {
        private BlogContext db = new BlogContext();

        // GET: Posts
        public ActionResult Index(string category = "All", string hashtag = "", int page = 1, string search = "")
        {
            SecurityController.CheckAuth(this);

            ViewBag.Categories = db.Categories.OrderBy(c => c.Name).ToList();
            ViewBag.Category = category;

            var events = db.Posts
                .Where(p => p.Categories.Any(c => (c.Name == "Event" || c.Name == "Birthday") || (c.Name == "Reminder" && p.PostDate > DateTime.Today) )).AsEnumerable();

            List<Post> allEvents = new List<Post>();
            allEvents.AddRange(
                events
                    .Where(e => e.EventDate > DateTime.Today && e.EventDate < DateTime.Today.AddMonths(3))
                    .OrderBy(e => e.EventDate)
                    .ToList());

            ViewBag.Events = allEvents.Take(3).ToList();

            var posts = db.Posts.OrderByDescending(p => p.PostDate).Include(p => p.Categories);
            if (hashtag != "")
            {
                ViewBag.Category = hashtag = "#" + hashtag.ToLower();
                posts = posts.Where(p => p.HashTags.Any(h => h.Name == hashtag));
            }
            else
            {
                if (category.ToUpper() != "ALL")
                {
                    posts = posts.Where(p => p.Categories.Any(c => c.Name == category));
                }
                else
                {
                    posts = posts.Where(p => p.Categories.All(c => c.Name != "Birthday" && c.Name != "Reminder"));
                }
            }

            if (search != "")
            {
                ViewBag.Search = search;
                posts = posts.Where(p => p.Heading.Contains(search) || p.Body.Contains(search));
            }

            if (page > 1)
            {
                posts = posts.Skip((page - 1) * 10);
            }

            ViewBag.Count = posts.Count();
            ViewBag.Prev = (page > 1 ? page - 1 : 0);
            ViewBag.Next = (posts.Count() > 10 ? page + 1 : 0);


            return View(posts.Take(10).ToList());
        }

        // GET: Posts/List
        public ActionResult List()
        {
            SecurityController.CheckAuth(this);

            return View(db.Posts.ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            SecurityController.CheckAuth(this);

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
            SecurityController.CheckAuth(this);

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
            SecurityController.CheckAuth(this);

            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(post.Heading))
                    post.Heading = "Daily post";

                if (selectedCategories != null)
                {
                    post.Categories = new List<Category>();
                    foreach(var category in selectedCategories)
                    {
                        var categoryToAdd = db.Categories.Find(int.Parse(category));
                        post.Categories.Add(categoryToAdd);
                    }
                }

                PopulateHashTags(post);

                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            SecurityController.CheckAuth(this);

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
            //PopulateHashTags(post);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,PostDate,Heading,Body")] Post post, string[] selectedCategories)
        {
            SecurityController.CheckAuth(this);

            if (ModelState.IsValid)
            {
                var postToUpdate = db.Posts.Include(p => p.Categories).Where(p => p.ID == post.ID).Single();
                postToUpdate.PostDate = post.PostDate;
                postToUpdate.Heading = post.Heading;
                postToUpdate.Body = post.Body;

                UpdateAssignedCategories(selectedCategories, postToUpdate);
                PopulateHashTags(postToUpdate);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            SecurityController.CheckAuth(this);

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
            SecurityController.CheckAuth(this);

            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private class JsonPost
        {
            public DateTime PostDate { get; set; }
            public string Heading { get; set; }
            public string Body { get; set; }
            private List<string> _Categories = new List<string>();
            public string[] Categories
            {
                get
                {
                    return _Categories.ToArray();
                }
                set
                {
                    _Categories = new List<string>();
                    foreach (string s in value)
                        _Categories.Add(s);
                }
            }
            public JsonPost() { }
            public JsonPost(Post post)
            {
                PostDate = post.PostDate;
                Heading = post.Heading;
                Body = post.Body;
                foreach (Category c in post.Categories)
                {
                    _Categories.Add(c.Name);
                }
            }
        }
        public ActionResult Export()
        {
            SecurityController.CheckAuth(this);

            var posts = db.Posts.Include(p => p.Categories);
            List<JsonPost> jPosts = new List<JsonPost>();
            
            foreach(Post p in posts)
            {
                jPosts.Add(new JsonPost(p));
            }

            string result = JsonConvert.SerializeObject(jPosts);

            return Json(jPosts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Import()
        {
            SecurityController.CheckAuth(this);

            return View();
        }

        [HttpPost]
        public void Import(string json)
        {
            SecurityController.CheckAuth(this);

            List<JsonPost> jPosts = JsonConvert.DeserializeObject<List<JsonPost>>(json);

            foreach (JsonPost jPost in jPosts)
            {
                Post post = new Post { PostDate = jPost.PostDate, Heading = jPost.Heading, Body = jPost.Body };
                if (jPost.Categories.Count() > 0)
                {
                    post.Categories = new List<Category>();
                    foreach (var category in jPost.Categories)
                    {
                        Category categoryToAdd = db.Categories.Single(c => c.Name == category);
                        post.Categories.Add(categoryToAdd);
                    }
                }
                db.Posts.Add(post);
            }
            db.SaveChanges();

            RedirectToAction("Index");
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

        private void PopulateHashTags(Post post)
        {
            var mHash = Regex.Matches(post.Body + " ", @"(#.*?)\W");
            if (mHash.Count == 0)
            {
                post.HashTags = new List<HashTag>();
                return;
            }

            var newTags = new HashSet<string>();
            foreach (Match m in mHash)
                newTags.Add(m.Groups[1].Value.ToLower());

            HashSet<string> postTags;
            if (post.HashTags == null)
            {
                postTags = new HashSet<string>();
                post.HashTags = new List<HashTag>();
            }
            else
            {
                postTags = new HashSet<string>(post.HashTags.Select(h => h.Name.ToLower()));
            }

            if (newTags.SetEquals(postTags))
                return;

            foreach(string tag in postTags)
            {
                if (!newTags.Contains(tag))
                {
                    post.HashTags.Remove(db.HashTags.Single(h => h.Name == tag));
                }
            }

            foreach(string tag in newTags)
            {
                if (!postTags.Contains(tag))
                {
                    var dbHash = db.HashTags.SingleOrDefault(h => h.Name == tag);
                    if (dbHash == null)
                    {
                        post.HashTags.Add(new HashTag() { Name = tag });
                    }
                    else
                    {
                        post.HashTags.Add(dbHash);
                    }
                }
            }
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
