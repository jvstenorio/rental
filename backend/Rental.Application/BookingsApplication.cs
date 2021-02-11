using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Rental.Domain.Applications;
using Rental.Domain.Entities;
using Rental.Domain.Enumerations;
using Rental.Domain.Models;
using Rental.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
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
            IMapper mapper
            )
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
            bookingDto.Date = DateTimeOffset.Now;
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

        public async Task<byte[]> GetContractFromBookingAsync(string bookingCode, CancellationToken cancellationToken)
        {
            var booking = await _bookingsRepository.GetByIdentifierAsync(Booking.GetIdentifier(bookingCode), cancellationToken);
            if (booking == null) 
            {
                throw new ValidationException("Booking not found");
            }
            var customerTask = _usersApplication.GetCustomerAsync(booking.Cpf, cancellationToken);
            var vehicleTask = _vehiclesApplication.GetVehicleAsync(booking.Plate, cancellationToken);
            await Task.WhenAll(customerTask, vehicleTask);
            var customer = customerTask.Result;
            var vehicle = vehicleTask.Result;

            using var ms = new MemoryStream();
            Document doc = new Document();
            Thread.Sleep(1000);

            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();
            var header = new Paragraph("Contrato");
            header.Alignment = Element.ALIGN_CENTER;
            doc.Add(header);
            doc.Add(new Paragraph("\n\n"));
            doc.Add(GetVehicleTable(vehicle));
            doc.Add(new Paragraph("\n"));
            doc.Add(GetCustomerTable(customer));
            doc.Add(new Paragraph("\n"));

            doc.Add(BookingTable(booking));
            doc.Add(new Paragraph("\n\n"));

            var footer = new Paragraph("Assinatura");
            footer.Alignment = Element.ALIGN_CENTER;
            doc.Add(footer);

            doc.Close();
            return ms.ToArray();
        }

        public async Task<List<BookingDto>> GetBookingsByCpfAsync(string cpf, CancellationToken cancellationToken) 
        {
            var bookings = await _bookingsRepository.GetBookingsByCpfAsync(cpf, cancellationToken);
            var bookingsDto = bookings.Select(b => _mapper.Map<BookingDto>(b)).ToList();
            return bookingsDto;
        }
        private PdfPTable GetVehicleTable(VehicleDto vehicle) 
        {

            PdfPTable table = new PdfPTable(3);

            var cell = new PdfPCell(new Phrase($"Veículo: {vehicle.Make} {vehicle.Model} {vehicle.Year}"));
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            table.AddCell($"Placa: {vehicle.Plate}");
            table.AddCell($"Categoria: {vehicle.Category}");
            table.AddCell($"Preço Hora: {vehicle.PricePerHour.Value}");
            table.AddCell($"Combustível: {vehicle.Fuel}");

            return table;
        }

        private PdfPTable GetCustomerTable(CustomerDto customer)
        {

            PdfPTable table = new PdfPTable(3);

            var cell = new PdfPCell(new Phrase($"Cliente: {customer.Name}"));
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            table.AddCell($"CPF: {customer.Cpf}");
            table.AddCell($"Nasc.: {customer.BirthDate.Value.ToString("dd/MM/yyyy")}");
            table.AddCell($"Cidade: {customer.Address.City}");
            table.AddCell($"Estado: {customer.Address.State}");

            return table;
        }

        private PdfPTable BookingTable(Booking booking)
        {

            PdfPTable table = new PdfPTable(3);

            var cell = new PdfPCell(new Phrase($"Reserva: {booking.BookingCode}"));
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            table.AddCell($"Preço: {booking.Price} R$");
            table.AddCell($"Data: {booking.Date.ToString("dd/MM/yyyy")}");

            return table;
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
