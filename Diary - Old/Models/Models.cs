using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
    }

    public class Category
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        //Navigation
        public virtual ICollection<Post> Posts { get; set; }
    }
}