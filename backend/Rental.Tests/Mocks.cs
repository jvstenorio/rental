using AutoMapper;
using Moq;
using Rental.Domain.Applications;
using Rental.Domain.Entities;
using Rental.Domain.Mappers;
using Rental.Domain.Models;
using Rental.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Tests
{
    public static class Mocks
    {
        public static Mock<IVehiclesApplication> GetVehiclesMock(string plate, double pricePerHour)
        {
            var mock = new Mock<IVehiclesApplication>();
            mock
                .Setup(m => m.GetVehicleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(default(VehicleDto)));
            mock
                .Setup(m => m.GetVehicleAsync(It.Is<string>(p => p.Equals(plate)), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(DefaultVehicle(plate, pricePerHour)));
            return mock;
        }
        public static Mock<IUsersApplication> GetCustomersMock(string cpf)
        {
            var mock = new Mock<IUsersApplication>();
            mock
                .Setup(m => m.GetCustomerAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(default(CustomerDto)));
            mock
                .Setup(m => m.GetCustomerAsync(It.Is<string>(c => c.Equals(cpf)), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(DefaultCustomer(cpf)));
            return mock;
        }
        public static Mock<IBookingsRepository> GetBookingsMock(string bookingCode, string cpf, string plate, double price, int totalHours)
        {
            var mock = new Mock<IBookingsRepository>();
            mock
                .Setup(m => m.GetByIdentifierAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(default(Booking)));
            mock
                .Setup(m => m.GetByIdentifierAsync(It.Is<Guid>(i => i.Equals(Booking.GetIdentifier(bookingCode))), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(DefaultBooking(bookingCode, cpf, plate, price, totalHours)));

            mock.Setup(m => m.AddAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            mock
                .Setup(m => m.VehicleHasOpenedBookingAsync(It.Is<string>(p => !p.Equals(p)), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            mock
                .Setup(m => m.VehicleHasOpenedBookingAsync(It.Is<string>(p => p.Equals(p)), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            return mock;
        }

        public static IMapper GetAutoMapperConfigure()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile());
            });

            return mapperConfig.CreateMapper();
        }
        public static VehicleDto DefaultVehicle(string plate, double pricePerHour) =>
            new VehicleDto
            {
                Plate = plate,
                PricePerHour = pricePerHour,
                Category = "LUXO",
                Year = 1993,
                Make = "Volskwagen",
                Fuel = "Gasolina",
                Model = "Fusca",
            };
        public static CustomerDto DefaultCustomer(string cpf) =>
            new CustomerDto
            {
                Cpf = cpf,
                Name = "João"
            };
        public static Booking DefaultBooking(string bookingCode, string cpf, string plate, double price, int totalHours) =>
            new Booking
            {
                BookingCode = bookingCode,
                Cpf = cpf,
                Plate = plate,
                Price = price,
                Date = DateTimeOffset.Now,
                TotalHours = totalHours
            };
    }
}
