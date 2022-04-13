using API.Configurations;
using API.Middleware.Filters;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ModelAttributesValidationFilter>();
}).AddNewtonsoftJson();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.ConfigureConnection(builder.Configuration);
builder.Services.ConfigureVersioning();
builder.Services.ConfigureHeaders();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseForwardedHeaders();

app.UseAuthorization();

app.MapControllers();

app.Run();
