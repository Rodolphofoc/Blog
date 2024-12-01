using Microsoft.AspNetCore.SignalR;


namespace Applications.Socket
{
    public class PostHub : Hub
    {
        public async Task SendPostAddedMessage(string message)
        {
            await Clients.All.SendAsync("ReceivePostAdded", message);
        }
    }
}
