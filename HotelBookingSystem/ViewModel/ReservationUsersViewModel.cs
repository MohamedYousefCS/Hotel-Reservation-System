namespace HotelBookingSystem.ViewModel
{
    public class ReservationUsersViewModel
    {

        public int Id { get; set; }
        public string UserName { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; }
    }
}
