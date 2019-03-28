using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HawkBlog.Data;
using HawkBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HawkBlog.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {

        public DashboardController(IOptionsSnapshot<HawkBlogSettings> settingsOptions) : base(settingsOptions)
        {
            
        }


        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Settings()
        {
            HawkBlogSettingsModel settings = new HawkBlogSettingsModel
            {
                SiteName = ViewData["SiteName"].ToString()
            };
            return View(settings);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateSettings([Bind("SiteName")] HawkBlogSettingsModel settings)
        {
            if (ModelState.IsValid)
            {
                JObject WebSettings = (JObject)JToken.FromObject(settings);
                // write JSON directly to a file
                try
                {
                    using (StreamWriter file = System.IO.File.CreateText("Config/HawkBlogSettings.json"))
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        await WebSettings.WriteToAsync(writer);
                        writer.Close();
                        file.Close();
                    }
                    await Task.Delay(300);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View(settings);
                }
            }

            return View(settings);
        }
    }
}