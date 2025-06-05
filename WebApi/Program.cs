
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Security.Authentication;
using WebApi.Data;
using WebApi.Protos;
using WebApi.Repositories;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);
var grpcUri = builder.Configuration["GrpcUri"];

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

builder.Services.AddGrpcClient<BookingHandler.BookingHandlerClient>(x =>
{
    x.Address = new Uri(grpcUri!);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        SslProtocols = SslProtocols.Tls12
    };
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DataContext>(option =>
    option.UseSqlServer(builder.Configuration["ConnectionStrings:VentixeDb"]));

builder.Services.AddScoped<BookingRepository>();
builder.Services.AddScoped<BookingService>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference("/api/docs");
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
