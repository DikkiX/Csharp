using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Schoolproject.Sensordata;
using Schoolproject.Models;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class SensorController : ControllerBase
{
    private readonly IHubContext<SensorHub> _hubContext;
    private readonly SensorDataSingleton _sensorDataSingleton;

    public SensorController(IHubContext<SensorHub> hubContext)
    {
        _hubContext = hubContext;
        _sensorDataSingleton = SensorDataSingleton.Instance;
    }

    [HttpGet]
    public IActionResult GetSensors()
    {
        var sensors = _sensorDataSingleton.GetSensors()
            .Select(sensor => new
            {
                id = sensor.SerialNumber,
                serial_number = sensor.SerialNumber,
                name = sensor.Name,
                last_measurements = new[]
                {
                    new { type = "Temperature", value = sensor.Temperature, unit = "Celsius", timestamp = sensor.Timestamp },
                    new { type = "Humidity", value = sensor.Humidity, unit = "Percent", timestamp = sensor.Timestamp },
                    new { type = "Battery", value = sensor.Battery, unit = "Percent", timestamp = sensor.Timestamp }
                },
                aggregations = new
                {
                    temperature = new { max_today = sensor.Temperature + 5, min_today = sensor.Temperature - 5, unit = "Celsius" },
                    humidity = new { max_today = sensor.Humidity + 10, min_today = sensor.Humidity - 10, unit = "Percent" }
                },
                last_measurement_timestamp = sensor.Timestamp
            })
            .ToList();

        return Ok(new { items = sensors });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSensor([FromBody] SensorData sensor)
    {
        var existing = _sensorDataSingleton.GetSensors().FirstOrDefault(s => s.SerialNumber == sensor.SerialNumber);
        if (existing != null)
        {
            existing.Temperature = sensor.Temperature;
            existing.Humidity = sensor.Humidity;
            existing.Battery = sensor.Battery;
            existing.Timestamp = sensor.Timestamp;
        }
        else
        {
            _sensorDataSingleton.GetSensors().Add(sensor);
        }

        // Verzenden naar frontend via SignalR
        await _hubContext.Clients.All.SendAsync("ReceiveSensorData", new
        {
            id = sensor.SerialNumber,
            serial_number = sensor.SerialNumber,
            name = sensor.Name,
            last_measurements = new[]
            {
                new { type = "Temperature", value = sensor.Temperature, unit = "Celsius", timestamp = sensor.Timestamp },
                new { type = "Humidity", value = sensor.Humidity, unit = "Percent", timestamp = sensor.Timestamp },
                new { type = "Battery", value = sensor.Battery, unit = "Percent", timestamp = sensor.Timestamp }
            },
            aggregations = new
            {
                temperature = new { max_today = sensor.Temperature + 5, min_today = sensor.Temperature - 5, unit = "Celsius" },
                humidity = new { max_today = sensor.Humidity + 10, min_today = sensor.Humidity - 10, unit = "Percent" }
            },
            last_measurement_timestamp = sensor.Timestamp
        });

        return Ok(sensor);
    }
}
