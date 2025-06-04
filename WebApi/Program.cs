
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WebApi.Data;
using WebApi.Protos;
using WebApi.Repositories;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClient<BookingHandler.BookingHandlerClient>(x =>
{
    x.Address = new Uri("https://localhost:7157");
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
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
