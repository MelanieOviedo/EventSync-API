using EventSync_API.Models;
using EventSync_API.DTOs;

namespace EventSync_API.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int id);
        Task<Event> CreateEventAsync(Event @event);
        Task UpdateEventAsync(Event @event);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<EventAttendeeDto>> GetEventAttendeesAsync(int eventId);
    }
}