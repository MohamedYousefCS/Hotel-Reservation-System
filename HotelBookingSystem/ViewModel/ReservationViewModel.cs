using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem.ViewModel
{
    public class ReservationViewModel
    {
        public List<int> RoomIds { get; set; } = new List<int>(); // Store multiple room IDs
        public List<string> RoomTypes { get; set; } = new List<string>();
        public int HotelId { get; set; }
        public decimal Price { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        public decimal TotalPrice => (CheckOutDate - CheckInDate).Days * Price * RoomIds.Count;
    }

}
