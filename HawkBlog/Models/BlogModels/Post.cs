using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HawkBlog.Models
{
    public class Post
    {
        [Key]
        public int PostID { get; set; }

        [Required(ErrorMessage = "Post title is required")]
        [Display(Name = "Title")]
        [StringLength(100)]
        public string PostTitle { get; set; }

        [Required(ErrorMessage = "Post description is required")]
        [Display(Name = "Short Description")]
        [StringLength(240)]
        public string PostShortDesc { get; set; }

        [Required(ErrorMessage = "Post slug is required")]
        [Display(Name = "Url Slug")]
        [StringLength(150)]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug format not valid.")]
        public string PostSlug { get; set; }

        [Required(ErrorMessage = "Post content is required")]
        [Display(Name = "Content")]
        public string PostContent { get; set; }

        [Required(ErrorMessage = "Publish Date is required")]
        [Display(Name = "Date Published")]
        public DateTime PostDatePub { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Date Modified")]
        public DateTime PostLastModified { get; set; } = DateTime.UtcNow;

        [Display(Name = "Published?")]
        public bool isPublished { get; set; }

        public int CatID { get; set; }

        public Category PostCategory { get; set; }
    }
}
