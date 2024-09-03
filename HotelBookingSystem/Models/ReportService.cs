namespace HotelBookingSystem.Models
{
    public class ReportService
    {
        private readonly HotelBookingSystemContext _context;

        public ReportService(HotelBookingSystemContext context)
        {
            _context = context;
        }

        public List<HotelRevenueReport> GenerateHotelRevenueReport(DateTime startDate, DateTime endDate)
        {
            var report = _context.Hotels
                .Select(hotel => new HotelRevenueReport
                {
                    HotelName = hotel.Name,
                    Location = hotel.Location,
                    TotalRevenue = hotel.Rooms
                        .SelectMany(room => room.Reservations)
                        .Where(reservation => reservation.CheckInDate >= startDate && reservation.CheckOutDate <= endDate)
                        .Sum(reservation => reservation.TotalPrice),
                    TotalBookings = hotel.Rooms
                        .SelectMany(room => room.Reservations)
                        .Where(reservation => reservation.CheckInDate >= startDate && reservation.CheckOutDate <= endDate)
                        .Count()
                })
                .ToList();

            return report;
        }
    }

}
