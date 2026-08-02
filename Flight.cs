using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AirportTicketBookingSystem
{
    internal class Flight
    {
        public int FlightID { get; set; }

        [Required]
        public string DepartureCountry { get; set; }

        [Required]
        public string DestinationCountry { get; set; }

        [Required]
        public string DepartureAirport { get; set; }
        [Required]
        public string ArrivalAirport { get; set; }

        [Required]
        [FutureDate]
        public DateTime DepartureDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal EconomyPrice { get; set; }
        [Range(0, double.MaxValue)]
        public decimal BusinessPrice { get; set; }
        [Range(0, double.MaxValue)]
        public decimal FirstClassPrice { get; set; }




    }
}
