namespace EventSync_API.Models
{
    public class Booking
    {
        public int Id { get; set; }
        
        // Relationship with User
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        // Relationship with Event
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Status of the reservation: "Active", "Cancelled"
        /// </summary>
        public string Status { get; set; } = "Active"; 
    }
}