namespace Schoolproject.Sensordata;

using System;
using System.Collections.Generic;
using System.IO;

// Importeren van sensordata.txt

public sealed class SensorDataSingleton
{
    private static SensorDataSingleton instance = null;
    private static readonly object padlock = new object();
    private SensorParserContext _parserContext;

    // Constructor van de Singleton: wordt slechts één keer aangemaakt
    private SensorDataSingleton()
    {
        //Console.WriteLine("✅ SensorDataSingleton wordt aangemaakt!"); // <-- DEBUG OUTPUT
        _parserContext = new SensorParserContext();
        _parserContext.SetParser(new TemperatureSensorParser()); // Gebruik de juiste parser

        string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
        string path = Path.Combine(projectRoot, "Sensordata", "sensorData.txt");
        Console.WriteLine($"📂 Verwachte locatie van sensorData.txt: {path}"); // 🔍 DEBUG OUTPUT

        // Controleer of sensorData.txt bestaat
        if (File.Exists(path))
        {
            Console.WriteLine("📂 sensorData.txt gevonden!");
            string content = File.ReadAllText(path); // Lees de inhoud van sensorData.txt
            var parsedData = _parserContext.Parse(content); // Parse de ruwe tekst naar key-value data

            Console.WriteLine("🔄 Parsed Data:");
            // Print de ingelezen data naar de console
            foreach (var entry in parsedData)
            {
                Console.WriteLine("🔹 Nieuwe meting:");
                foreach (var kv in entry)
                {
                    Console.WriteLine($"{kv.Key}: {kv.Value}");
                }
            }
        }
        else
        {
            Console.WriteLine("⚠️ sensorData.txt NIET gevonden!");
        }
    }

    // Singleton Instance: zorgt ervoor dat er maar één instantie van SensorDataSingleton is
    public static SensorDataSingleton Instance
    {
        get
        {
            lock (padlock) // Zorgt ervoor dat er geen twee instanties tegelijk worden aangemaakt (thread-safe)
            {
                if (instance == null)
                {
                    instance = new SensorDataSingleton();
                }
                return instance;
            }
        }
    }
}