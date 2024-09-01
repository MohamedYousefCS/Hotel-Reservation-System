using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Controllers
{
    public class HomeController : Controller
    {

        private readonly HotelBookingSystemContext _context;

        public HomeController(HotelBookingSystemContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var availableRooms = _context.Rooms
                .Include(r => r.Hotel)
                .Where(r => r.AvailabilityStatus.Value == true) // Filter rooms based on AvailabilityStatus
                .Select(r => new RoomViewModel
                {
                    Id = r.Id,
                    HotelId = r.HotelId,
                    HotelName = r.Hotel.Name,
                    RoomType = r.RoomType,
                    ImagePath = r.ImagePath,
                    Price = r.Price,
                    AvailabilityStatus = r.AvailabilityStatus
                }).ToList();

            return View(availableRooms);
        }




        [HttpPost]
        public IActionResult SubmitContact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var contact = new Contact
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Message = model.Message,
                    CreatedAt = DateTime.Now
                };

                _context.Contacts.Add(contact);
                _context.SaveChanges();

                return RedirectToAction("ContactSuccess");
            }

            return View(model);
        }

        public IActionResult ContactList()
        {
            var contacts = _context.Contacts.ToList();
            return View(contacts);
        }

        public IActionResult ContactSuccess()
        {
            return View();
        }


    }

}

