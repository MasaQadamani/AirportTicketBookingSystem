using Xunit;
using System.Collections.Generic;

namespace AirportTicketBookingSystem.Tests
{
    public class BookingServiceShould
    {
        [Fact]
        public void RejectCancellation_When_BookingBelongsToDifferentPassenger()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking { BookingId = 1, PassengerId = 1, FlightID = 1, Price = 150, Status = BookingStatus.Active }
            };
            var flightService = new FlightService(new List<Flight>());
            var bookingService = new BookingService(bookings, flightService);

            // Act
            bookingService.CancelBooking(bookingId: 1, passengerId: 2, isConfirmed: true); // first test

            // Assert
            Assert.Equal(BookingStatus.Active, bookings[0].Status);
        }

        [Fact]
        public void SetStatusToCancelled_When_ValidBookingIsCancelled()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking { BookingId = 1, PassengerId = 1, FlightID = 1, Price = 150, Status = BookingStatus.Active }
            };
            var flightService = new FlightService(new List<Flight>());
            var bookingService = new BookingService(bookings, flightService);

            // Act
            bookingService.CancelBooking(bookingId: 1, passengerId: 1, isConfirmed: true); // second test
            // Assert
            Assert.Equal(BookingStatus.Cancelled, bookings[0].Status);
        }
    }
}