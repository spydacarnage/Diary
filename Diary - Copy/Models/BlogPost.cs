using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Diary.Models
{
    public class BlogPost
    {
        public int ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DataType(DataType.Date)]
        public DateTime PostDate { get; set; }

        [StringLength(100)]
        [Required]
        public string Heading { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Post { get; set; }

        public virtual List<Tag> Tags { get; set; }
    }

}