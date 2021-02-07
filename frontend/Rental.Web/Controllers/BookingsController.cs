using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rental.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rental.Web.Controllers
{
    public class BookingsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private const string QUOTATIONS_URL = "/Bookings/vehicles/{0}/quotations/{1}/";

        public BookingsController(
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration.GetSection("API").Value;
        }

        public ActionResult Index() 
        {
            return View("Bookings");
        }

        // GET: BookingsController/Details/5
        public ActionResult Quotation(string plate)
        {
            var booking = new BookingDto()
            {
                Plate = plate
            };
            return View("BookingForm", booking);
        }

        // GET: BookingsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BookingsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookingDto booking)
        {
            try
            {
                return RedirectToAction(nameof(Index));
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
