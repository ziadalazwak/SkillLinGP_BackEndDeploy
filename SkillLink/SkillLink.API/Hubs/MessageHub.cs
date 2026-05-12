using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SkillLink.API.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        // ASP.NET Core SignalR's DefaultUserIdProvider automatically handles ClaimTypes.NameIdentifier
        // which we correctly map to the Domain UserId in the JWT generator.
        
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
