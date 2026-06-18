namespace EventSync_API.DTOs
{
    public class EventAttendeeDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
    }
}