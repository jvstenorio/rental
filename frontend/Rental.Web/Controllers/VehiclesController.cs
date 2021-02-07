using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rental.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rental.Web.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly ILogger<VehiclesController> _logger;
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;
        private const string VEHICLES_URL = "/vehicles";

        public VehiclesController(
            ILogger<VehiclesController> logger,
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _apiBaseUrl = configuration.GetSection("API").Value;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new VehiclesViewModel();

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}{VEHICLES_URL}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var vehicles = JsonConvert.DeserializeObject<List<VehicleDto>>(content);

                var query = vehicles.GroupBy(
                                    v => v.Category,
                                    v => v,
                                    (key, groups) => new
                                    {
                                        Key = key,
                                        Values = groups
                                    });

                foreach (var result in query)
                {
                    var category = new CategoryDto();
                    category.Name = result.Key;
                    category.Vehicles = result.Values.ToList();
                    viewModel.Categories.Add(category);
                }
            }
            else
            {
                return RedirectToAction("Error");
            }
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
