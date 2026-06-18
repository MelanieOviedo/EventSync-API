using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventSync_API.DTOs;
using EventSync_API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace EventSync_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IWebHostEnvironment _env;

        public EventsController(IEventService eventService, IWebHostEnvironment env)
        {
            _eventService = eventService;
            _env = env;
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> PostEvent([FromForm] EventCreateDto eventDto)
        {
            if (eventDto.Image != null)
            {
                eventDto.ImagePath = await SaveImage(eventDto.Image);
            }

            var createdEvent = await _eventService.CreateEventAsync(eventDto);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(int id, [FromForm] EventUpdateDto eventDto)
        {
            var existingEvent = await _eventService.GetEventByIdAsync(id);
            if (existingEvent == null) return NotFound(new { message = "Evento no encontrado" });

            if (eventDto.Image != null)
            {
                // Nota: Podrías implementar lógica para borrar la imagen anterior aquí si lo deseas
                eventDto.ImagePath = await SaveImage(eventDto.Image);
            }

            var updated = await _eventService.UpdateEventAsync(id, eventDto);
            if (!updated) return BadRequest(new { message = "No se pudo actualizar el evento" });

            return NoContent();
        }

        [HttpGet("{id}/attendees")]
        public async Task<ActionResult<IEnumerable<EventAttendeeDto>>> GetEventAttendees(int id)
        {
            var attendees = await _eventService.GetEventAttendeesAsync(id);
            
            return Ok(attendees);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var deleted = await _eventService.DeleteEventAsync(id);
            if (!deleted) return NotFound(new { message = "Evento no encontrado" });

            return NoContent();
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            // Retornamos la ruta relativa que el EventMapper espera (incluyendo wwwroot para el replace)
            return Path.Combine("wwwroot/images", uniqueFileName).Replace("\\", "/");
        }
    }
}