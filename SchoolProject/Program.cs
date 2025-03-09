using SignalRChat.Hubs;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<ChatHub>("/chatHub");




// 1 Creeer SignalR Hub

// 2. Definieer een functie die constant data uitleest van een pi -> (PICO -> Mockdata) -> 
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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}