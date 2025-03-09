// Importeren van sensordata.txt

// Maak een singleton
// Parsen van sensordata.txt naar key->value -> Strategy Pattern
// maak er een array met een object met alle properties per datapunt


public sealed class Singleton
{
    private static Singleton instance = null;
    private static readonly object padlock = new object();
    
    
    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sensorData.txt");
    
    Singleton()
    {
    if (File.Exists(path))
    {
        string content = File.ReadAllText(path);
        Console.WriteLine(content);
        
        
        
    }
    }

    public static Singleton Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }
    }
}