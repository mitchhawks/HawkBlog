using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HawkBlog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace HawkBlog.Controllers
{
    public class BaseController : Controller
    {
        protected readonly HawkBlogSettings _settings;

        public BaseController(IOptionsSnapshot<HawkBlogSettings> settingsOptions)
        {
            _settings = settingsOptions.Value;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["SiteName"] = _settings.SiteName;
            ViewData["LogoPath"] = _settings.LogoPath;
            base.OnActionExecuting(filterContext);
        }
    }
}