using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Schoolproject.Sensordata;

namespace Schoolproject.Services
{
    public class FileWatcherService
    {
        private readonly string _filePath; // Pad naar sensorData.txt
        private readonly IHubContext<SensorHub> _hubContext; // SignalR hub
        private FileSystemWatcher _fileWatcher; // De FileWatcher

        public FileWatcherService(IHubContext<SensorHub> hubContext)
        {
            // Het juiste pad bepalen naar sensorData.txt in de root van het project
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "sensorData.txt");
            _hubContext = hubContext;
        }

        public void StartWatching()
        {
            // Controleer of het bestand bestaat
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"Bestand niet gevonden op pad {_filePath}");
                return;
            }

            Console.WriteLine($"FileWatcherService gestart. Monitoring: {_filePath}");

            // FileWatcher configureren
            _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(_filePath))
            {
                Filter = Path.GetFileName(_filePath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size // Detecteer wijzigingen in het bestand
            };

            _fileWatcher.Changed += async (sender, e) =>
            {
                Console.WriteLine($"Wijziging gedetecteerd in: {e.FullPath}");
                await OnFileChanged();
            };

            _fileWatcher.EnableRaisingEvents = true; // Start het luisteren naar wijzigingen
        }

        private async Task OnFileChanged()
        {
            try
            {
                // Wacht even zodat het bestand volledig is weggeschreven
                await Task.Delay(500);

                // Controleer opnieuw of het bestand bestaat
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("Bestand sensorData.txt niet gevonden na wijziging.");
                    return;
                }

                Console.WriteLine("Nieuwe wijzigingen gedetecteerd. Data opnieuw laden...");

                string content = await File.ReadAllTextAsync(_filePath); // Lees de nieuwe inhoud van sensorData.txt

                // SensorParserContext gebruiken met TemperatureSensorParser
                var parserContext = new SensorParserContext();
                parserContext.SetParser(new TemperatureSensorParser());

                var parsedData = parserContext.Parse(content); // Verwerk de data opnieuw

                // Zet om naar JSON en netjes formatteren
                string json = JsonSerializer.Serialize(new { items = parsedData },
                    new JsonSerializerOptions { WriteIndented = true });

                Console.WriteLine($"Nieuwe JSON-data verzonden via SignalR:\n{json}");

                // Verstuur de data naar alle clients via SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveSensorData", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij het verwerken van sensorData.txt: {ex.Message}");
            }
        }
    }
}