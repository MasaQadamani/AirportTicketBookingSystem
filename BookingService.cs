using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AirportTicketBookingSystem
{
    internal class BookingService
    {
        private List<Booking> bookings;
        private FlightService flightService;

        public BookingService(List<Booking> bookings, FlightService flightService)
        {
            this.bookings = bookings;
            this.flightService = flightService;
        }

        public Booking BookFlight(Flight flight, int passengerId, FlightClass flightClass)
        {
            decimal price = flightClass switch
            {
                FlightClass.Economy => flight.EconomyPrice,
                FlightClass.Business => flight.BusinessPrice,
                FlightClass.FirstClass => flight.FirstClassPrice,
            };

            Booking booking = new Booking
            {
                BookingId = bookings.Count + 1,
                FlightID = flight.FlightID,
                PassengerId = passengerId,
                FlightClass = flightClass,
                Price = price
            };

            bookings.Add(booking);
            return booking;
        }

        public void CancelBooking(int bookingId)
        {
            Booking booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null)
            {
                Console.WriteLine("Booking not found to be cancelled.");
                return;
            }

            booking.Status = BookingStatus.Cancelled;
            Console.WriteLine($"Booking {bookingId} has been cancelled.");
        }

        public void ModifyBooking(int bookingId, FlightClass newClass, Flight flight)
        {
            Booking booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null)
            {
                Console.WriteLine("Booking not found to be modified.");
                return;
            }

            decimal newPrice = newClass switch
            {
                FlightClass.Economy => flight.EconomyPrice,
                FlightClass.Business => flight.BusinessPrice,
                FlightClass.FirstClass => flight.FirstClassPrice,
            };

            booking.FlightClass = newClass;
            booking.Price = newPrice;

            Console.WriteLine($"Booking {bookingId} updated to {newClass}, new price: {newPrice}");
        }

        public List<Booking> ViewBookings(int passengerId)
        {
            return bookings.Where(b => b.PassengerId == passengerId).ToList();
        }

        public List<Booking> FilterBookings(
    int? flightId = null,
    decimal? maxPrice = null,
    string departureCountry = null,
    string destinationCountry = null,
    DateTime? departureDate = null,
    string departureAirport = null,
    string arrivalAirport = null,
    int? passengerId = null,
    FlightClass? flightClass = null)
        {
            var results = bookings.AsEnumerable();

            if (flightId.HasValue)
                results = results.Where(b => b.FlightID == flightId.Value);

            if (passengerId.HasValue)
                results = results.Where(b => b.PassengerId == passengerId.Value);

            if (flightClass.HasValue)
                results = results.Where(b => b.FlightClass == flightClass.Value);

            if (maxPrice.HasValue)
                results = results.Where(b => b.Price <= maxPrice.Value);

            if (departureCountry != null)
                results = results.Where(b =>
                {
                    var flight = flightService.GetFlightById(b.FlightID);
                    return flight != null && flight.DepartureCountry == departureCountry;
                });

            if (destinationCountry != null)
                results = results.Where(b =>
                {
                    var flight = flightService.GetFlightById(b.FlightID);
                    return flight != null && flight.DestinationCountry == destinationCountry;
                });

            if (departureAirport != null)
                results = results.Where(b =>
                {
                    var flight = flightService.GetFlightById(b.FlightID);
                    return flight != null && flight.DepartureAirport == departureAirport;
                });

            if (arrivalAirport != null)
                results = results.Where(b =>
                {
                    var flight = flightService.GetFlightById(b.FlightID);
                    return flight != null && flight.ArrivalAirport == arrivalAirport;
                });

            if (departureDate.HasValue)
                results = results.Where(b =>
                {
                    var flight = flightService.GetFlightById(b.FlightID);
                    return flight != null && flight.DepartureDate.Date == departureDate.Value.Date;
                });

            return results.ToList();
        }
    }
}