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
public class BookingsController(BookingHandler.BookingHandlerClient bookingHandlerClient, BookingService service) : ControllerBase
{
    private readonly BookingHandler.BookingHandlerClient _bookingHandlerClient = bookingHandlerClient;
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
            var reply = await _bookingHandlerClient.GetEventInformationAsync(request);

            if (!reply.Success)
            {
                return BadRequest("Failed to get event information from eventservice.");

            }

            var bookingDto = new BookingRegForm
            {
                Seats = form.Seats,
                EventId = reply.Event.Id,
                TermsAndConditions = true,
                CustomerFirstName = form.CustomerFirstName,
                CustomerLastName = form.CustomerLastName,
                CustomerEmail = form.CustomerEmail,
                CustomerPhone = form.CustomerPhone,
                CustomerBirthDate = form.CustomerBirthDate,
                CustomerStreetAddress = form.CustomerStreetAddress,
                CustomerPostalCode = form.CustomerPostalCode,
                CustomerCity = form.CustomerCity,
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
