using CRBS.Models;
using CRBS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRBS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        public readonly RoleManager<IdentityRole> roleManager;
        public UserManager<AppUser> userManager;

        public UserManagementController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users =await userManager.Users.ToListAsync();
            List<ProfileViewModel> userList = new List<ProfileViewModel>();
            if (users == null || users.Count == 0)
            {
                TempData["Error"] = "No users found.";
                return RedirectToAction("Index","Account");
            }
            
            
            foreach (var user in users)
            {
                    var roles = await userManager.GetRolesAsync(user);
                    ProfileViewModel userModel = new ProfileViewModel
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = roles.FirstOrDefault() ?? "No Role Assigned"
                    };
                    userList.Add(userModel);
                    //user.Role = roles.FirstOrDefault() ?? "No Role Assigned";
            }
            
            return View(userList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = roleManager.Roles.Select(r=>r.Name).ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await userManager.FindByEmailAsync(model.Email) == null)
                {
                    var user = new AppUser
                    {
                        Name = model.Name,
                        UserName = model.Email,
                        Email = model.Email,
                        EmailConfirmed = true
                    };
                    string password = "Pass@123";
                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, model.Role);
                        TempData["Success"] = "User created successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewData["Error"] = "Some thing went wrong....";
                    }
                }
                else
                {
                    ViewData["Error"] = "User Already Esxist....";
                }

            }
            ViewBag.Roles = roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }
            var roles = await userManager.GetRolesAsync(user);
            var model = new ProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = roles.FirstOrDefault()
            };
            ViewBag.Roles = roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }
                user.Name = model.Name;
                user.Email = model.Email;
                user.UserName = model.Email;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    if (!userRoles.Contains(model.Role))
                    {
                        await userManager.RemoveFromRolesAsync(user, userRoles);
                        await userManager.AddToRoleAsync(user, model.Role);
                    }
                    TempData["Success"] = "User updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["Error"] = "Some thing went wrong....";
                }
            }
            ViewBag.Roles = roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }
            var roles = await userManager.GetRolesAsync(user);
            var model = new ProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = roles.FirstOrDefault()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }
            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "User deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Some thing went wrong....";
            }
            return RedirectToAction("Index");
        }
    }
}
