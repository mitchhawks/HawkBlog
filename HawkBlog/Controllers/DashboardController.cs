using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HawkBlog.Data;
using HawkBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private IHostingEnvironment _environment;

        public DashboardController(IOptionsSnapshot<HawkBlogSettings> settingsOptions, IHostingEnvironment environment) : base(settingsOptions)
        {
            _environment = environment;
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
        public async Task<IActionResult> UpdateSettings([Bind("SiteName,RemovePhoto")] HawkBlogSettingsModel settings, ICollection<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                
                // write JSON directly to a file
                try
                {
                    var uploads = Path.Combine(_environment.WebRootPath, "images/");
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            using (var fileStream = new FileStream(Path.Combine(uploads, "logo" + Path.GetExtension(file.FileName)), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                settings.LogoPath = "/images/logo" + Path.GetExtension(file.FileName);
                            }
                            
                        }
                    }
                    if (files.Count == 0)
                    {
                        settings.LogoPath = _settings.LogoPath;
                    }
                    if (settings.RemovePhoto)
                    {
                        settings.LogoPath = null;
                    }
                    JObject WebSettings = (JObject)JToken.FromObject(settings);

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
                catch (Exception e)
                {
                    if (_environment.EnvironmentName == "Development")
                    {
                        ViewData["ErrorMessage"] = e.Message;
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "An error has occured. Please check your input and try again";
                    }
                    return View("Settings", settings);
                }
            }

            ViewData["ErrorMessage"] = "An error has occured. Please check your input and try again";
            return View("Settings", settings);
        }
    }
}