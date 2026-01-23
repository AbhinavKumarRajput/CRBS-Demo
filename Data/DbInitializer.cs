using CRBS.Models;
using Microsoft.AspNetCore.Identity;

namespace CRBS.Data
{
    public class DbInitializer
    {
        public static async Task SeedRolesAndAdmin(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            string[] roles = { "Admin", "Manager", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            if (await userManager.FindByEmailAsync("admin@company.com") == null)
            {
                var adminUser = new AppUser
                {
                    Name = "Admin",
                    UserName = "admin@company.com",
                    Email = "admin@company.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            
        }
    }
}
