namespace HotelBookingSystem.ViewModel
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public string RoomType { get; set; }
        public string ImagePath { get; set; }

        public decimal Price { get; set; }
        public bool? AvailabilityStatus { get; set; }
    }

}
