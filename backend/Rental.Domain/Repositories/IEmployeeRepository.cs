using Rental.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Domain.Repositories
{
    public interface IEmployeesRepository
    {
        Task<Employee> GetEmployeeFromRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken);
        Task<List<Employee>> ListEmployeesAsync(CancellationToken cancellationToken);
        Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken);
    }
}
