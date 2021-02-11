using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rental.Web.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rental.Web.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private const string QUOTATIONS_URL = "/bookings/vehicles/{0}/quotations/{1}/";
        private const string POST_BOOKING_URL = "/bookings";
        private const string GET_BOOKINGS_BY_CPF = "/bookings/customers/{0}";
        private const string MOCK_CPF = "13915337390";

        public BookingsController(
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration.GetSection("API").Value;
        }
        public async Task<ActionResult> IndexAsync() 
        {
  
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}{string.Format(GET_BOOKINGS_BY_CPF, User.FindFirst(ClaimTypes.UserData).Value)}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var bookingResponse = JsonConvert.DeserializeObject<List<BookingDto>>(content);
                return View("Bookings", bookingResponse);
            }
            else 
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public ActionResult Quotation(string plate)
        {
            var booking = new BookingDto()
            {
                Plate = plate,
                Cpf = User.FindFirst(ClaimTypes.UserData).Value
            };
            return View("BookingForm", booking);
        }

        // GET: BookingsController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(BookingDto booking)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(booking);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}{POST_BOOKING_URL}", contentString);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var bookingUpdate = JsonConvert.DeserializeObject<BookingDto>(responseContent);
                    return View("BookingConfirmed", bookingUpdate);
                }
                else
                {
                    ViewBag.Error = responseContent;
                    return View("BookingForm", booking);
                }
                
            }
            catch
            {
                ViewBag.Error = "Ocorreu um erro inesperado, por favor tente mais tarde!";
                return View("BookingForm", booking);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateAsync(BookingDto booking)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}{string.Format(QUOTATIONS_URL, booking.Plate, booking.TotalHours)}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var bookingResponse = JsonConvert.DeserializeObject<BookingDto>(content);
                booking.Price = bookingResponse.Price;
            }
            return PartialView("_partialBooking", booking);
        }

        public ActionResult BookingConfirmed(BookingDto bookingDto) 
        {
            return View(bookingDto);
        }
    }
}
