using EventSync_API.DTOs;
using EventSync_API.Repositories;
using EventSync_API.Mappers;
using EventSync_API.Models;

namespace EventSync_API.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly INotificationService _notificationService;

        public EventService(IEventRepository eventRepository, INotificationService notificationService)
        {
            _eventRepository = eventRepository;
            _notificationService = notificationService;
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

        public async Task<IEnumerable<EventAttendeeDto>> GetEventAttendeesAsync(int eventId)
        {
            return await _eventRepository.GetEventAttendeesAsync(eventId);
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

        public async Task<bool> UpdateEventAsync(int id, EventUpdateDto eventDto)
        {
            var @event = await _eventRepository.GetEventByIdAsync(id);
            if (@event == null) return false;

            bool isDateChanged = @event.Date != eventDto.Date;
            bool isDescriptionChanged = @event.Description != eventDto.Description;
            bool isTitleChanged = @event.Title != eventDto.Title;
            bool hasImportantChanges = isDateChanged || isDescriptionChanged || isTitleChanged;

            string oldTitle = @event.Title;
            DateTime oldDate = @event.Date;

            @event.Title = eventDto.Title;
            @event.Description = eventDto.Description;
            @event.Date = eventDto.Date;

            int capacityDiff = eventDto.MaxCapacity - @event.MaxCapacity;
            @event.MaxCapacity = eventDto.MaxCapacity;
            @event.AvailableSpots += capacityDiff;

            if (eventDto.ImagePath != null)
            {
                @event.ImagePath = eventDto.ImagePath;
            }

            await _eventRepository.UpdateEventAsync(@event);

            if (hasImportantChanges)
            {
                var attendees = await _eventRepository.GetEventAttendeesAsync(id);
                var userIds = attendees.Select(a => a.UserId).ToList();

                if (userIds.Any())
                {
                    var changeDetails = new List<string>();
                    if (isTitleChanged) changeDetails.Add($"título (ahora: '{eventDto.Title}')");
                    if (isDateChanged) changeDetails.Add($"fecha/hora (ahora: {eventDto.Date:dd/MM/yyyy HH:mm})");
                    if (isDescriptionChanged) changeDetails.Add("descripción");

                    string title = $"Actualización de evento: {eventDto.Title}";
                    string message = $"El evento '{oldTitle}' (programado originalmente para el {oldDate:dd/MM/yyyy HH:mm}) ha sido modificado. Cambios en: {string.Join(", ", changeDetails)}.";

                    try
                    {
                        await _notificationService.SendNotificationToUsersAsync(userIds, title, message);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al notificar actualización del evento: {ex.Message}");
                    }
                }
            }

            return true;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var @event = await _eventRepository.GetEventByIdAsync(id);
            if (@event == null) return false;

            var attendees = await _eventRepository.GetEventAttendeesAsync(id);
            var userIds = attendees.Select(a => a.UserId).ToList();

            await _eventRepository.DeleteEventAsync(id);

            if (userIds.Any())
            {
                string title = $"Evento Cancelado: {@event.Title}";
                string message = $"El evento '{@event.Title}' programado para el {@event.Date:dd/MM/yyyy HH:mm} ha sido cancelado.";

                try
                {
                    await _notificationService.SendNotificationToUsersAsync(userIds, title, message);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al notificar cancelación del evento: {ex.Message}");
                }
            }

            return true;
        }
    }
}