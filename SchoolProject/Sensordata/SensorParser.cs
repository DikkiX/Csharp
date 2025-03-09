// Sensordata/SensorParser.cs
namespace Schoolproject.Sensordata;
using System;
using System.Collections.Generic;

//Dit zorgt ervoor dat alle parsers een Parse(string data) methode moeten hebben.

public interface ISensorParser
{
    List<Dictionary<string, string>> Parse(string data);
}

//Hiermee parse je de ruwe string data naar een nette Dictionary met key-value pairs.
public class TemperatureSensorParser : ISensorParser
{
    public List<Dictionary<string, string>> Parse(string data)
    {
        var parsedDataList = new List<Dictionary<string, string>>();
        var keys = new[] { "serial", "bat", "temp", "state", "batmax", "batmin", "hum", "manu", "type", "error", "manufac", "batterylevel", "serialnumber", "v" };

        Console.WriteLine("🔍 Begonnen met parsen...");

        int index = 0;
        while (index < data.Length)
        {
            var entryData = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                int keyIndex = data.IndexOf(key + ":", index);
                if (keyIndex != -1)
                {
                    int nextKeyIndex = data.Length;
                    foreach (var nextKey in keys)
                    {
                        if (nextKey != key)
                        {
                            int tempIndex = data.IndexOf(nextKey + ":", keyIndex + key.Length);
                            if (tempIndex != -1 && tempIndex < nextKeyIndex)
                            {
                                nextKeyIndex = tempIndex;
                            }
                        }
                    }

                    string value = data.Substring(keyIndex + key.Length + 1, nextKeyIndex - (keyIndex + key.Length + 1)).Trim();
                    entryData[key] = value;
                    Console.WriteLine($"✅ Geparsed: {key} -> {value}");
                }
            }

            if (entryData.Count > 0)
            {
                parsedDataList.Add(entryData);
            }

            index = data.IndexOf("serial:", index + 1);
            if (index == -1) break; // Stop als er geen nieuwe meting begint
        }

        return parsedDataList;
    }
}

//Nu kan je gemakkelijk een parser kiezen en gebruiken zonder dat je de code moet aanpassen!
public class SensorParserContext
{
    private ISensorParser _parser;

    public void SetParser(ISensorParser parser)
    {
        _parser = parser;
    }

    public List<Dictionary<string, string>> Parse(string data)
    {
        return _parser?.Parse(data) ?? new List<Dictionary<string, string>>();
    }
}
