using Rental.Application;
using Rental.Domain.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace Rental.Tests
{
    public class BookingsApplicationTest
    {
        [Theory]
        [InlineData("PQS9574", 100.0, 10)]
        [InlineData("ABC9774", 200.0, 148)]
        [InlineData("LCC9584", 10.0, 500)]
        [InlineData("LLA3574", 551.0, 93)]
        [InlineData("UBC9774", 23000.0, 24)]
        public async Task Get_Quotation_Success(string plate, double pricePerHour, int totalHours)
        {
            var bookingApplication = new BookingsApplication(Mocks.GetVehiclesMock(plate, pricePerHour).Object, null, null, Mocks.GetAutoMapperConfigure());

            var quotation = await bookingApplication.GetQuotationAsync(plate, totalHours, default);

            Assert.True(quotation.Price == (pricePerHour * totalHours));
        }

        [Theory]
        [InlineData("PQS9574", 100.0, 10)]
        [InlineData("ABC9774", 200.0, 148)]
        [InlineData("LCC9584", 10.0, 500)]
        [InlineData("LLA3574", 551.0, 93)]
        [InlineData("UBC9774", 23000.0, 24)]
        public async Task Get_Quotation_Vehicle_Not_Found(string plate, double pricePerHour, int totalHours)
        {
            var bookingApplication = new BookingsApplication(Mocks.GetVehiclesMock("DEFAULT", pricePerHour).Object, null, null, Mocks.GetAutoMapperConfigure());

            var quotation = await bookingApplication.GetQuotationAsync(plate, totalHours, default);

            Assert.Null(quotation);
        }

        [Theory]
        [InlineData("LQLHQ23X", "79908899007", "PQS9574", 200, 10)]
        [InlineData("W726232J", "79908899007", "ABC9774", 78, 148)]
        [InlineData("96EIY5BW", "79908899007", "ABC9774", 87, 7)]
        [InlineData("IJ89T7UL", "79908899007", "ABC9774", 93, 8)]
        [InlineData("LDL8ULP0", "79908899007", "ABC9774", 24, 1)]
        public async Task Create_Booking_Success(string bookingCode, string cpf, string plate, double pricePerHour, int totalHours)
        {
            var bookingsRepository = Mocks.GetBookingsMock(bookingCode, cpf, plate, pricePerHour, totalHours);
            var bookingApplication = new BookingsApplication(
                Mocks.GetVehiclesMock(plate, pricePerHour).Object,
                Mocks.GetCustomersMock(cpf).Object,
                bookingsRepository.Object,
                Mocks.GetAutoMapperConfigure());

            var bookingInput = new BookingDto { Cpf = cpf, Plate = plate, TotalHours = totalHours };
            var quotation = await bookingApplication.GetQuotationAsync(plate, totalHours, default);
            var booking = await bookingApplication.CreateBookingAsync(bookingInput, default);

            Assert.Equal(quotation.Price, booking.Price);
        }

        [Theory]
        [InlineData("LQLHQ23X", "79908899007", "PQS9574", 200, 10)]
        [InlineData("W726232J", "79908899007", "ABC9774", 78, 148)]
        [InlineData("96EIY5BW", "79908899007", "ABC9774", 87, 7)]
        [InlineData("IJ89T7UL", "79908899007", "ABC9774", 93, 8)]
        [InlineData("LDL8ULP0", "79908899007", "ABC9774", 24, 1)]
        public async Task Create_Booking_Vehicle_Has_Opened_Booking(string bookingCode, string cpf, string plate, double pricePerHour, int totalHours)
        {
            var bookingsRepository = Mocks.GetBookingsMock(bookingCode, cpf, plate, pricePerHour, totalHours);
            var bookingApplication = new BookingsApplication(
                Mocks.GetVehiclesMock(plate, pricePerHour).Object,
                Mocks.GetCustomersMock(cpf).Object,
                bookingsRepository.Object,
                Mocks.GetAutoMapperConfigure());

            var bookingInput = new BookingDto { Cpf = cpf, Plate = "DEFAULT", TotalHours = totalHours };

            await Assert.ThrowsAsync<ValidationException>(async () => await bookingApplication.CreateBookingAsync(bookingInput, default));
        }

        [Theory]
        [InlineData("LQLHQ23X", 1254.5, true, true, false, false, 0)]
        [InlineData("W726232J", 3659.2, false, true, false, false, 1)]
        [InlineData("96EIY5BW", 547.2, false, false, false, false, 2)]
        [InlineData("IJ89T7UL", 58.1, false, false, true, false, 3)]
        [InlineData("LDL8ULP0", 8658.9, false, false, true, true, 4)]
        public async Task Booking_PriceAfter_Checklist(string bookingCode, double price, bool fullFuelTank, bool clean, bool smashed, bool scratches, int countIncidents)
        {
            var bookingsRepository = Mocks.GetBookingsMock(bookingCode, "DEFAULT", "DEFAULT", price, 100);
            var bookingApplication = new BookingsApplication(
                Mocks.GetVehiclesMock("DEFAULT", 100).Object,
                Mocks.GetCustomersMock("DEFAULT").Object,
                bookingsRepository.Object,
                Mocks.GetAutoMapperConfigure());

            var checklistInput = new VehicleChecklistDto
            {
                Clean = clean,
                FullFuelTank = fullFuelTank,
                Scratches = scratches,
                Smashed = smashed
            };

            var bookingAfterChecklist = await bookingApplication.GetBookingAfterChecklistAsync(checklistInput, bookingCode, default);
            var incrementPerItem = price * 0.3;
            var expectedPrice = price + (countIncidents * incrementPerItem);
            Assert.Equal(
                Math.Round(bookingAfterChecklist.Price.Value, 4, MidpointRounding.AwayFromZero), 
                Math.Round(expectedPrice, 4, MidpointRounding.AwayFromZero)
                );
        }
    }
}
