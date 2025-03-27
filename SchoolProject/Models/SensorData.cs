namespace Schoolproject.Models
{
    public class SensorData
    {
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Battery { get; set; }
        public DateTime Timestamp { get; set; }
    }
}