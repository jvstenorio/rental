using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rental.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rental.Web.Controllers
{
    public class BookingsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private const string QUOTATIONS_URL = "/Bookings/vehicles/{0}/quotations/{1}/";
        private const string POST_BOOKING_URL = "/Bookings";
        private const string GET_BOOKINGS_BY_CPF = "/Bookings/customers/{0}";
        private const string MOCK_CPF = "13915337390";

        public BookingsController(
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration.GetSection("API").Value;
        }

        public async Task<ActionResult> Index() 
        {
  
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}{string.Format(GET_BOOKINGS_BY_CPF, MOCK_CPF)}");
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
                Cpf = MOCK_CPF
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
        public async Task<ActionResult> Create(BookingDto booking)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(booking);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}{POST_BOOKING_URL}", contentString);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Bookings");
                }
                else 
                {
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }
                
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(BookingDto booking)
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
    }
}
