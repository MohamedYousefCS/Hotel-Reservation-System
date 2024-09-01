using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace HotelBookingSystem.Controllers
{
    public class AccountController : Controller
    {
        

        private readonly HotelBookingSystemContext db;

        public AccountController(HotelBookingSystemContext db)
        {
            this.db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (user != null)
            {
                // Set session values
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserRole", user.Role); // Store user role in session

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (user.Role == "User")
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password, 
                    Role = "User"
                };

                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(model);
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();


            return RedirectToAction("Login", "Account");
        }




        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = db.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Address= user.Address,
                Phone = user.Phone,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
                {
                    if (user.Password == model.CurrentPassword)
                    {
                        user.Password = model.NewPassword;
                    }
                    else
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                        return View(model);
                    }
                }

                user.Name = model.Name;
                user.Email = model.Email;
                user.Phone = model.Phone;
                user.Address = model.Address;
                
                db.SaveChanges();

                HttpContext.Session.SetString("UserName", user.Name);

                ViewBag.Message = "Profile updated successfully!";

                if(model.Role=="Admin")
                return RedirectToAction("Dashboard", "Admin");
                else if(model.Role=="User")
                return RedirectToAction("Index", "Home");


            }

            return View(model);
        }




    }

}

