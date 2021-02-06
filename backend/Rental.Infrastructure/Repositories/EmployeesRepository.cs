using Microsoft.Extensions.Logging;
using Rental.Domain.Entities;
using Rental.Domain.Extensions;
using Rental.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Infrastructure.Repositories
{
    public class EmployeesRepository : BaseElasticsearchRepository<Employee>, IEmployeesRepository
    {
        public EmployeesRepository(
            IServiceProvider provider, 
            ILogger<BaseElasticsearchRepository<Employee>> logger) : base(provider, logger)
        {
        }

        public async Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            await AddAsync(employee, cancellationToken);
        }

        public Task<Employee> GetEmployeeFromRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken)
        {
            return GetByIdentifierAsync(new object[] { registrationNumber}.GetIdentifier(), cancellationToken);
        }

        public async Task<List<Employee>> ListEmployeesAsync(CancellationToken cancellationToken)
        {
            return await ListAllAsync(cancellationToken);
        }
    }
}
