using System;
using System.Collections.Generic;
using System.Text;

namespace AirportTicketBookingSystem.Tests
{
    public class FlightServiceShould
    {

        [Fact]
        public void ReturnOnlyMatchingFlights_When_SearchingByDepartureCountry()
        {
            // Arrange
            var flights = new List<Flight>
            {
                new Flight { FlightID = 1, DepartureCountry = "UK", DestinationCountry = "France", DepartureAirport = "LHR", ArrivalAirport = "CDG", DepartureDate = DateTime.Today.AddDays(5), EconomyPrice = 150, BusinessPrice = 450, FirstClassPrice = 900 },
                new Flight { FlightID = 2, DepartureCountry = "UAE", DestinationCountry = "USA", DepartureAirport = "DXB", ArrivalAirport = "JFK", DepartureDate = DateTime.Today.AddDays(10), EconomyPrice = 600, BusinessPrice = 2200, FirstClassPrice = 5000 },
                new Flight { FlightID = 3, DepartureCountry = "UK", DestinationCountry = "Spain", DepartureAirport = "LHR", ArrivalAirport = "MAD", DepartureDate = DateTime.Today.AddDays(7), EconomyPrice = 200, BusinessPrice = 500, FirstClassPrice = 1000 }
            };
            var flightService = new FlightService(flights);

            // Act
            var results = flightService.Search(departureCountry: "UK");

            // Assert
            Assert.Equal(2, results.Count);
            Assert.All(results, f => Assert.Equal("UK", f.DepartureCountry));
        }
    }
}
