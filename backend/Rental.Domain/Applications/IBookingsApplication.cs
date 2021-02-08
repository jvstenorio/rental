using Rental.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Domain.Applications
{
    public interface IBookingsApplication
    {
        Task<BookingDto> GetQuotationAsync(string plate, int totalHours, CancellationToken cancellationToken);

        Task<BookingDto> CreateBookingAsync(BookingDto bookingDto, CancellationToken cancellationToken);

        Task<BookingDto> GetBookingAfterChecklistAsync(VehicleChecklistDto checklist, string bookingCode, CancellationToken cancellationToken);

        Task<byte[]> GetContractFromBookingAsync(string bookingCode, CancellationToken cancellationToken);

        Task<List<BookingDto>> GetBookingsByCpfAsync(string cpf, CancellationToken cancellationToken);
    }
}
