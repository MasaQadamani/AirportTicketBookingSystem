using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AirportTicketBookingSystem
{
    internal class FlightService
    {
        private List<Flight> flights;

        public FlightService(List<Flight> flights)
        {
            this.flights = flights;
        }
        public Flight GetFlightById(int flightId)
        {
            return flights.FirstOrDefault(f => f.FlightID == flightId);
        }
        public List<Flight> Search(
            decimal? maxPrice = null,
            string departureCountry = null,
            string destinationCountry = null,
            DateTime? departureDate = null,
            string departureAirport = null,
            string arrivalAirport = null,
            FlightClass? flightClass = null)
        {
            var results = flights.AsEnumerable();

            if (departureCountry != null)
            {
                results = results.Where(f => f.DepartureCountry == departureCountry);
            }
            if (destinationCountry != null)
            {
                results = results.Where(f => f.DestinationCountry == destinationCountry);
            }
            if (departureAirport != null)
            {
                results = results.Where(f => f.DepartureAirport == departureAirport);

            }
            if (arrivalAirport != null)
            {
                results = results.Where(f => f.ArrivalAirport == arrivalAirport);
            }
            if (departureDate.HasValue)
            {
                results = results.Where(f => f.DepartureDate.Date == departureDate.Value.Date);
            }

            if (maxPrice.HasValue)
            {
                if (flightClass.HasValue)
                {
                    results = flightClass switch
                    {
                        FlightClass.Economy => results.Where(f => f.EconomyPrice <= maxPrice),
                        FlightClass.Business => results.Where(f => f.BusinessPrice <= maxPrice),
                        FlightClass.FirstClass => results.Where(f => f.FirstClassPrice <= maxPrice),
                        _ => results
                    };
                }
                else
                {

                    results = results.Where(f =>
                        f.EconomyPrice <= maxPrice ||
                        f.BusinessPrice <= maxPrice ||
                        f.FirstClassPrice <= maxPrice);
                }
            }
            return results.ToList();

        }

    }
}
