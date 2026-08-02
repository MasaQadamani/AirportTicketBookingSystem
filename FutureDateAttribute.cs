using System;
using System.ComponentModel.DataAnnotations;

namespace AirportTicketBookingSystem
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public string ConstraintDescription => "Allowed Range (today -> future)";

        public FutureDateAttribute()
        {
            ErrorMessage = "Departure date must be today or a future date.";
        }

        public override bool IsValid(object value)
        {
            if (value is not DateTime date)
                return false;

            return date.Date >= DateTime.Today;
        }
    }
}