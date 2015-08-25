using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Diary.Models
{
    public class Post
    {
        public int ID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PostDate { get; set; }

        [StringLength(100)]
        public string Heading { get; set; }

        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        //Navigation
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<HashTag> HashTags { get; set; }

        //Display
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime DisplayDate
        {
            get
            {
                return PostDate;
            }
        }

        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime EventDate
        {
            get
            {
                return new DateTime(DateTime.Now.Year, PostDate.Month, PostDate.Day);
            }
        }

        public bool InCategory(string category)
        {
            foreach(Category c in Categories)
            {
                if (c.Name == category)
                    return true;
            }

            return false;
        }

        public string HtmlView
        {
            get
            {
                string result = Body.Replace(System.Environment.NewLine, "<br />");
                result = Regex.Replace(result, "#(\\w+)", "<a href=\"/?hashtag=$1\" class=\"hashtag\">#$1</a>");
                return result;
            }
        }
    }

    public class Category
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        //Navigation
        public virtual ICollection<Post> Posts { get; set; }
    }

    public class HashTag
    {
        public int ID { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        //Navigation
        public virtual ICollection<Post> Posts { get; set; }
    }
}