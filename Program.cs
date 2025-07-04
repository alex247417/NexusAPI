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
// Bloco para garantir que o banco de dados seja criado ao iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Essencial: Expõe o arquivo swagger.json
    app.MapScalarApiReference(); // Novo: Usa o visualizador Scalar
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapGet("/projects", async (ProjectDbContext db) =>
{
    var projects = await db.Projects.ToListAsync();
    return Results.Ok(projects);
});
app.MapGet("/projects/{id}", async (int id, ProjectDbContext db) =>
{
    var project = await db.Projects.FindAsync(id);

    return project is null ? Results.NotFound() : Results.Ok(project);
});
app.MapPost("/projects", async (Project project, ProjectDbContext db) =>
{
    db.Projects.Add(project);
    await db.SaveChangesAsync();

    return Results.Created($"/projects/{project.Id}", project);
});
app.MapPut("/projects/{id}", async (int id, Project updatedProject, ProjectDbContext db) =>
{
    var project = await db.Projects.FindAsync(id);

    if (project is null)
    {
        return Results.NotFound();
    }

    project.Name = updatedProject.Name;
    project.Description = updatedProject.Description;

    await db.SaveChangesAsync();

    return Results.NoContent();
});
app.MapDelete("/projects/{id}", async (int id, ProjectDbContext db) =>
{
    var project = await db.Projects.FindAsync(id);

    if (project is null)
    {
        return Results.NotFound();
    }

    db.Projects.Remove(project);
    await db.SaveChangesAsync();

    return Results.NoContent();
});
app.Run();

