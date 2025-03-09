using SignalRChat.Hubs;
using Schoolproject.Sensordata; 

var builder = WebApplication.CreateBuilder(args);

// Roep de Singleton aan om sensorData.txt in te laden
var instance = SensorDataSingleton.Instance; 

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<ChatHub>("/chatHub");

// 1. Creëer SignalR Hub

// 2. Definieer een functie die constant data uitleest van een Pi -> (PICO -> Mockdata) -> 
// 3. Op het moment dat je dat ontvangt, wil je dit beschikbaar stellen voor je client -> (Signal R)
// 4. Client moet nieuwe data kunnen ontvangen wanneer er nieuwe data beschikbaar is -> (Signal R)

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Verwijder of commentarieer de regel uit die de HTTPS-redirect uitvoert
// app.UseHttpsRedirection(); 

// Stel de URL in voor alleen HTTP
app.Urls.Add("http://localhost:5000");

app.Run();
