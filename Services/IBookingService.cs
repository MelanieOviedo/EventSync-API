using EventSync_API.DTOs;
using EventSync_API.Models;

namespace EventSync_API.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId);
        Task<BookingResponseDto?> CreateBookingAsync(int eventId, int userId);
    }
}