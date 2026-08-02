using CsvHelper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AirportTicketBookingSystem
{
    internal class CsvImportService
    {
        public class ImportResult
        {
            public List<Flight> ValidFlights { get; set; } = new();
            public List<string> Errors { get; set; } = new();
        }

        public ImportResult ImportFlights(string filePath)
        {
            var result = new ImportResult();

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<Flight>().ToList();

            int rowNumber = 1; // header is row 1, data starts at row 2
            foreach (var flight in records)
            {
                rowNumber++;

                var context = new ValidationContext(flight);
                var validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(flight, context, validationResults, validateAllProperties: true);

                if (isValid)
                {
                    result.ValidFlights.Add(flight);
                }
                else
                {
                    foreach (var error in validationResults)
                    {
                        result.Errors.Add($"Row {rowNumber}: {error.ErrorMessage}");
                    }
                }
            }

            return result;
        }
    }
}