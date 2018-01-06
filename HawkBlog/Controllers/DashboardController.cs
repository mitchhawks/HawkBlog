using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HawkBlog.Data;
using HawkBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HawkBlog.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

    }
}