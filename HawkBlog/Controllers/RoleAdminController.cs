using HawkBlog.Data;
using HawkBlog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TribalCardsCore.Controllers
{
    [Authorize]
    public class RoleAdminController : Controller
    {
        private readonly RoleManager<ApplicationRole> roleManager;

        public RoleAdminController(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<ApplicationRoleListViewModel> model = new List<ApplicationRoleListViewModel>();
            model = roleManager.Roles.Select(r => new ApplicationRoleListViewModel
            {
                RoleName = r.Name,
                Id = r.Id
            }).ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ApplicationRoleViewModel model = new ApplicationRoleViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole applicationRole = new ApplicationRole();

                applicationRole.Name = model.RoleName;
                IdentityResult roleResult = await roleManager.CreateAsync(applicationRole);
                if (roleResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            ApplicationRoleViewModel model = new ApplicationRoleViewModel();
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            ApplicationRole applicationRole = await roleManager.FindByIdAsync(id);
            if (applicationRole == null)
            {
                return NotFound();
            }

            model.Id = applicationRole.Id;
            model.RoleName = applicationRole.Name;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole applicationRole = await roleManager.FindByIdAsync(id);
                if (applicationRole == null)
                {
                    return NotFound();
                }
                applicationRole.Name = model.RoleName;
                IdentityResult roleResult = await roleManager.UpdateAsync(applicationRole);

                if (roleResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteApplicationRole(string id)
        {
            string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationRole applicationRole = await roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    name = applicationRole.Name;
                }
            }
            return PartialView("_DeleteApplicationRole", name);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteApplicationRole(string id, IFormCollection form)
        {
            if(!String.IsNullOrEmpty(id))
            {
                ApplicationRole applicationRole = await roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    IdentityResult roleRuslt = roleManager.DeleteAsync(applicationRole).Result;
                    if (roleRuslt.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                }
            return View();
        }
    }
}
