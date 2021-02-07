﻿using Rental.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Domain.Applications
{
    public interface IBookingsApplication
    {
        Task<BookingDto> GetQuotationAsync(string plate, int totalHours, CancellationToken cancellationToken);
    }
}