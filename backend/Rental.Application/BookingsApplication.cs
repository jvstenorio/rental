using Rental.Domain.Applications;
using Rental.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Application
{
    public class BookingsApplication : IBookingsApplication
    {
        private readonly IVehiclesApplication _vehiclesApplication;

        public BookingsApplication(IVehiclesApplication vehiclesApplication)
        {
            _vehiclesApplication = vehiclesApplication;
        }

        public async Task<BookingDto> GetQuotationAsync(string plate, int totalHours, CancellationToken cancellationToken)
        {
            var vehicle = await _vehiclesApplication.GetVehicleAsync(plate, cancellationToken);
            if (vehicle == null || vehicle.PricePerHour == null) 
            {
                return default;
            }
            else 
            {
                return new BookingDto
                {
                    Vehicle = vehicle,
                    TotalHours = totalHours,
                    Price = totalHours * vehicle.PricePerHour
                };
            }
        }
    }
}
