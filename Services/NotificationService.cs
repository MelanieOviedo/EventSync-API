using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            // TODO: Integrar FCM real aquí utilizando FirebaseAdmin SDK.
            //
            // Ejemplo de implementación con FirebaseAdmin:
            // 
            // var multicastMessage = new FirebaseAdmin.Messaging.MulticastMessage()
            // {
            //     Tokens = fcmTokens,
            //     Notification = new FirebaseAdmin.Messaging.Notification()
            //     {
            //         Title = title,
            //         Body = message
            //     },
            //     Android = new FirebaseAdmin.Messaging.AndroidConfig()
            //     {
            //         Priority = FirebaseAdmin.Messaging.Priority.High,
            //         Notification = new FirebaseAdmin.Messaging.AndroidNotification()
            //         {
            //             Sound = "default"
            //         }
            //     }
            // };
            // 
            // var response = await FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.SendMulticastAsync(multicastMessage);
            // System.Diagnostics.Debug.WriteLine($"Notificaciones push enviadas: {response.SuccessCount} exitosas, {response.FailureCount} fallidas.");

            await Task.CompletedTask;
        }
    }
}
