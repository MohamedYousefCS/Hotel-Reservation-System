using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelBookingSystem.Controllers
{
    public class AdminController : Controller
    {

        private readonly HotelBookingSystemContext db;

        public AdminController(HotelBookingSystemContext db)
        {
            this.db = db;
        }

        public IActionResult Dashboard()
        {
            var model = new AdminDashboardViewModel
            {
                RegisteredUsersCount = db.Users.Count(),
                AvailableRoomsCount = db.Rooms.Count(r => r.AvailabilityStatus.Value),
                BookedRoomsCount = db.Rooms.Count(r => !r.AvailabilityStatus.Value),
                TotalHotelsCount=db.Hotels.Count(),
                TotalMessage=db.Contacts.Count()
            };
            return View(model);
        }



        public IActionResult RegisteredUsers()
        {
            var users = db.Users
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Address = u.Address,
                    Phone = u.Phone,
                    Role = u.Role
                }).ToList();

            return View(users);
        }


        public IActionResult ViewReservations()
        {
            var reservations = db.Reservations
                .Select(reservation => new ReservationUsersViewModel
                {
                    Id = reservation.Id,
                    UserName = db.Users.FirstOrDefault(user => user.Id == reservation.UserId).Name,
                    RoomId = reservation.RoomId,
                    CheckInDate = reservation.CheckInDate,
                    CheckOutDate = reservation.CheckOutDate,
                    TotalPrice = reservation.TotalPrice,
                    PaymentStatus = reservation.PaymentStatus
                })
                .ToList();

            return View(reservations);
        }




        public IActionResult GenerateReport(string reportType)
        {
            var report = new Report
            {
                AdminId = GetCurrentUserId(),
                ReportType = reportType,
                GeneratedOn = DateTime.Now
            };

           
            GenerateReportFile(report);

            db.Reports.Add(report);
            db.SaveChanges();

            return RedirectToAction("Dashboard");
        }


        private int GetCurrentUserId()
        {
          
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        private void GenerateReportFile(Report report)
        {
        
        }



    }
}

