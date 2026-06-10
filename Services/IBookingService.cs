using EventSync_API.DTOs;

namespace EventSync_API.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDto?> CreateBookingAsync(int eventId, int userId);
    }
}