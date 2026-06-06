using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventSync_API.Data;
using EventSync_API.Models;
using EventSync_API.DTOs;

namespace EventSync_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Event)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(BookingCreateDto bookingDto)
        {
            // Basic validation for Module 3
            var targetEvent = await _context.Events.FindAsync(bookingDto.EventId);
            if (targetEvent == null || targetEvent.AvailableSpots <= 0)
            {
                return BadRequest("No spots available or event not found.");
            }

            var booking = new Booking
            {
                UserId = bookingDto.UserId,
                EventId = bookingDto.EventId,
                BookingDate = DateTime.UtcNow,
                Status = "Active"
            };

            targetEvent.AvailableSpots--; // Decrease spots
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, booking);
        }
    }
}