using CRBS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRBS.Data
{
    public class AppDbContext:IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<ConferenceRoom> ConferenceRooms { get; set; }

        public DbSet<Booking> Bookings { get; set; }
    }
}
