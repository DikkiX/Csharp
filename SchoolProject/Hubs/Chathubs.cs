using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        // Deze methode wordt gebruikt om de sensor data naar de frontend te sturen
        public async Task SendSensorData(object data)
        {
            // Verzenden van de sensor data naar alle verbonden clients
            await Clients.All.SendAsync("ReceiveSensorData", data);
        }

        // Optioneel: Als je berichten wilt sturen zoals in je oorspronkelijke code, kun je de oude methode behouden.
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}