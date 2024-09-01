using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelBookingSystem.Controllers
{
    public class UserController : Controller
    {


        private readonly HotelBookingSystemContext _context;

        public UserController(HotelBookingSystemContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var hotels = _context.Hotels.Include(h => h.Rooms).ToList();
            return View(hotels);
        }

        // GET: Testimonials/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Hotels = new SelectList(_context.Hotels, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Testimonial testimonial)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not logged in
            }

            testimonial.UserId = userId.Value; // Set the UserId from session

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Testimonials.Add(testimonial);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    // Log error and return view with model
                    Console.WriteLine($"Error: {ex.Message}");
                    ViewBag.Hotels = new SelectList(_context.Hotels, "Id", "Name", testimonial.HotelId);
                    return View(testimonial);
                }
            }

            ViewBag.Hotels = new SelectList(_context.Hotels, "Id", "Name", testimonial.HotelId);
            return View(testimonial);
        }

        public IActionResult ViewAllHotels()
        {
            var hotels = _context.Hotels.Include(h => h.Rooms).ToList();
            return View(hotels);
        }








    }
}
