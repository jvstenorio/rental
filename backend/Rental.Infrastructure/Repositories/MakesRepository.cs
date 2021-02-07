using Microsoft.Extensions.Logging;
using Rental.Domain.Entities;
using Rental.Domain.Repositories;
using System;

namespace Rental.Infrastructure.Repositories
{
    public class MakesRepository : BaseElasticsearchRepository<Make>, IMakesRepository
    {
        public MakesRepository(
            IServiceProvider provider,
            ILogger<BaseElasticsearchRepository<Make>> logger, 
            IMemoryCacheRepository memoryCacheRepository) : base(provider, logger, memoryCacheRepository)
        {
        }
    }
}
