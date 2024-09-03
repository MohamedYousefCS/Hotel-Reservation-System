namespace HotelBookingSystem.Models
{
    public class HotelRevenueReport
    {
        public string HotelName { get; set; }  // اسم الفندق
        public string Location { get; set; }  // موقع الفندق
        public decimal TotalRevenue { get; set; }  // إجمالي الأرباح
        public int TotalBookings { get; set; }  // إجمالي عدد الحجوزات
    }

}
