using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Diary.Models
{
    public class DB : DbContext
    {
        public virtual DbSet<BlogPost> BlogPosts { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
    }
}