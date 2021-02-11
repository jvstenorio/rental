using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rental.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rental.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private const string LOGIN_URL = "/users/login";
        private const string POST_USER_URL = "/users/{0}";
        private const string POST_COSTUMER_URL = "/users/customers";
        private const int CUSTOMER_CODE = 1;

        public UsersController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiBaseUrl = configuration.GetSection("API").Value;
        }

        public IActionResult LoginPage(string returnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(returnUrl ?? "/");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPageAsync(UserDto user, string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(returnUrl ?? "/");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    await LoginAsync(user);
                    return Redirect(returnUrl ?? "/");
                }
            }
            catch (ValidationException) 
            {
                
                return RedirectToAction("CreateCustomer");
            }
            catch (UnauthorizedAccessException)
            {
                ViewBag.Error = "Credenciais inválidas";
            }
            catch
            {
                ViewBag.Error = "Ocorreu um erro inesperado, tente novamente mais tarde!";
            }
            return View();
        }

        [Authorize]
        public IActionResult UserPage()
        {
            return View();
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Vehicles");
        }

        private async Task LoginAsync(UserDto user)
        {
            var authResponse = await AuthorizeUserAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.UserData, user.Login),
                new Claim(ClaimTypes.Name, authResponse.Name)
            };

            var userIdentity = new ClaimsIdentity(claims, "Login");
            ClaimsPrincipal claimPrincipal = new ClaimsPrincipal(userIdentity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTime.Now.ToLocalTime().AddMinutes(30),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, authProperties);
        }

        private async Task<UserDto> AuthorizeUserAsync(UserDto userDto)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri($"{_apiBaseUrl}{LOGIN_URL}");
            request.Headers.Add("login", userDto.Login);
            request.Headers.Add("password", userDto.Password);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserDto>(content);
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ValidationException();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        public async Task<IActionResult> CreateCustomerAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CustomerDto customerDto) 
        {
            try
            {
                customerDto.Profile = CUSTOMER_CODE;
                var jsonContent = JsonConvert.SerializeObject(customerDto);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}{POST_COSTUMER_URL}", contentString);

                if (response.IsSuccessStatusCode) 
                {
                    var request = new HttpRequestMessage();
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri($"{_apiBaseUrl}{string.Format(POST_USER_URL, CUSTOMER_CODE)}");
                    request.Headers.Add("login", customerDto.Cpf);
                    request.Headers.Add("password", customerDto.Password);

                    response = await _httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        return View("LoginPage");
                    }
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();
                
                return View();
            }
            catch 
            {
                ViewBag.Error = "Ocorreu um erro inesperado!";
                return View();
            }
        }
    }
}
