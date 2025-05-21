using WebApi.Dto;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Factories;

public static class BookingFactory
{
    public static BookingEntity EntityFromDto(BookingRegForm dto)
    {
        // Total price need to be fetched and calculated from EventEntity in EventService.
        return new BookingEntity
        {
            Seats = dto.Seats,
            EventId = dto.EventId,
            TotalPrice = 1,
            CustomerId = dto.CustomerId ?? "",
            CustomerFirstName = dto.CustomerFirstName ?? "",
            CustomerLastName = dto.CustomerLastName ?? "",
            CustomerEmail = dto.CustomerEmail ?? "",
            CustomerPhone = dto.CustomerPhone ?? "",
            CustomerStreetAddress = dto.CustomerStreetAddress ?? "",
            CustomerPostalCode = dto.CustomerPostalCode ?? "",
            CustomerCity = dto.CustomerCity ?? "",
            CustomerBirthDate = dto.CustomerBirthDate
        };
    }

    public static BookingModel ModelFromEntity(BookingEntity entity)
    {
        return new BookingModel
        {
            Id = entity.Id,
            Seats = entity.Seats,
            CreatedAt = entity.CreatedAt,
            EventId = entity.EventId,
            TotalPrice = entity.TotalPrice,
            CustomerId = entity.CustomerId ?? "",
            CustomerFirstName = entity.CustomerFirstName ?? "",
            CustomerLastName = entity.CustomerLastName ?? "",
            CustomerEmail = entity.CustomerEmail ?? "",
            CustomerPhone = entity.CustomerPhone ?? "",
            CustomerStreetAddress = entity.CustomerStreetAddress ?? "",
            CustomerPostalCode = entity.CustomerPostalCode ?? "",
            CustomerCity = entity.CustomerCity ?? "",
            CustomerBirthDate = entity.CustomerBirthDate
        };
    }

    public static BookingEntity EntityFromModel(BookingModel model)
    {
        return new BookingEntity
        {
            Id = model.Id,
            Seats = model.Seats,
            CreatedAt = model.CreatedAt,
            EventId = model.EventId,
            TotalPrice = model.TotalPrice,
            CustomerId = model.CustomerId ?? "",
            CustomerFirstName = model.CustomerFirstName ?? "",
            CustomerLastName = model.CustomerLastName ?? "",
            CustomerEmail = model.CustomerEmail ?? "",
            CustomerPhone = model.CustomerPhone ?? "",
            CustomerStreetAddress = model.CustomerStreetAddress ?? "",
            CustomerPostalCode = model.CustomerPostalCode ?? "",
            CustomerCity = model.CustomerCity ?? "",
            CustomerBirthDate = model.CustomerBirthDate
        };
    }
}
