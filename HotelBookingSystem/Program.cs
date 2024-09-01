using DinkToPdf.Contracts;
using DinkToPdf;
using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace HotelBookingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            // Register services in the container
            builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
            builder.Services.AddHttpContextAccessor(); // Add this line if you haven't already



            builder.Services.AddDistributedMemoryCache(); // 1 Adds a default in-memory implementation of IDistributedCache


            builder.Services.AddDbContext<HotelBookingSystemContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));


             builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

            builder.Services.AddDbContext<HotelBookingSystemContext>();

            //2
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // Ensure the session cookie is only accessible via HTTP
                options.Cookie.IsEssential = true; // Make the session cookie essential for the application
            });
       


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:Secretkey").Get<string>();

            app.UseAuthorization();

            app.UseSession(); //3 Enable session middleware


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
