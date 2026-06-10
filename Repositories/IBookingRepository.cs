using EventSync_API.Models;

namespace EventSync_API.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking> CreateBookingAsync(Booking booking);
    }
}