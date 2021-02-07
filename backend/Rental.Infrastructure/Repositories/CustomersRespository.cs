using Microsoft.Extensions.Logging;
using Rental.Domain.Entities;
using Rental.Domain.Repositories;
using System;

namespace Rental.Infrastructure.Repositories
{
    public class CustomersRespository : BaseElasticsearchRepository<Customer>, ICustomersRepository
    {
        public CustomersRespository(IServiceProvider provider, ILogger<BaseElasticsearchRepository<Customer>> logger) : base(provider, logger)
        {
        }
    }
}
