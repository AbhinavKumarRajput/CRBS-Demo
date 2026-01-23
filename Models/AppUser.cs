using Microsoft.AspNetCore.Identity;

namespace CRBS.Models
{
    public class AppUser: IdentityUser
    {
        public string Name { get; set; }
    }
}
