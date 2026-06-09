using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventSync_API.Data;
using EventSync_API.Models;
using EventSync_API.DTOs;
using System.Globalization;

namespace EventSync_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public EventsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetEvents()
        {
            var events = await _context.Events.ToListAsync();
            
            var response = events.Select(e => new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date,
                MaxCapacity = e.MaxCapacity,
                AvailableSpots = e.AvailableSpots,
                // Enviamos la ruta relativa limpia (sin wwwroot y con / en lugar de \)
                ImagePath = string.IsNullOrEmpty(e.ImagePath) 
                    ? null 
                    : e.ImagePath.Replace("wwwroot/", "").Replace("wwwroot\\", "").Replace("\\", "/")
            }).ToList();

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(EventCreateDto eventDto)
        {
            var @event = new Event
            {
                Title = eventDto.Title,
                Description = eventDto.Description,
                Date = eventDto.Date,
                MaxCapacity = eventDto.MaxCapacity,
                AvailableSpots = eventDto.MaxCapacity,
                ImagePath = eventDto.ImagePath
            };

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvents), new { id = @event.Id }, @event);
        }
    }
}