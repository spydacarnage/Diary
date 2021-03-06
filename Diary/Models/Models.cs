﻿using System;
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
                result = Regex.Replace(result, "__(.+?)__", "<u>$1</u>");
                result = Regex.Replace(result, "_(.+?)_", "<i>$1</i>");
                result = Regex.Replace(result, "\\*(.+?)\\*", "<b>$1</b>");
                result = Regex.Replace(result + " ", "((http|https)://.+?)(<br />|\\s)", m =>
                {
                    string temp = m.Groups[1].Value;
                    if (temp.EndsWith(".") || temp.EndsWith("!"))
                    {
                        return string.Format("<a href=\"{0}\" target=\"_new\">{0}</a>{1}", temp.Remove(temp.Length - 1), temp.Remove(0, temp.Length - 1));
                    }
                    else
                    {
                        return string.Format("<a href=\"{0}\" target=\"_new\">{0}</a>", temp);
                    }
                });

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