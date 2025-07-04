using Microsoft.EntityFrameworkCore;
using NexusAPI;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProjectDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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
app.Run();

