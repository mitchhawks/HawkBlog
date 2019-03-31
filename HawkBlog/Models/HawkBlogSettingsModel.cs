using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HawkBlog.Models
{
    public class HawkBlogSettingsModel
    {
        [Required]
        [Display(Name = "Site Name", Description = "The name of your site")]
        public string SiteName { get; set; }

        public string LogoPath { get; set; }

        [Display(Name = "Remove Photo?")]
        public bool RemovePhoto { get; set; }
    }
}
