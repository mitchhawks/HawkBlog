using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HawkBlog.Models
{
    public class Category
    {
        [Key]
        public int CatID { get; set; }

        [Required]
        [StringLength(60)]
        [Display(Name = "Category Name")]
        public string CatName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug format not valid.")]
        [Display(Name = "Category Slug")]
        public string CatUrlSlug { get; set; }

        [StringLength(450)]
        [Display(Name = "Category Description")]
        public string CatDesc { get; set; }

        public List<Post> PostsInCat { get; set; }
    }
}