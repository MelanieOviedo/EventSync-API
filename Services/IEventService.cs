using EventSync_API.DTOs;

namespace EventSync_API.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventResponseDto>> GetAllEventsAsync();
        Task<EventResponseDto?> GetEventByIdAsync(int id);
        Task<IEnumerable<EventAttendeeDto>> GetEventAttendeesAsync(int eventId);
        Task<EventResponseDto> CreateEventAsync(EventCreateDto eventDto);
        Task<bool> UpdateEventAsync(int id, EventUpdateDto eventDto);
        Task<bool> DeleteEventAsync(int id);
    }
}