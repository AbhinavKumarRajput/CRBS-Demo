using CRBS.Data;
using CRBS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRBS.Controllers
{
    [Authorize(Roles = "Admin,Manager,Employee")]
    public class BookingController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext context;


        public BookingController(UserManager<AppUser> userManager, AppDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public IActionResult Index()
        {
            var bookings = context.Bookings
                                  .Include(b => b.Room)
                                  .Where(b => b.UserId == userManager.GetUserId(User)) // optional
                                  .OrderByDescending(b => b.BookingDate)
                                  .ToList();

            return View(bookings);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Rooms = context.ConferenceRooms
                                .Where(r => r.IsActive)
                                .ToList();

            var booking = new Booking
            {
                BookingDate = DateOnly.FromDateTime(DateTime.Today),
               
            };
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Booking booking)
        {
            ViewBag.Rooms = context.ConferenceRooms
                                .Where(r => r.IsActive)
                                .ToList();
            if (booking.RoomId == 0 || booking.BookingDate == default ||  booking.FromTime == default ||  booking.ToTime == default)
            {
                ViewData["Error"] = "Please fill all required fields.";
                return View(booking);
            }
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");
            if (booking.BookingDate < DateOnly.FromDateTime(DateTime.Today))
            {
                ViewData["Error"] = "Booking date cannot be in the past.";
                return View(booking);
            }
            if (booking.FromTime >= booking.ToTime)
            {
                ViewData["Error"] = "From Time must be earlier than To Time.";
                return View(booking);
            }
            if(booking.ToTime == booking.FromTime)
            {
                ViewData["Error"] = "From Time and To Time cannot be the same.";
                return View(booking);
            }
            if(booking.BookingDate == DateOnly.FromDateTime(DateTime.Today))
            {
                var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                if(booking.FromTime <= currentTime)
                {
                    ViewData["Error"] = "From Time must be later than current time.";
                    return View(booking);
                }
            }


            booking.UserId = user.Id;
            booking.Status = "Pending";

            context.Bookings.Add(booking);
            var result = await context.SaveChangesAsync();
            if (result > 0)
            {
                TempData["Success"] = "Booking created successfully.";
            }
            else
            {
                TempData["Error"] = "Booking Failed.Please try again....";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) { 
                TempData["Error"] = "Booking Failed.Please try again....";
                return RedirectToAction(nameof(Index));
            }
            else if (booking.Status == "Approved")
            {
                TempData["Error"] = "Approved bookings cannot be edited.";
                return RedirectToAction(nameof(Index));
                // ✅ same page, error show
            }
            else if(booking.Status == "Rejected")
            {
                TempData["Error"] = "Rejected booking cannot be edited.";
                return RedirectToAction(nameof(Index));
            }

                // Active rooms dropdown ke liye
                ViewBag.Rooms = context.ConferenceRooms
                    .Where(r => r.IsActive)
                    .ToList();

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Booking booking)
        {
            ViewBag.Rooms = context.ConferenceRooms
                .Where(r => r.IsActive)
                .ToList();

            if (booking.RoomId == 0 || booking.BookingDate == default || booking.FromTime == default || booking.ToTime == default)
            {
                ViewData["Error"] = "Please fill all required fields.";
                return View(booking);
            }

            var dbBooking = await context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);

            if (dbBooking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction(nameof(Index));
            }
            

            else
            {
                if (booking.BookingDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    ViewData["Error"] = "Booking date cannot be in the past.";
                }
                else if (booking.FromTime >= booking.ToTime)
                {
                    ViewData["Error"] = "From Time must be earlier than To Time.";
                    // ✅ same page, error show
                }
                else if (booking.ToTime == booking.FromTime)
                {
                    ViewData["Error"] = "From Time and To Time cannot be the same.";
                     // ✅ same page, error show
                }
                else if (booking.BookingDate == DateOnly.FromDateTime(DateTime.Today))
                {
                    var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                    if (booking.FromTime <= currentTime)
                    {
                        ViewData["Error"] = "From Time must be later than current time.";
                         // ✅ same page, error show
                    }
                }
                
                else
                {
                    // 🔴 NO CHANGE CHECK (IMPORTANT)
                    bool isSame =
                        dbBooking.RoomId == booking.RoomId &&
                        dbBooking.BookingDate == booking.BookingDate &&
                        dbBooking.FromTime == booking.FromTime &&
                        dbBooking.ToTime == booking.ToTime;

                    if (isSame)
                    {
                        ViewData["Error"] = "No changes detected. Please modify at least one field.";
                        // ✅ same page, error show
                    }
                    else
                    {
                        // 🔹 Update fields
                        dbBooking.RoomId = booking.RoomId;
                        dbBooking.BookingDate = booking.BookingDate;
                        dbBooking.FromTime = booking.FromTime;
                        dbBooking.ToTime = booking.ToTime;

                        var result = await context.SaveChangesAsync();
                        if (result <= 0)
                        {
                            TempData["Error"] = "Booking update failed. Please try again.";

                        }
                        else
                        {
                            TempData["Success"] = "Booking updated successfully.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    
                }

                   

            }
            
            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var booking = await context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                TempData["Error"] = "Booking Failed.Please try again....";
                return RedirectToAction(nameof(Index));
            }

            return View(booking);

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await context.Bookings.FindAsync(id);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
            }
            else
            {
                if(booking.Status=="Approved")
                {
                    TempData["Error"] = "Approved bookings cannot be deleted.";
                }
                else
                {
                    context.Bookings.Remove(booking);
                    var result = await context.SaveChangesAsync();
                    if (result <= 0)
                    {
                        TempData["Error"] = "Booking deletion failed. Please try again.";
                    }
                    else
                    {

                        TempData["Success"] = "Booking deleted successfully.";
                    }
                }

            }
            return RedirectToAction(nameof(Index));
        }

        


    }
}
