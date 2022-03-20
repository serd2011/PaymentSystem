using API.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureHeaders();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseForwardedHeaders();

app.UseAuthorization();

app.MapControllers();

app.Run();
