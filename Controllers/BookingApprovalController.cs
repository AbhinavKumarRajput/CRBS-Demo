using CRBS.Data;
using CRBS.Helper;
using CRBS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRBS.Controllers
{
    [Authorize(Roles ="Admin,Manager")]
    public class BookingApprovalController : Controller
    {
        private readonly AppDbContext context;

        private readonly BookingCleanupService _cleanupService;

        public BookingApprovalController(AppDbContext context, BookingCleanupService cleanupService)
        {
            this.context = context;
            this._cleanupService = cleanupService;
        }

        public IActionResult Index()
        {
            _cleanupService.CancelOldPendingBookings();
            var pendingBookings = context.Bookings
                                         .Where(b => b.Status == "Pending").Include(b => b.Room).Include(b => b.User).ToList();
            return View(pendingBookings);
        }

        public IActionResult Approve(int id)
        {
            var booking = context.Bookings
                         .Include(b => b.User).Include(b => b.Room)         
                         .FirstOrDefault(b => b.BookingId == id);

            if (booking == null) {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Index");
            }

            // 🔹 Current Date & Time
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            TimeOnly nowTime = TimeOnly.FromDateTime(DateTime.Now);

            // ❌ Past Date
            if (booking.BookingDate < today)
            {
                TempData["Error"] = "Cannot approve past date booking.";
                return RedirectToAction("Index");
            }

            // ❌ Same date but past time
            if (booking.BookingDate == today && booking.ToTime <= nowTime)
            {
                TempData["Error"] = "Cannot approve booking with past time.";
                return RedirectToAction("Index");
            }

            if (booking.Status == "Pending")
            {
                booking.Status = "Approved";
                context.SaveChanges();

                string subject = "Conference Room Booking Approved";

                string message = $@"
                    Dear {booking.User.Name}, <br/><br/>

                    Your conference room booking has been <b>approved</b>.<br/><br/>

                    <b>Room Name:</b> {booking.Room.RoomName}<br/>
                    <b>Date:</b> {booking.BookingDate}<br/>
                    <b>From:</b> {booking.FromTime}<br/>
                    <b>To:</b> {booking.ToTime}<br/><br/>

                    Regards,<br/>
                    Management Team
                    ";

                EmailHelper.Send(booking.User.Email, subject, message);
                TempData["Success"] = "Booking approved successfully.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Reject(int id)
        {
            var booking = context.Bookings
                         .Include(b => b.User).Include(b => b.Room)
                         .FirstOrDefault(b => b.BookingId == id);
            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Index");
            }
            // Current Date & Time
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            TimeOnly nowTime = TimeOnly.FromDateTime(DateTime.Now);
            // ❌ Past Date
            if (booking.BookingDate < today)
            {
                TempData["Error"] = "Cannot reject past date booking.";
                return RedirectToAction("Index");
            }
            // ❌ Same date but past time
            if (booking.BookingDate == today && booking.ToTime <= nowTime)
            {
                TempData["Error"] = "Cannot reject booking with past time.";
                return RedirectToAction("Index");
            }
            if (booking != null && booking.Status == "Pending")
            {
                booking.Status = "Rejected";
                context.SaveChanges();
                string subject = "Conference Room Booking Rejected";
                string message = $@"
                    Dear {booking.User.Name}, <br/><br/>
                    We regret to inform you that your conference room booking has been <b>rejected</b>.<br/><br/>
                    <b>Room Name:</b> {booking.Room.RoomName}<br/>
                    <b>Date:</b> {booking.BookingDate}<br/>
                    <b>From:</b> {booking.FromTime}<br/>
                    <b>To:</b> {booking.ToTime}<br/><br/>
                    Please contact the management team for further details.<br/><br/>
                    Regards,<br/>
                    Management Team
                    ";
                EmailHelper.Send(booking.User.Email, subject, message);
                TempData["Success"] = "Booking rejected successfully.";
            }
            return RedirectToAction("Index");
        }
    }
}
