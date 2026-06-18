namespace EventSync_API.DTOs
{
    public class BookingHistoryResponseDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string EventDate { get; set; } = string.Empty;
        public string BookingDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}