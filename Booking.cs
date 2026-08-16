using System;

namespace AirportTicketBookingSystem
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int FlightID { get; set; }
        public int PassengerId { get; set; }
        public FlightClass FlightClass { get; set; }
        public decimal Price { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public BookingStatus Status { get; set; } = BookingStatus.Active;
    }
}