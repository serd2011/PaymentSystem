using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using API.Configurations;
using API.Middleware.Filters;

var builder = WebApplication.CreateBuilder(args);

// Reading messages configuration file
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("messages.json", optional: false, reloadOnChange: true); ;
});

// Configurations
builder.Services.ConfigureConnection(builder.Configuration);
builder.Services.ConfigureVersioning();
builder.Services.ConfigureHeaders();

// JWT Tokens Authorisation setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = false;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin", "true"));
    options.AddPolicy("HasUserId", policy => policy.RequireClaim("id"));
});

// Controllers setup
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ModelAttributesValidationFilter>();
    options.Filters.Add(new AuthorizeFilter("HasUserId"));
}).AddNewtonsoftJson();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
