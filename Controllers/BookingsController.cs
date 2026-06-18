using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EventSync_API.DTOs;
using EventSync_API.Services;

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
        public async Task<ActionResult<IEnumerable<BookingHistoryResponseDto>>> GetMyBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            var bookings = await _bookingService.GetUserBookingsAsync(userId);

            var response = bookings.Select(b => new BookingHistoryResponseDto
            {
                Id = b.Id,
                EventId = b.EventId,
                EventTitle = b.Event?.Title ?? "Evento no encontrado",
                EventDate = b.Event?.Date.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty,
                BookingDate = b.BookingDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                Status = b.Status
            });

            return Ok(response);
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