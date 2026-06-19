using Microsoft.EntityFrameworkCore;
using EventSync_API.Data;
using EventSync_API.Models;

namespace EventSync_API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendNotificationToUsersAsync(IEnumerable<int> userIds, string title, string message)
        {
            var userIdList = userIds.Distinct().ToList();
            if (!userIdList.Any()) return;

            // 1. Guardar notificaciones en la base de datos local (in-app notifications)
            var notifications = userIdList.Select(userId => new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                SentDate = DateTime.UtcNow,
                IsRead = false
            }).ToList();

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            // 2. Preparar el envío de notificaciones push de FCM
            // Obtenemos los tokens FCM de los usuarios correspondientes
            var userTokens = await _context.Users
                .Where(u => userIdList.Contains(u.Id) && !string.IsNullOrEmpty(u.FcmToken))
                .Select(u => new { u.Id, u.FcmToken })
                .ToListAsync();

            if (userTokens.Any())
            {
                var tokens = userTokens.Select(ut => ut.FcmToken!).ToList();
                await SendPushNotificationViaFcmAsync(tokens, title, message);
            }
        }

        private async Task SendPushNotificationViaFcmAsync(List<string> fcmTokens, string title, string message)
        {
            if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
            {
                System.Diagnostics.Debug.WriteLine("FirebaseApp no ha sido inicializado. Verifica que 'service-account.json' esté en la raíz del proyecto.");
                return;
            }

            try
            {
                var multicastMessage = new FirebaseAdmin.Messaging.MulticastMessage()
                {
                    Tokens = fcmTokens,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = title,
                        Body = message
                    },
                    Android = new FirebaseAdmin.Messaging.AndroidConfig()
                    {
                        Priority = FirebaseAdmin.Messaging.Priority.High,
                        Notification = new FirebaseAdmin.Messaging.AndroidNotification()
                        {
                            Sound = "default"
                        }
                    },
                    Apns = new FirebaseAdmin.Messaging.ApnsConfig()
                    {
                        Headers = new Dictionary<string, string>()
                        {
                            { "apns-priority", "10" }
                        },
                        Aps = new FirebaseAdmin.Messaging.Aps()
                        {
                            Sound = "default"
                        }
                    }
                };

                var response = await FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(multicastMessage);
                System.Diagnostics.Debug.WriteLine($"Notificaciones push enviadas: {response.SuccessCount} exitosas, {response.FailureCount} fallidas.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar notificaciones FCM: {ex.Message}");
            }
        }
    }
}
