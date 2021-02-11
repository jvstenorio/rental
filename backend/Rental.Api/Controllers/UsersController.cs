using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Rental.Domain.Applications;
using Rental.Domain.Enumerations;
using Rental.Domain.Errors;
using Rental.Domain.Extensions;
using Rental.Domain.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersApplication _usersApplication;
        private readonly TimeSpan _timeout;
        public UsersController(IConfiguration configuration, IUsersApplication usersApplication)
        {
            _usersApplication = usersApplication;
            _timeout = TimeSpan.FromSeconds(configuration.GetTimeoutInSec());
        }
        /// <summary>
        /// Authenticates an user.
        /// </summary>
        /// <param name="login">CPF or registration number</param>
        /// <param name="password">User's password</param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync(
            [FromHeader, Required] string login, 
            [FromHeader, Required] string password) 
        {
            using var cts = new CancellationTokenSource(_timeout);
            var authentication = await _usersApplication.AuthenticateAsync(login, password, cts.Token);
            if (authentication == null)
            {
                return Unauthorized();
            }
            else 
            {
                return Ok(authentication);
            }
        }

        /// <summary>
        /// Create an user
        /// </summary>
        /// <param name="login">CPF or registration number</param>
        /// <param name="password">User's password</param>
        /// <param name="profile">Profile type (CUSTOMER = 1 OR EMPLOYEE = 0)</param>
        /// <returns></returns>
        [HttpPost("{profile}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostUserAsync(
            [FromHeader, Required] string login,
            [FromHeader, Required] string password,
            [FromRoute, Required] Profile profile)
        {
            using var cts = new CancellationTokenSource(_timeout);
            var user = await _usersApplication.CreateUserAsync(login, password, profile, cts.Token);
            return Created(string.Empty, user);
        }

        /// <summary>
        /// Create a customer
        /// </summary>
        /// <param name="customerDto"></param>
        /// <returns></returns>
        [HttpPost("customers")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostCustomersAsync([FromBody, Required] CustomerDto customerDto)
        {
            using var cts = new CancellationTokenSource(_timeout);
            var customer = await _usersApplication.CreateCustomerAsync(customerDto, cts.Token);
            return Created(string.Empty, customer);
        }
        /// <summary>
        /// Create a employee
        /// </summary>
        /// <param name="employeeDto"></param>
        /// <returns></returns>
        [HttpPost("employee")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostEmployeeAsync([FromBody, Required] EmployeeDto employeeDto)
        {
            using var cts = new CancellationTokenSource(_timeout);
            var employee = await _usersApplication.CreateEmployeeAsync(employeeDto, cts.Token);
            return Created(string.Empty, employee);
        }
    }
}
