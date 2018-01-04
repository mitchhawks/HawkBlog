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
        public string CatName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug format not valid.")]
        public string CatUrlSlug { get; set; }

        [StringLength(450)]
        public string CatDesc { get; set; }

        public List<Post> PostsInCat { get; set; }
    }
}