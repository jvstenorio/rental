using Microsoft.Extensions.Logging;
using Rental.Domain.Entities;
using Rental.Domain.Repositories;
using System;

namespace Rental.Infrastructure.Repositories
{
    public class CustomersRepository : BaseElasticsearchRepository<Customer>, ICustomersRepository
    {
        public CustomersRepository(IServiceProvider provider, ILogger<BaseElasticsearchRepository<Customer>> logger) : base(provider, logger)
        {
        }
    }
}
