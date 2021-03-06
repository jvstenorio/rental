﻿using Rental.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Domain.Repositories
{
    public interface IBookingsRepository : IBaseRepository<Booking>
    {
        Task<bool> VehicleHasOpenedBookingAsync(string plate, CancellationToken cancellationToken);
        Task<List<Booking>> GetBookingsByCpfAsync(string cpf, CancellationToken cancellationToken);
    }
}
