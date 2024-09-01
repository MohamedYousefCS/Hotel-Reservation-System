using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Controllers
{
    public class RoomsController : Controller
    {
       

            private readonly HotelBookingSystemContext _context;

            public RoomsController(HotelBookingSystemContext context)
            {
                _context = context;
            }

            // GET: Rooms
            public IActionResult Index()
            {
                var rooms = _context.Rooms
                    .Include(r => r.Hotel)
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

                return View(rooms);
            }

            // GET: Rooms/Create
            public IActionResult Create()
            {
                ViewBag.Hotels = new SelectList(_context.Hotels, "Id", "Name");
                return View();
            }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room room, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imagePath = Path.Combine("wwwroot/images", imageFile.FileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    room.ImagePath = imageFile.FileName;
                }

                _context.Rooms.Add(room);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Hotels = new SelectList(_context.Hotels, "Id", "Name", room.HotelId);
            return View(room);
        }

      


        // GET: Rooms/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }

            ViewBag.Hotels = new SelectList(_context.Hotels, "Id", "Name", room.HotelId);
            return View(room);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Room updatedRoom, IFormFile imageFile)
        {
            if (id != updatedRoom.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var room = _context.Rooms.Find(id);
                    if (room == null)
                    {
                        return NotFound();
                    }

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imagePath = Path.Combine("wwwroot/images", imageFile.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            imageFile.CopyTo(stream);
                        }
                        updatedRoom.ImagePath = imageFile.FileName;
                    }

                    // Update the room properties
                    room.HotelId = updatedRoom.HotelId;
                    room.RoomType = updatedRoom.RoomType;
                    room.Price = updatedRoom.Price;
                    room.AvailabilityStatus = updatedRoom.AvailabilityStatus;
                    room.ImagePath = updatedRoom.ImagePath;

                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index");
            }

            ViewBag.Hotels = new SelectList(_context.Hotels, "Id", "Name", updatedRoom.HotelId);
            return View(updatedRoom);
        }


        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }
            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        }

    }

