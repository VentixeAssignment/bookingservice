
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Net;
using System.Security.Authentication;
using System.Text;
using WebApi.Data;
using WebApi.Protos;
using WebApi.Repositories;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.Information);

var allowedOrigins = builder.Configuration["AllowedOrigins"];
var originArray = allowedOrigins?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

if (originArray == null || originArray.Length == 0)
{
    throw new Exception($"Appsettings not loaded correctly. {allowedOrigins}");
}


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(originArray)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DataContext>(option =>
    option.UseSqlServer(builder.Configuration["ConnectionStrings:VentixeDb"]));

builder.Services.AddScoped<GrpcService>();
builder.Services.AddScoped<BookingRepository>();
builder.Services.AddScoped<BookingService>();


var port = Environment.GetEnvironmentVariable("PORT");

builder.WebHost.ConfigureKestrel(options =>
{
    if (port is not null)
    {
        options.ListenAnyIP(int.Parse(port));
    }
    else
    {
        options.ListenAnyIP(5022); 
        options.ListenAnyIP(7084, listenOptions => listenOptions.UseHttps()); 
    }
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateIssuer = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateAudience = true,
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true
    };
    x.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference("/api/docs");

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
