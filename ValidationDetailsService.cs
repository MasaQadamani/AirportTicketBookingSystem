using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AirportTicketBookingSystem
{
    internal class ValidationDetailsService
    {
        public class FieldValidationInfo
        {
            public string FieldName { get; set; }
            public string Type { get; set; }
            public List<string> Constraints { get; set; } = new();
        }

        public List<FieldValidationInfo> GetValidationDetails<T>()
        {
            var details = new List<FieldValidationInfo>();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var info = new FieldValidationInfo
                {
                    FieldName = prop.Name,
                    Type = GetFriendlyTypeName(prop.PropertyType)
                };

                var attributes = prop.GetCustomAttributes(true);

                foreach (var attr in attributes)
                {
                    if (attr is RequiredAttribute)
                        info.Constraints.Add("Required");

                    if (attr is RangeAttribute range)
                        info.Constraints.Add($"Allowed Range ({range.Minimum} - {range.Maximum})");

                    if (attr is FutureDateAttribute futureDate)
                        info.Constraints.Add(futureDate.ConstraintDescription);
                }

                if (info.Constraints.Count == 0)
                    info.Constraints.Add("None");

                details.Add(info);
            }

            return details;
        }

        private string GetFriendlyTypeName(Type type)
        {
            if (type == typeof(string)) return "Free Text";
            if (type == typeof(DateTime)) return "Date Time";
            if (type == typeof(decimal)) return "Decimal Number";
            if (type == typeof(int)) return "Whole Number";
            return type.Name;
        }
    }
}