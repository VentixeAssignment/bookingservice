using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Models;
using WebApi.Protos;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("api")]
[ApiController]
public class BookingsController(BookingService service) : ControllerBase
{
    private readonly BookingService _service = service;

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookingRegForm form)
    {
        if (!ModelState.IsValid)
            return BadRequest("Request is incomplete.");

        try
        {
            var request = new EventInformationRequest
            {
                Id = form.EventId
            };

            var bookingDto = new BookingRegForm
            {
                Seats = form.Seats,
                TotalPrice = form.TotalPrice,
                EventId = form.EventId ?? "",
                TermsAndConditions = form.TermsAndConditions,
                CustomerFirstName = form.CustomerFirstName,
                CustomerLastName = form.CustomerLastName,
                CustomerEmail = form.CustomerEmail,
                CustomerPhone = form.CustomerPhone ?? "",
                CustomerBirthDate = form.CustomerBirthDate ?? null,
                CustomerStreetAddress = form.CustomerStreetAddress ?? "",
                CustomerPostalCode = form.CustomerPostalCode ??  "",
                CustomerCity = form.CustomerCity ?? "",
            };
            var result = await _service.CreateAsync(bookingDto);

            return result.Success
                ? Ok(result)
                : BadRequest($"{result.Success}\n{result.StatusCode}\n{result.ErrorMessage}");

        }
        catch (RpcException ex)
        {
            return BadRequest(ex.Status.Detail);
        }
    }
}
