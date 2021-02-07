using AutoMapper;
using Rental.Domain.Applications;
using Rental.Domain.Entities;
using Rental.Domain.Models;
using Rental.Domain.Repositories;
using Rental.Domain.ValueObjects;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Application
{
    public class UsersApplication : IUsersApplication
    {
        private readonly ICustomersRepository _customersRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public UsersApplication(
            ICustomersRepository customersRepository,
            IEmployeesRepository employeesRepository,
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            _customersRepository = customersRepository;
            _employeesRepository = employeesRepository;
            _usersRepository = usersRepository;
            _mapper = mapper;
        }


        public async Task<UserDto> AuthenticateAsync(string login, string password, CancellationToken cancellationToken)
        {
            var cpf = new Cpf(login);

            if (cpf.IsValid)
            {
                return await AuthorizeCustomerAsync(cpf.Value, password, cancellationToken);
            }
            else
            {
                return await AuthorizeEmployeeAsync(login, password, cancellationToken);
            }
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto, CancellationToken cancellationToken)
        {
            var customer = _mapper.Map<Customer>(customerDto);
            customer.Identifier = Customer.GetIdentifier(customer.Cpf);
            await _customersRepository.AddAsync(customer, cancellationToken);
            return customerDto;
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto, CancellationToken cancellationToken)
        {
            var employee = _mapper.Map<Employee>(employeeDto);
            employee.Identifier = Employee.GetIdentifier(employeeDto.RegistrationNumber);
            await _employeesRepository.AddAsync(employee, cancellationToken);
            return employeeDto;
        }

        public async Task<UserDto> CreateUserAsync(string login, string password, Domain.Enumerations.Profile profile, CancellationToken cancellationToken)
        {
            var user = new User()
            {
                Login = login,
                Password = password,
                Profile = profile,
                Identifier = User.GetIdentifier(login)
            };
            await _usersRepository.AddAsync(user, cancellationToken);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<CustomerDto> GetCustomerAsync(string cpf, CancellationToken cancellationToken)
        {
            var identifier = Customer.GetIdentifier(cpf);
            var customer = await _customersRepository.GetByIdentifierAsync(identifier, cancellationToken);
            if (customer == null)
            {
                return default;
            }
            return _mapper.Map<CustomerDto>(customer);
        }

        private async Task<bool> IsValidPasswordAsync(Guid identifier, string password, CancellationToken cancellationToken)
        {
            var user = await _usersRepository.GetByIdentifierAsync(identifier, cancellationToken);
            return user != null && user.Password.Equals(password);
        }

        private async Task<CustomerDto> AuthorizeCustomerAsync(string login, string password, CancellationToken cancellationToken)
        {
            var customer = await _customersRepository.GetByIdentifierAsync(Customer.GetIdentifier(login), cancellationToken);
            if (customer == null)
            {
                throw new ValidationException("Customer's not found!");
            }
            var isValidPassword = await IsValidPasswordAsync(User.GetIdentifier(login), password, cancellationToken);
            return isValidPassword ? _mapper.Map<CustomerDto>(customer) : default;
        }

        private async Task<EmployeeDto> AuthorizeEmployeeAsync(string login, string password, CancellationToken cancellationToken)
        {
            var employee = await _employeesRepository.GetByIdentifierAsync(Employee.GetIdentifier(login), cancellationToken);
            if (employee == null)
            {
                throw new ValidationException("Employee's not found!");
            }
            var isValidPassword = await IsValidPasswordAsync(User.GetIdentifier(login), password, cancellationToken);
            return isValidPassword ? _mapper.Map<EmployeeDto>(employee) : default;
        }
    }
}
