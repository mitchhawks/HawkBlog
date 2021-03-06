using HawkBlog.Data;
using HawkBlog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HawkBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserAdminController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public UserAdminController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptionsSnapshot<HawkBlogSettings> settingsOptions) : base(settingsOptions)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<UserListViewModel> model = new List<UserListViewModel>();
            ViewData["CurrentUserID"] = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            model = userManager.Users.Select(u => new UserListViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            }).ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            UserViewModel model = new UserViewModel();
            model.ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            }).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email
                };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    selectedRole = selectedRole ?? new string[] { };

                    string[] newRoles = selectedRole.ToArray<string>();

                    foreach (string role in newRoles)
                    {
                        ApplicationRole roleToAdd = await roleManager.FindByIdAsync(role);
                        result = await userManager.AddToRoleAsync(user, roleToAdd.Name);
                    }
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            
            EditUserViewModel model = new EditUserViewModel();
            
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser user = await userManager.FindByIdAsync(id);
                var userRoles = await userManager.GetRolesAsync(user);
                if (user != null)
                {
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.Email = user.Email;
                    model.ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem
                    {
                        Selected = userRoles.Contains(r.Name),
                        Text = r.Name,
                        Value = r.Id
                    }).ToList();
                    try
                    {
                        model.ApplicationRoleId = roleManager.Roles.Single(r => r.Name == userManager.GetRolesAsync(user).Result.Single()).Id;
                    }
                    catch
                    {
                        ViewData["roleInfo"] = "error";
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    string[] existingRoles = userManager.GetRolesAsync(user).Result.ToArray();
                    
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        selectedRole = selectedRole ?? new string[] { };
                        
                        
                        result = await userManager.RemoveFromRolesAsync(user, existingRoles.Except(selectedRole).ToArray<string>());

                        if (!result.Succeeded)
                        {
                            return View();
                        }
                        
                        string[] newRoles = selectedRole.Except(existingRoles).ToArray<string>();
                        
                        foreach (string role in newRoles)
                        {
                            ApplicationRole roleToAdd = await roleManager.FindByIdAsync(role);
                            result = await userManager.AddToRoleAsync(user, roleToAdd.Name);
                        }

                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                    
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            string userInfo = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser applicationUser = await userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    userInfo = applicationUser.FirstName + " " + applicationUser.LastName + " with email " + applicationUser.Email;
                }
            }
            return View("Delete", userInfo);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, IFormCollection form)
        {
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser applicationUser = await userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    IdentityResult result = await userManager.DeleteAsync(applicationUser); 
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }
    }
   
}
