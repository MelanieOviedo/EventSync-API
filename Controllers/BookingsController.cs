using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EventSync_API.DTOs;
using EventSync_API.Services;
using EventSync_API.Models;

namespace EventSync_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetMyBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] BookingRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var response = await _bookingService.CreateBookingAsync(request.EventId, userId);

            if (response == null)
            {
                return BadRequest(new { message = "No se pudo realizar la reserva. Verifique si hay cupos disponibles o si el evento existe." });
            }

            return Ok(response);
        }
    }
}