using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Essencial: Expõe o arquivo swagger.json
    app.MapScalarApiReference(); // Novo: Usa o visualizador Scalar
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var projects = new List<Project>
{
    new(1, "NexusOS", "Sistema operacional quântico para a próxima geração."),
    new(2, "ChronoLeap", "App de viagem no tempo com baixo consumo de bateria."),
    new(3, "BioSynth API", "API para simulação de ecossistemas alienígenas.")
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
app.MapGet("/projects", () =>
{
    return projects;
});

app.MapGet("/projects/{id}", (int id) =>
{
    var project = projects.FirstOrDefault(p => p.Id == id);

    if (project is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(project);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
record Project(int Id, string Name, string Description);