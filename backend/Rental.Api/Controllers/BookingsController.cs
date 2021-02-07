using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Rental.Domain.Applications;
using Rental.Domain.Errors;
using Rental.Domain.Extensions;
using Rental.Domain.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly TimeSpan _timeout;
        private readonly IBookingsApplication _bookingsApplication;

        public BookingsController(IConfiguration configuration, IBookingsApplication bookingsApplication)
        {
            _timeout = TimeSpan.FromSeconds(configuration.GetTimeoutInSec());
            _bookingsApplication = bookingsApplication;
        }
        /// <summary>
        /// Get a quotation
        /// </summary>
        /// <param name="plate"></param>
        /// <param name="totalHours"></param>
        /// <returns></returns>
        [HttpGet("vehicles/{plate}/quotations/{totalHours}")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQuotationAsync(
            [FromRoute, Required] string plate,
            [FromRoute, Required] int totalHours)
        {
            using var cts = new CancellationTokenSource(_timeout);
            var booking = await _bookingsApplication.GetQuotationAsync(plate, totalHours, cts.Token);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }
        /// <summary>
        /// Create a booking
        /// </summary>
        /// <param name="bookingDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostBookingAsync([FromBody, Required] BookingDto bookingDto)
        {
            using var cts = new CancellationTokenSource(_timeout);
            var booking = await _bookingsApplication.CreateBookingAsync(bookingDto, cts.Token);
            return Created(string.Empty, booking);
        }

        /// <summary>
        /// Get booking after checklist
        /// </summary>
        /// <param name="bookingCode"></param>
        /// <param name="vehicleChecklistDto"></param>
        /// <returns></returns>
        [HttpPost("{bookingCode}/checklist")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookingAfterChecklistAsync(
            [FromRoute, Required] string bookingCode,
            [FromBody, Required] VehicleChecklistDto vehicleChecklistDto)
        {
            using var cts = new CancellationTokenSource(_timeout);
            var booking = await _bookingsApplication.GetBookingAfterChecklistAsync(vehicleChecklistDto, bookingCode, cts.Token);
            return Ok(booking);
        }
    }
}
