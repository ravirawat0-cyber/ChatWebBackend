﻿using ChatAPIs.DataService;
using ChatAPIs.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPIs.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SharedDb _shared;

        public ChatHub(SharedDb shared) => _shared = shared;
        public async Task JoinChat(UserConnections con)
        {
            await Clients.All
                .SendAsync("ReceiveMessage", "admin", $"{con.Username} has joined");

            await SendNotification($"{con.Username} joined the chat room");
        }

        public async Task JoinSpecificChatRoom(UserConnections conn)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatRoom);

            _shared.Connections[Context.ConnectionId] = conn;

            await Clients.Group(conn.ChatRoom)
                .SendAsync("ReceiveMessage", "admin", $"{conn.Username} has joined {conn.ChatRoom}");

            await SendNotification($"{conn.Username} joined {conn.ChatRoom}");
        }


        public async Task SendMessage(string msg)
        {
            if (_shared.Connections.TryGetValue(Context.ConnectionId, out UserConnections conn))
            {
                await Clients.Groups(conn.ChatRoom)
                    .SendAsync("ReceiveSpecificMessage", conn.Username, msg);
                await SendNotification($"New message from {conn.Username}: {msg}");
            }
        }

        private async Task SendNotification(string notification)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }
}
