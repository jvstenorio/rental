using Microsoft.Extensions.Logging;
using Rental.Domain.Entities;
using Rental.Domain.Repositories;
using System;

namespace Rental.Infrastructure.Repositories
{
    public class UsersRepository : BaseElasticsearchRepository<User>, IUsersRepository
    {
        public UsersRepository(
            IServiceProvider provider,
            ILogger<BaseElasticsearchRepository<User>> logger,
            IMemoryCacheRepository memoryCacheRepository) : base(provider, logger, memoryCacheRepository)
        {
        }
    }
}
