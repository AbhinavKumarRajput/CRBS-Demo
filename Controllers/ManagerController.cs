using CRBS.Data;
using CRBS.Models;
using CRBS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CRBS.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext context;

        public ManagerController(UserManager<AppUser> userManager, AppDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public IActionResult Dashboard()
        {
            ViewBag.TotalTeamBookings = context.Bookings.Count();
            ViewBag.UpcomingMeetings = context.Bookings
                .Count(b => b.BookingDate >= DateOnly.FromDateTime(DateTime.Today));

            ViewBag.ActiveRooms = context.ConferenceRooms.Count(r => r.IsActive);
            ViewBag.EmployeeCount = context.Users.Count();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

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
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");
            // 🔴 CHECK CURRENT PASSWORD
            var isCurrentPasswordValid =
                await userManager.CheckPasswordAsync(user, model.CurrentPassword);

            if (!isCurrentPasswordValid)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View(model);
            }
            var result = await userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            if (result.Succeeded)
            {
                TempData["Success"] = "Password changed successfully.";
                return RedirectToAction("Profile");
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                string name = user.Name;
                ViewBag.Name = name;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string name)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else if (ModelState.IsValid)
            {
                user.Name = name;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "User Updated Successfully.....";
                    return RedirectToAction("Profile");
                }
                else
                {
                    ViewData["Error"] = "Something went wrong";
                }
            }
            else
            {
                ViewData["Error"] = "Invalid data";
            }
            return View();
        }
    }
}
