using CRBS.Data;
using CRBS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRBS.Controllers
{
    [Authorize(Roles = "Admin,Manager,Employee")]
    public class ConferenceRoomController : Controller
    {
        private readonly AppDbContext context;

        public ConferenceRoomController(AppDbContext context)
        {
            this.context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index()
        {
            var rooms = context.ConferenceRooms.ToList();
            return View(rooms);
        }

        [HttpGet]
        public IActionResult ActiveRooms()
        {
            var activeRooms = context.ConferenceRooms
                                     .Where(r => r.IsActive)
                                     .ToList();
            return View(activeRooms);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(ConferenceRoom room)
        {
            if (ModelState.IsValid)
            {
                context.ConferenceRooms.Add(room);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var room = context.ConferenceRooms.Find(id);
            if (room == null)
            {
                TempData["Error"] = "Conference Room not found.";
                return RedirectToAction("Index");
            }
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(ConferenceRoom room)
        {
            if (ModelState.IsValid)
            {
                context.ConferenceRooms.Update(room);
                var res = await context.SaveChangesAsync();

                if (res > 0)
                {
                    TempData["Success"] = "Conference Room updated successfully.";
                }
                else
                {
                    TempData["Error"] = "No changes were made to the Conference Room.";
                }
                return RedirectToAction("Index");
            }
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var room = context.ConferenceRooms.Find(id);
            if (room == null)
            {
                TempData["Error"] = "Conference Room not found.";
                return RedirectToAction("Index");
            }
            else
            {
                context.ConferenceRooms.Remove(room);
                var res = await context.SaveChangesAsync();
                if (res > 0)
                {
                    TempData["Success"] = "Conference Room deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Error occurred while deleting the Conference Room.";
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Details(int id)
        {
            var room = context.ConferenceRooms.FirstOrDefault(r => r.RoomId == id);
            if (room == null)
            {
                TempData["Error"] = "Conference Room not found.";
                return RedirectToAction("Index");
            }
            return View(room);
        }
    }
}
