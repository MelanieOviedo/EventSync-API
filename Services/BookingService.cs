using EventSync_API.DTOs;
using EventSync_API.Models;
using EventSync_API.Repositories;
using EventSync_API.Data;
using Microsoft.EntityFrameworkCore;

namespace EventSync_API.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEventRepository _eventRepository;
        private readonly AppDbContext _context; // Necesario para la transacción o actualización directa

        public BookingService(IBookingRepository bookingRepository, IEventRepository eventRepository, AppDbContext context)
        {
            _bookingRepository = bookingRepository;
            _eventRepository = eventRepository;
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Event) // Incluimos los detalles del evento
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<BookingResponseDto?> CreateBookingAsync(int eventId, int userId)
        {
            var @event = await _eventRepository.GetEventByIdAsync(eventId);
            
            if (@event == null || @event.AvailableSpots <= 0)
            {
                return null;
            }

            // Crear la reserva
            var booking = new Booking
            {
                EventId = eventId,
                UserId = userId,
                BookingDate = DateTime.Now,
                Status = "Confirmed"
            };

            // Reducir cupos disponibles
            @event.AvailableSpots -= 1;
            
            // Guardar cambios
            await _bookingRepository.CreateBookingAsync(booking);
            _context.Events.Update(@event);
            await _context.SaveChangesAsync();

            return new BookingResponseDto
            {
                Message = "¡Reserva realizada con éxito!"
            };
        }
    }
}