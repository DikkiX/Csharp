public interface ISensorState
{
    string GetStatus();
    void HandleMeasurement(Dictionary<string, object> sensorData);
    string GetStatusColor();
}