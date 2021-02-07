using AutoMapper;
using Rental.Domain.Applications;
using Rental.Domain.Entities;
using Rental.Domain.Enumerations;
using Rental.Domain.Models;
using Rental.Domain.Repositories;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Application
{
    public class BookingsApplication : IBookingsApplication
    {
        private readonly IVehiclesApplication _vehiclesApplication;
        private readonly IUsersApplication _usersApplication;
        private readonly IBookingsRepository _bookingsRepository;
        private readonly IMapper _mapper;

        public BookingsApplication(
            IVehiclesApplication vehiclesApplication,
            IUsersApplication usersApplication,
            IBookingsRepository bookingsRepository,
            IMapper mapper)
        {
            _vehiclesApplication = vehiclesApplication;
            _usersApplication = usersApplication;
            _bookingsRepository = bookingsRepository;
            _mapper = mapper;
        }

        public async Task<BookingDto> GetQuotationAsync(string plate, int totalHours, CancellationToken cancellationToken)
        {
            var vehicle = await _vehiclesApplication.GetVehicleAsync(plate, cancellationToken);
            if (vehicle == null || vehicle.PricePerHour == null)
            {
                return default;
            }
            else
            {
                return new BookingDto
                {
                    Plate = vehicle.Plate,
                    TotalHours = totalHours,
                    Price = GetBookingPrice(vehicle, totalHours)
                };
            }
        }

        public async Task<BookingDto> CreateBookingAsync(BookingDto bookingDto, CancellationToken cancellationToken)
        {
            ValidateBookingRequest(bookingDto);
            var customerTask = _usersApplication.GetCustomerAsync(bookingDto.Cpf, cancellationToken);
            var vehicleTask = _vehiclesApplication.GetVehicleAsync(bookingDto.Plate, cancellationToken);
            await Task.WhenAll(customerTask, vehicleTask);
            if (vehicleTask.Result == null)
            {
                throw new ValidationException("Customer not found");
            }
            if (vehicleTask.Result == null)
            {
                throw new ValidationException("Vehicle not found");
            }
            if (await _bookingsRepository.VehicleHasOpenedBookingAsync(bookingDto.Plate, cancellationToken))
            {
                throw new ValidationException("Vehicle has opened booking");
            }

            bookingDto.BookingCode = GenerateRandomCode(8);
            bookingDto.Price = GetBookingPrice(vehicleTask.Result, bookingDto.TotalHours.Value);
            var booking = _mapper.Map<Booking>(bookingDto);
            booking.Identifier = Booking.GetIdentifier(bookingDto.BookingCode);
            booking.Status = BookingStatus.OPEN.ToString();
            await _bookingsRepository.AddAsync(booking, cancellationToken);
            return bookingDto;
        }

        public async Task<BookingDto> GetBookingAfterChecklistAsync(VehicleChecklistDto checklist, string bookingCode, CancellationToken cancellationToken)
        {
            var booking = await _bookingsRepository.GetByIdentifierAsync(Booking.GetIdentifier(bookingCode), cancellationToken);
            if (booking == null)
            {
                throw new ValidationException("Booking not found");
            }
            var bookingDto =  _mapper.Map<BookingDto>(booking);
            bookingDto.Price = GetPriceAfterChecklist(checklist, booking.Price);
            return bookingDto;
        }

        private double GetPriceAfterChecklist(VehicleChecklistDto checklist, double price) 
        {
            var priceIncrementPerItem = price * 0.3;
            if (!checklist.FullFuelTank) 
            {
                price += priceIncrementPerItem;
            }
            if (!checklist.Clean)
            {
                price += priceIncrementPerItem;
            }
            if (checklist.Smashed)
            {
                price += priceIncrementPerItem;
            }
            if (checklist.Scratches)
            {
                price += priceIncrementPerItem;
            }
            return price;
        }


        private void ValidateBookingRequest(BookingDto bookingDto) 
        {
            if (
                bookingDto == null ||
                string.IsNullOrEmpty(bookingDto.Cpf) || 
                string.IsNullOrEmpty(bookingDto.Plate) || 
                bookingDto.TotalHours == null
                ) 
            {
                throw new ValidationException("Invalid booking request");
            }
        }

        private string GenerateRandomCode(int length)
        {
            string charPool = "ABCDEFGHIJKLMNOPQRSTUVWXY1234567890";
            StringBuilder rs = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                rs.Append(charPool[(int)(random.NextDouble() * charPool.Length)]);
            }
            return rs.ToString();
        }
        private double GetBookingPrice(VehicleDto vehicleDto, int totalHours) => totalHours * vehicleDto.PricePerHour.Value;
    }
}
