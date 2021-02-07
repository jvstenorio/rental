using Microsoft.Extensions.Logging;
using Rental.Domain.Entities;
using Rental.Domain.Repositories;
using System;

namespace Rental.Infrastructure.Repositories
{
    public class EmployeesRepository : BaseElasticsearchRepository<Employee>, IEmployeesRepository
    {
        public EmployeesRepository(
            IServiceProvider provider, 
            ILogger<BaseElasticsearchRepository<Employee>> logger, 
            IMemoryCacheRepository memoryCacheRepository) : base(provider, logger, memoryCacheRepository)
        {
        }
    }
}
