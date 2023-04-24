using Microsoft.AspNetCore.SignalR;

namespace app_ui_server.Services.Hubs
{
    public class CarHub : Hub, IPublishHub
    {
        public async Task SendMessage(string message)
        {
            // send message to all clients
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
