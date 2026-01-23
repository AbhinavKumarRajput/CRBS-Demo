using CRBS.Data;
using CRBS.Models;
using System.Linq;

namespace CRBS.Services
{
    public class BookingCleanupService
    {
        private readonly AppDbContext _context;

        public BookingCleanupService(AppDbContext context)
        {
            _context = context;
        }

        public void CancelOldPendingBookings(int days = 2)
        {
            DateOnly cancelBeforeDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-days));

            var oldPendingBookings = _context.Bookings
                                             .Where(b => b.Status == "Pending" &&
                                                         b.BookingDate <= cancelBeforeDate)
                                             .ToList();

            if (oldPendingBookings.Any())
            {
                foreach (var booking in oldPendingBookings)
                {
                    booking.Status = "Cancelled";
                }

                _context.SaveChanges();
            }
        }
    }
}
