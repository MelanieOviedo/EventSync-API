namespace EventSync_API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer"; // Options: "Admin", "Customer"
        
        /// <summary>
        /// Firebase Cloud Messaging Token for Push Notifications
        /// </summary>
        public string? FcmToken { get; set; } 
        
        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}