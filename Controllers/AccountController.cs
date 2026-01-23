using CRBS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CRBS.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AppUser appUser)
        {
            
            if(appUser.Email == null && appUser.PasswordHash == null)
            {
                ViewData["Message"] = "Email and Password are required";
            }
            else if (appUser.Email == null)
            {
                    ViewData["Message"] = "Email Id is required";
            }
            else if (appUser.PasswordHash == null)
            {
                ViewData["Message"] = "Password is required";
            }
            else
            {
                if (appUser != null)
                {
                    var usr = await userManager.FindByEmailAsync(appUser.Email);
                    if (usr != null)
                    {
                        var res = await signInManager.PasswordSignInAsync(usr, appUser.PasswordHash,false,false);
                        if (res.Succeeded)
                        {
                            if (await userManager.IsInRoleAsync(usr, "Admin"))
                            {
                                return RedirectToAction("Dashboard", "Admin");
                            }
                            else if (await userManager.IsInRoleAsync(usr, "Manager"))
                            {
                                return RedirectToAction("Dashboard", "Manager");
                            }

                            return RedirectToAction("Dashboard", "Employee");
                        }
                        else
                        {
                            ViewData["Message"] = "Invalid Password";
                        }
                    }
                    else
                    {
                        ViewData["Message"] = "Invalid Email";
                    }
                }
                else
                {
                    ViewData["Message"] = "Something went wrong";
                }
            }
            return View(appUser);

        }

        public async Task<IActionResult> Logout()
        {
            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "Account");
        }

        public IActionResult AccessDenied() => View();
    }
}
