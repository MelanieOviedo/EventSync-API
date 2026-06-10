using EventSync_API.DTOs;

namespace EventSync_API.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventResponseDto>> GetAllEventsAsync();
        Task<EventResponseDto?> GetEventByIdAsync(int id);
        Task<EventResponseDto> CreateEventAsync(EventCreateDto eventDto);
    }
}