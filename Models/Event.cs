namespace EventSync_API.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int MaxCapacity { get; set; }
        public int AvailableSpots { get; set; }
        
        /// <summary>
        /// Stores the relative physical path of the uploaded image
        /// </summary>
        public string? ImagePath { get; set; } 
        
        // Navigation properties
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}