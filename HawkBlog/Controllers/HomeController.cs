using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HawkBlog.Models;
using Microsoft.Extensions.Options;
using HawkBlog.Data;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace HawkBlog.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IOptionsSnapshot<HawkBlogSettings> settingsOptions) : base(settingsOptions)
        {

        }

        public IActionResult Index()
        {
            
            return RedirectToAction("", "Blog");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
