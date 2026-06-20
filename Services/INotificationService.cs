using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSync_API.Services
{
    public interface INotificationService
    {
        Task SendNotificationToUsersAsync(IEnumerable<int> userIds, string title, string message);
    }
}
