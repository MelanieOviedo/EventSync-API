namespace EventSync_API.DTOs
{
    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int MaxCapacity { get; set; }
        public int AvailableSpots { get; set; }
        public string? ImagePath { get; set; }
    }
}