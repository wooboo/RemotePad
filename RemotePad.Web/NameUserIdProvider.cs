﻿using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace RemotePad.Web
{
    internal class NameUserIdProvider: IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}