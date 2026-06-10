using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventSync_API.Data;
using EventSync_API.Models;
using EventSync_API.DTOs;
using EventSync_API.Services;
using System.Globalization;

namespace EventSync_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventResponseDto>> GetEvent(int id)
        {
            var @event = await _eventService.GetEventByIdAsync(id);

            if (@event == null)
            {
                return NotFound(new { message = "Evento no encontrado" });
            }

            return Ok(@event);
        }

        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> PostEvent(EventCreateDto eventDto)
        {
            var createdEvent = await _eventService.CreateEventAsync(eventDto);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
        }
    }
}