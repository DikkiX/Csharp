using Schoolproject.Models;  // Zorg ervoor dat het correct naar de Models namespace verwijst
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Schoolproject.Sensordata
{
    public class SensorDataSingleton
    {
        private static SensorDataSingleton instance = null;
        private static readonly object padlock = new object();
        private List<SensorData> _sensors;

        private SensorDataSingleton()
        {
            _sensors = new List<SensorData>();
            LoadData();  // Laad data vanuit sensorData.txt
        }

        public static SensorDataSingleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SensorDataSingleton();
                    }
                    return instance;
                }
            }
        }

        public List<SensorData> GetSensors() => _sensors;

        private void LoadData()
        {
            try
            {
                if (!File.Exists("sensorData.txt"))
                {
                    Console.WriteLine("Bestand sensorData.txt niet gevonden.");
                    return;
                }

                var lines = File.ReadAllLines("sensorData.txt");
                foreach (var line in lines)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        var data = line.Split("name:");
                        if (data.Length < 2) continue;

                        var serialNumber = ExtractValue(data[0], "serial:");
                        var name = ExtractValue(data[1], "", "temperature:");  // ✅ FIXED
                        var temperature = ParseDouble(ExtractValue(data[1], "temperature:", "humidity:"));
                        var humidity = ParseDouble(ExtractValue(data[1], "humidity:", "battery:"));
                        var battery = ParseDouble(ExtractValue(data[1], "battery:", "timestamp:"));
                        var timestamp = DateTime.Parse(ExtractValue(data[1], "timestamp:").Trim());

                        _sensors.Add(new SensorData
                        {
                            SerialNumber = serialNumber,
                            Name = name,
                            Temperature = temperature,
                            Humidity = humidity,
                            Battery = battery,
                            Timestamp = timestamp
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fout bij verwerken van een regel: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij het inladen van sensordata: {ex.Message}");
            }
        }

        private string ExtractValue(string input, string start, string end = null)
        {
            var startIndex = string.IsNullOrEmpty(start) ? 0 : input.IndexOf(start, StringComparison.OrdinalIgnoreCase);
            if (startIndex == -1) return "";

            startIndex += start.Length;
            var endIndex = end != null ? input.IndexOf(end, startIndex, StringComparison.OrdinalIgnoreCase) : -1;

            return endIndex == -1 ? input[startIndex..].Trim() : input[startIndex..endIndex].Trim();
        }

        private double ParseDouble(string value)
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;

            return 0.0;  // Standaardwaarde bij fout
        }
    }
}
