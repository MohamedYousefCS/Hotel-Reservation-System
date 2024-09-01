using DinkToPdf;
using DinkToPdf.Contracts;
using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Diagnostics;
using System.Net.Mail;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering; // تأكد من إضافة هذا التوجيه






namespace HotelBookingSystem.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly HotelBookingSystemContext db;
        private readonly IConverter _converter;
        private readonly IViewRenderService _viewRenderService;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;


        public ReservationsController(HotelBookingSystemContext context, IRazorViewEngine viewEngine,
    ITempDataProvider tempDataProvider,
    IConverter converter,
    IViewRenderService viewRenderService)
        {
            db = context;
            _converter = converter;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _viewRenderService = viewRenderService ?? throw new ArgumentNullException(nameof(viewRenderService));
        }



        public IActionResult Book(int roomId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var room = db.Rooms.Find(roomId);
            if (room == null)
            {
                return NotFound();
            }

            // Check if the user has already booked the same room for the same date
            var existingReservation = db.Reservations
                .FirstOrDefault(r => r.UserId == userId.Value
                                     && r.RoomId == roomId
                                     && r.CheckInDate == DateTime.Today
                                     && r.CheckOutDate == DateTime.Today.AddDays(1));

            if (existingReservation != null)
            {
                // Optional: Redirect to an error page or display an error message
                ViewBag.ErrorMessage = "You have already booked this room for the selected dates.";
                return View("Error");
            }

            var reservation = new Reservation
            {
                UserId = userId.Value,
                RoomId = roomId,
                CheckInDate = DateTime.Today, // Assuming same-day check-in, modify as necessary
                CheckOutDate = DateTime.Today.AddDays(1), // Modify the checkout date as needed
                TotalPrice = room.Price, // Assuming price is per night, calculate as needed
                PaymentStatus = "Pending" // Set the default payment status
            };

            db.Reservations.Add(reservation);
            db.SaveChanges();

            return RedirectToAction("Payment", new { reservationId = reservation.Id });
        }





        public IActionResult MyReservations()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var reservations = db.Reservations
                .Include(r => r.Room)
                .ThenInclude(room => room.Hotel) // Eagerly load Hotel details
                .Where(r => r.UserId == userId.Value)
                .ToList();

            var viewModel = new UserReservationsViewModel
            {
                Reservations = reservations,
                Rooms = reservations.Select(r => r.Room).Distinct().ToList()
            };

            return View(viewModel);
        }


        public IActionResult Error(string message)
        {
            var errorViewModel = new ErrorViewModel1
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = message ?? "An unexpected error occurred."
            };

            return View(errorViewModel); // Ensure the model is not null
        }




        public IActionResult Payment(int reservationId)
        {
            var reservation = db.Reservations
                .Include(r => r.Room)
                .ThenInclude(r => r.Hotel)
                .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession(int reservationId)
        {
            var reservation = db.Reservations
                .Include(r => r.Room)
                .ThenInclude(r => r.Hotel)
                .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
            {
                return NotFound();
            }

            var domain ="https://localhost:7017/";

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(reservation.Room.Price * 100), // Price in cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = reservation.Room.RoomType,
                        },
                    },
                    Quantity = 1,
                },
            },
                Mode = "payment",
                SuccessUrl = domain + "Reservations/Confirmation?reservationId=" + reservationId,
                CancelUrl = domain + "Reservations/Payment?reservationId=" + reservationId,
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }













[HttpPost]
public async Task<IActionResult> ProcessPayment(int reservationId)
{
    var reservation = db.Reservations.Find(reservationId);
    if (reservation == null)
    {
        return NotFound();
    }

    // افترض نجاح الدفع
    reservation.PaymentStatus = "Paid";
    db.SaveChanges();

    // إنشاء HTML للفاتورة
    var htmlContent = await GetViewHtmlAsync("Invoice", reservation);

    // تحويل HTML إلى PDF باستخدام DinkToPdf
    var pdfDocument = new HtmlToPdfDocument
    {
        GlobalSettings = {
            DocumentTitle = "Invoice",
            PaperSize = PaperKind.A4
        },
        Objects = {
            new ObjectSettings
            {
                HtmlContent = htmlContent
            }
        }
    };

    var pdf = _converter.Convert(pdfDocument);

    // إرسال الفاتورة عبر البريد الإلكتروني
    await SendInvoiceEmailAsync(reservation, pdf);

    return RedirectToAction("Confirmation", new { reservationId = reservationId });
}



        public IActionResult Confirmation(int reservationId)
        {
            var reservation = db.Reservations
                .Include(r => r.Room)
                .ThenInclude(r => r.Hotel)
                .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
            {
                return NotFound();
            }

            // Check the payment status (assuming it's handled by Stripe and updated in the session)
            if (reservation.PaymentStatus != "Paid")
            {
                reservation.PaymentStatus = "Paid";
                db.SaveChanges();
            }

            return View(reservation);
        }





        //----------------------------------------------------------



        private async Task<string> GetViewHtmlAsync(string viewName, object model)
        {
            var actionContext = new ActionContext
            {
                HttpContext = HttpContext,
                RouteData = RouteData,
                ActionDescriptor = ControllerContext.ActionDescriptor
            };

            var viewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: false);

            if (!viewResult.Success)
            {
                throw new ArgumentException($"The view '{viewName}' was not found.");
            }

            using (var writer = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
                    new TempDataDictionary(HttpContext, _tempDataProvider),
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return writer.ToString();
            }
        }





        private async Task SendInvoiceEmailAsync(Reservation reservation, byte[] pdf)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Hotel Booking System", "mohamedyousefhoutar@gmail.com"));
                message.To.Add(new MailboxAddress("Recipient Name", "mohamedyousefcs@gmail.com"));// Replace with recipient email
                message.Subject = "Your Reservation Invoice";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
        <p>شكراً لك على حجزك.</p>
        <p><a href='https://localhost:7017/Reservations/DownloadInvoice?reservationId={reservation.Id}'>اضغط هنا</a> لعرض أو تنزيل الفاتورة الخاصة بك.</p>"
                };

                bodyBuilder.Attachments.Add("invoice.pdf", pdf);

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("mohamedyousefhoutar@gmail.com", "Mohamed_12000");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the error as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }



    }

}
