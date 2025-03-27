using Schoolproject.Sensordata;
using Schoolproject.Services;

var builder = WebApplication.CreateBuilder(args);

// Registreer services
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true; // Schakel pretty-print JSON in
});
builder.Services.AddSingleton<SensorDataSingleton>(provider => SensorDataSingletonFactory.Create());
builder.Services.AddSignalR(); // Voeg SignalR toe
builder.Services.AddSingleton<FileWatcherService>(); // Voeg FileWatcherService toe

// CORS-configuratie
var corsPolicy = "_allowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Sta Vue-app toe
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

// Middleware configuratie
app.UseRouting();       
app.UseCors(corsPolicy); 
app.UseAuthorization();

// Voeg controllers toe!
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();  //  Belangrijk voor API-routes!
    endpoints.MapHub<SensorHub>("/sensorHub").RequireCors(corsPolicy);
});

// Start FileWatcherService
var fileWatcher = app.Services.GetRequiredService<FileWatcherService>();
fileWatcher.StartWatching();

app.Urls.Add("http://localhost:5000"); // Stel de URL in voor de backend
app.Run();