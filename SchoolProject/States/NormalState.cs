public class NormalState : ISensorState
{
    public string GetStatus() => "Normal";
    public string GetStatusColor() => "green";
    
    public void HandleMeasurement(Dictionary<string, object> sensorData)
    {
        // Normal operatie logic
        Console.WriteLine($"Sensor {sensorData["name"]} opereert normaal");
    }
}