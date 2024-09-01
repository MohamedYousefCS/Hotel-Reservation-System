namespace HotelBookingSystem.ViewModel
{
    public class ErrorViewModel1
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string ErrorMessage { get; set; } // Add this property to hold the custom error message
    }

}
