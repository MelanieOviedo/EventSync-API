using EventSync_API.DTOs;
using EventSync_API.Models;

namespace EventSync_API.Mappers
{
    public static class EventMapper
    {
        public static EventResponseDto ToEventResponse(Event @event)
        {
            return new EventResponseDto
            {
                Id = @event.Id,
                Title = @event.Title,
                Description = @event.Description,
                Date = @event.Date,
                MaxCapacity = @event.MaxCapacity,
                AvailableSpots = @event.AvailableSpots,
                ImagePath = string.IsNullOrEmpty(@event.ImagePath) 
                    ? null 
                    : @event.ImagePath.Replace("wwwroot/", "").Replace("wwwroot\\", "").Replace("\\", "/")
            };
        }
    }
}