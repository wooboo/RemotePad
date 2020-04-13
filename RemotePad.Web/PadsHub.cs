using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemotePad.Web
{
    [Authorize]
    public class PadsHub : Hub
    {
        public Task RequestHosts()
        {
            return Clients.User(this.Context.UserIdentifier).SendAsync("requestHost");
        }
        public Task SendHost(string id, PadHost host)
        {
            return Clients.User(this.Context.UserIdentifier).SendAsync("receiveHost", id, host);
        }
    }

    public class PadHost
    {
        public string Name { get; set; }
    }
}
