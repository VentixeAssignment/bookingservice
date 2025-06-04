
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WebApi.Data;
using WebApi.Protos;
using WebApi.Repositories;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
var azureUri = builder.Configuration["GrpcUri:AzureUri"];
var localUri = builder.Configuration["GrpcUri:LocalUri"];

builder.Services.AddGrpcClient<BookingHandler.BookingHandlerClient>(x =>
{
    x.Address = new Uri(azureUri!);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DataContext>(option =>
    option.UseSqlServer(builder.Configuration["ConnectionStrings:LocalDb"]));

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
