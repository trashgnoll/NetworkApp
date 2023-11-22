using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace NetworkApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string fromUser, string toUser, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", fromUser, toUser, message);
        }

    }
}
