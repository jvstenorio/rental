using Rental.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Domain.Applications
{
    public interface IUsersApplication
    {
        Task<UserDto> AuthenticateAsync(string login, string password, CancellationToken cancellationToken);

        Task<UserDto> CreateUserAsync(string login, string password, Domain.Enumerations.Profile profile, CancellationToken cancellationToken);

        Task<CustomerDto> CreateCustomerAsync(CustomerDto customer, CancellationToken cancellationToken);

        Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto, CancellationToken cancellationToken);

        Task<CustomerDto> GetCustomerAsync(string cpf, CancellationToken cancellationToken);
    }
}
