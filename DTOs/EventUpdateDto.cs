using Microsoft.AspNetCore.Http;

namespace EventSync_API.DTOs
{
    public class EventUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int MaxCapacity { get; set; }
        
        public IFormFile? Image { get; set; }
        public string? ImagePath { get; set; } // Se asigna internamente tras guardar el archivo
    }
}