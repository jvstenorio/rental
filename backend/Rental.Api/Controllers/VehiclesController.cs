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
    public class VehiclesController : ControllerBase
    {
        private readonly IVehiclesApplication _vehiclesApplication;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _timeout;

        public VehiclesController(
            IVehiclesApplication vehiclesApplication,
            IConfiguration configuration )
        {
            _vehiclesApplication = vehiclesApplication;
            _configuration = configuration;
            _timeout = TimeSpan.FromSeconds(configuration.GetTimeoutInSec());
        }
        /// <summary>
        /// Create a vehicle
        /// </summary>
        /// <param name="vehicleDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostVehicleAsync([FromBody, Required] VehicleDto vehicleDto)
        {
            using var cts = new CancellationTokenSource(_timeout);
            var vehicle = await _vehiclesApplication.CreateVehicleAsync(vehicleDto, cts.Token);
            return Created(string.Empty, vehicle);
        }

        /// <summary>
        /// Create a make
        /// </summary>
        /// <param name="makeDto"></param>
        /// <returns></returns>
        [HttpPost("make")]
        [ProducesResponseType(typeof(MakeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostMakeAsync([FromBody, Required] MakeDto makeDto)
        {
            using var cts = new CancellationTokenSource(_timeout);
            var make = await _vehiclesApplication.CreateMakeAsync(makeDto, cts.Token);
            return Created(string.Empty, make);
        }

        /// <summary>
        /// Create a model
        /// </summary>
        /// <param name="modelDto"></param>
        /// <returns></returns>
        [HttpPost("model")]
        [ProducesResponseType(typeof(ModelDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostModelAsync([FromBody, Required] ModelDto modelDto)
        {
            using var cts = new CancellationTokenSource(_timeout);
            var model = await _vehiclesApplication.CreateModelAsync(modelDto, cts.Token);
            return Created(string.Empty, model);
        }

    }
}
