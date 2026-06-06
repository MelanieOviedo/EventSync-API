namespace EventSync_API.DTOs
{
    public class EventCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int MaxCapacity { get; set; }
        public string? ImagePath { get; set; }
    }
}