using Microsoft.AspNetCore.SignalR;
using Schoolproject.Models;


public class SensorHub : Hub
{
    public async Task SendSensorData(SensorData data)
    {
        await Clients.All.SendAsync("ReceiveSensorData", data);
    }
}