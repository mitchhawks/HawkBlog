using System.ComponentModel.DataAnnotations;

namespace HawkBlog.Models
{
    public class ApplicationRoleViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
