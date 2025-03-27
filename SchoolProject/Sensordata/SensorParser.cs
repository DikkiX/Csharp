namespace Schoolproject.Sensordata;
using System;
using System.Collections.Generic;

// Dit zorgt ervoor dat alle parsers een Parse(string data) methode moeten hebben.
public interface ISensorParser
{
    List<Dictionary<string, object>> Parse(string data);
}

// Hiermee parse je de ruwe string data naar een nette Dictionary met key-value pairs.
public class TemperatureSensorParser : ISensorParser
{
    public List<Dictionary<string, object>> Parse(string data)
    {
        var parsedDataList = new List<Dictionary<string, object>>();
        var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var parts = line.Split("name:");
            var serial = parts[0].Replace("serial:", "").Trim();
            var name = parts[1].Split("temperature:")[0].Trim();
            var temp = double.Parse(parts[1].Split("temperature:")[1].Split("humidity:")[0]);
            var humidity = double.Parse(parts[1].Split("humidity:")[1].Split("battery:")[0]);
            var battery = double.Parse(parts[1].Split("battery:")[1].Split("timestamp:")[0]);
            var timestamp = DateTime.Parse(parts[1].Split("timestamp:")[1]);

            var measurements = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "type", "Temperature" },
                    { "value", temp / 10 }, // Convert to correct temperature format
                    { "unit", "Celsius" },
                    { "timestamp", timestamp }
                },
                new Dictionary<string, object>
                {
                    { "type", "Humidity" },
                    { "value", humidity },
                    { "unit", "Percent" },
                    { "timestamp", timestamp }
                },
                new Dictionary<string, object>
                {
                    { "type", "Battery" },
                    { "value", battery },
                    { "unit", "Percent" },
                    { "timestamp", timestamp }
                }
            };

            var aggregations = new Dictionary<string, object>
            {
                { "temperature", new Dictionary<string, object>
                    {
                        { "max_today", temp / 10 + 5 },
                        { "min_today", temp / 10 - 5 },
                        { "unit", "Celsius" }
                    }
                },
                { "humidity", new Dictionary<string, object>
                    {
                        { "max_today", humidity + 10 },
                        { "min_today", humidity - 10 },
                        { "unit", "Percent" }
                    }
                }
            };

            var sensorData = new Dictionary<string, object>
            {
                { "id", serial },
                { "serial_number", serial },
                { "name", name },
                { "last_measurements", measurements },
                { "aggregations", aggregations },
                { "last_measurement_timestamp", timestamp }
            };

            parsedDataList.Add(sensorData);
            Console.WriteLine($"✅ Geparsed sensor: {name} met temperatuur: {temp/10}°C");
        }

        return parsedDataList;
    }
}

// Nu kan je gemakkelijk een parser kiezen en gebruiken zonder dat je de code moet aanpassen!
public class SensorParserContext
{
    private ISensorParser _parser;

    public void SetParser(ISensorParser parser)
    {
        _parser = parser;
    }

    // Verander hier de return type naar List<Dictionary<string, object>>
    public List<Dictionary<string, object>> Parse(string data)
    {
        return _parser?.Parse(data) ?? new List<Dictionary<string, object>>();
    }
}