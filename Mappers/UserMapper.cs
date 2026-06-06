using EventSync_API.DTOs;
using EventSync_API.Models;

namespace EventSync_API.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToUserResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}