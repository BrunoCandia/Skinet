﻿using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace API.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
        /// <summary>
        /// Key: ConnectionId, value: User email
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> UserConnections = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            var email = Context.User?.GetUserEmail();

            if (!string.IsNullOrWhiteSpace(email))
            {
                UserConnections[email] = Context.ConnectionId;
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var email = Context.User?.GetUserEmail();

            if (!string.IsNullOrWhiteSpace(email))
            {
                UserConnections.TryRemove(email, out _);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public static string? GetConnectionIdByEmail(string email)
        {
            UserConnections.TryGetValue(email, out var connectionId);

            return connectionId;
        }
    }
}
