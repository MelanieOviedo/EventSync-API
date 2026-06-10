using Microsoft.AspNetCore.Mvc;
using EventSync_API.DTOs;
using EventSync_API.Services;

namespace EventSync_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] BookingRequestDto request)
        {
            // Nota: Por ahora usamos un UserId fijo (1) hasta que implementemos JWT completo
            // para obtener el ID del usuario autenticado.
            int userId = 1; 

            var response = await _bookingService.CreateBookingAsync(request.EventId, userId);

            if (response == null)
            {
                return BadRequest(new { message = "No se pudo realizar la reserva. Verifique si hay cupos disponibles o si el evento existe." });
            }

            return Ok(response);
        }
    }
}