using EventSync_API.DTOs;
using EventSync_API.Repositories;
using EventSync_API.Mappers;
using EventSync_API.Models;

namespace EventSync_API.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<EventResponseDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            // Filtramos solo los eventos futuros y los ordenamos cronológicamente
            return events
                .Where(e => e.Date >= DateTime.Now)
                .OrderBy(e => e.Date)
                .Select(e => EventMapper.ToEventResponse(e));
        }

        public async Task<EventResponseDto?> GetEventByIdAsync(int id)
        {
            var @event = await _eventRepository.GetEventByIdAsync(id);
            if (@event == null) return null;
            return EventMapper.ToEventResponse(@event);
        }

        public async Task<EventResponseDto> CreateEventAsync(EventCreateDto eventDto)
        {
            var @event = new Event
            {
                Title = eventDto.Title,
                Description = eventDto.Description,
                Date = eventDto.Date,
                MaxCapacity = eventDto.MaxCapacity,
                AvailableSpots = eventDto.MaxCapacity, // Inicialmente todos los cupos disponibles
                ImagePath = eventDto.ImagePath
            };

            var createdEvent = await _eventRepository.CreateEventAsync(@event);
            return EventMapper.ToEventResponse(createdEvent);
        }
    }
}