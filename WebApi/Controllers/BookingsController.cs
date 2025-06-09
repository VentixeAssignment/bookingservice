using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.Dto;
using WebApi.Models;
using WebApi.Protos;
using WebApi.Services;

namespace WebApi.Controllers;

[Authorize]
[Route("api/bookings")]
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

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("You have to sign in to book an event.");

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
                CustomerId = userId,
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

            Debug.WriteLine($"Create booking result: {result.Success}, StatusCode: {result.StatusCode}, ErrorMessage: {result.ErrorMessage}");

            if(result.StatusCode == 500)
            {
                return StatusCode(500, new { message = $"Status 500: {result.Success}\n{result.StatusCode}\n{result.ErrorMessage}" });
            }

            return result.Success
                ? Ok(result)
                : BadRequest(new { message = $"Bad request: {result.Success}\n{result.StatusCode}\n{result.ErrorMessage}" });

        }
        catch (RpcException ex)
        {
            return BadRequest(new { message = $"Bad request:{ex.Message}" });
        }
    }
}
