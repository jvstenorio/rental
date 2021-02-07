using Microsoft.Extensions.Logging;
using Nest;
using Rental.Domain.Entities;
using Rental.Domain.Repositories;
using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Rental.Domain.Enumerations;
using System.Linq;

namespace Rental.Infrastructure.Repositories
{
    public class BookingsRepository : BaseElasticsearchRepository<Booking>, IBookingsRepository
    {
        private readonly ElasticClient _client;

        public BookingsRepository(IServiceProvider provider, ILogger<BaseElasticsearchRepository<Booking>> logger, IMemoryCacheRepository memoryCacheRepository) : base(provider, logger, memoryCacheRepository)
        {
            _client = provider.GetService<ElasticClient>();
        }

        public async Task<bool> VehicleHasOpenedBookingAsync(string plate, CancellationToken cancellationToken)
        {
            var searchResponse = await _client.SearchAsync<object>(
                search => search
                .Index(_index)
                .Size(1)
                .Query(q =>
                        q.Match(m => m.Field("plate.keyword").Query(plate))
                        &&
                        q.Match(m => m.Field("status.keyword").Query(BookingStatus.OPEN.ToString()))
                       ),
                cancellationToken);

            return searchResponse.Documents?.Any() ?? false;
        }
    }
}
