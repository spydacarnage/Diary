using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Diary.Models
{
    public class Tag
    {
        public int ID { get; set; }
        [StringLength(50)]
        public string TagName { get; set; }

        public virtual List<BlogPost> BlogPosts { get; set; }
    }
}