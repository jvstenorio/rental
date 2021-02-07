using Microsoft.Extensions.Logging;
using Rental.Domain.Entities;
using Rental.Domain.Repositories;
using System;

namespace Rental.Infrastructure.Repositories
{
    public class ModelsRepository : BaseElasticsearchRepository<Model>, IModelsRepository
    {
        public ModelsRepository(
            IServiceProvider provider, 
            ILogger<BaseElasticsearchRepository<Model>> logger,
            IMemoryCacheRepository memoryCacheRepository) : base(provider, logger, memoryCacheRepository)
        {
        }
    }
}
