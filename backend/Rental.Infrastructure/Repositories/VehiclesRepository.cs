using Microsoft.Extensions.Logging;
using Rental.Domain.Entities;
using Rental.Domain.Repositories;
using System;

namespace Rental.Infrastructure.Repositories
{
    public class VehiclesRepository : BaseElasticsearchRepository<Vehicle>, IVehiclesRepository
    {
        public VehiclesRepository(
            IServiceProvider provider,
            ILogger<BaseElasticsearchRepository<Vehicle>> logger,
            IMemoryCacheRepository memoryCacheRepository) : base(provider, logger, memoryCacheRepository)
        {
        }
    }
}
