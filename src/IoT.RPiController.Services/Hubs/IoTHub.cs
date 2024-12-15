using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IoT.RPiController.Services.Hubs
{
    [Authorize]
    public class IoTHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
