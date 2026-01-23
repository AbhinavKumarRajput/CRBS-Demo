using CRBS.Data;
using CRBS.Models;
using CRBS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRBS.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext context;

        public EmployeeController(UserManager<AppUser> userManager, AppDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await userManager.GetUserAsync(User);
            var totalBookings = await context.Bookings
                                       .CountAsync(b => b.UserId == user.Id);
            ViewBag.TotalBookings = totalBookings;

            var UpcomingBookingsCount = await context.Bookings
                                               .CountAsync(b => b.UserId == user.Id &&
                                                                b.BookingDate >= DateOnly.FromDateTime(DateTime.Today) &&
                                                                (b.Status == "Approved" || b.Status == "Pending"));
            ViewBag.UpcomingBookingsCount = UpcomingBookingsCount;

            var activeRoomsCount = await context.ConferenceRooms
                                          .CountAsync(r => r.IsActive);
            ViewBag.ActiveRoomsCount = activeRoomsCount;

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
                    return RedirectToAction("Profile", "Employee");
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
