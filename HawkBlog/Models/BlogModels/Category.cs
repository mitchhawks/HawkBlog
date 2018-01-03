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

        public string CatName { get; set; }

        public string CatUrlSlug { get; set; }

        public string CatDesc { get; set; }

        public List<Post> PostsInCat { get; set; }
    }
}