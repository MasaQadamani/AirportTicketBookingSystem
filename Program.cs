using System;
using System.Collections.Generic;
using System.Linq;

namespace AirportTicketBookingSystem
{
    internal class Program
    {
        static JsonFileRepository<Flight> flightRepo = new JsonFileRepository<Flight>("flights.json");
        static JsonFileRepository<Booking> bookingRepo = new JsonFileRepository<Booking>("bookings.json");

        static List<Flight> flights = LoadInitialFlights();
        static List<Booking> bookings = bookingRepo.Load();

        static FlightService flightService = new FlightService(flights);
        static BookingService bookingService = new BookingService(bookings, flightService);
        static CsvImportService csvImportService = new CsvImportService();
        static ValidationDetailsService validationDetailsService = new ValidationDetailsService();

        static List<Flight> LoadInitialFlights()
        {
            var loaded = flightRepo.Load();
            if (loaded.Count > 0)
                return loaded;

            var seed = new List<Flight>
            {
                new Flight { FlightID = 1, DepartureCountry = "UK", DestinationCountry = "France", DepartureAirport = "LHR", ArrivalAirport = "CDG", DepartureDate = DateTime.Today.AddDays(5), EconomyPrice = 150, BusinessPrice = 450, FirstClassPrice = 900 },
                new Flight { FlightID = 2, DepartureCountry = "UAE", DestinationCountry = "USA", DepartureAirport = "DXB", ArrivalAirport = "JFK", DepartureDate = DateTime.Today.AddDays(10), EconomyPrice = 600, BusinessPrice = 2200, FirstClassPrice = 5000 },
                new Flight { FlightID = 3, DepartureCountry = "Jordan", DestinationCountry = "UK", DepartureAirport = "AMM", ArrivalAirport = "LHR", DepartureDate = DateTime.Today.AddDays(3), EconomyPrice = 300, BusinessPrice = 700, FirstClassPrice = 1500 }
            };
            flightRepo.Save(seed);
            return seed;
        }

        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("=== Airport Ticket Booking System ===");
                Console.WriteLine("1. Passenger");
                Console.WriteLine("2. Manager");
                Console.WriteLine("3. Exit");
                Console.Write("Choose a role: ");
                string roleChoice = Console.ReadLine();

                if (roleChoice == "1")
                    PassengerMenu();
                else if (roleChoice == "2")
                    ManagerMenu();
                else if (roleChoice == "3")
                    exit = true;
                else
                    Console.WriteLine("Invalid choice.");
            }
        }

        static void PassengerMenu()
        {
            Console.Write("\nEnter your Passenger ID: ");
            if (!int.TryParse(Console.ReadLine(), out int passengerId))
            {
                Console.WriteLine("Invalid input. Passenger ID must be a number.");
                return;
            }

            bool running = true;
            while (running)
            {
                Console.WriteLine("\n--- Passenger Menu ---");
                Console.WriteLine("1. Search Flights");
                Console.WriteLine("2. Book a Flight");
                Console.WriteLine("3. View My Bookings");
                Console.WriteLine("4. Cancel a Booking");
                Console.WriteLine("5. Modify a Booking");
                Console.WriteLine("6. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Departure Country (or leave blank): ");
                        string depCountry = Console.ReadLine();
                        depCountry = string.IsNullOrWhiteSpace(depCountry) ? null : depCountry;

                        Console.Write("Destination Country (or leave blank): ");
                        string destCountry = Console.ReadLine();
                        destCountry = string.IsNullOrWhiteSpace(destCountry) ? null : destCountry;

                        var searchResults = flightService.Search(departureCountry: depCountry, destinationCountry: destCountry);

                        if (searchResults.Count == 0)
                        {
                            Console.WriteLine("No flights found.");
                        }
                        else
                        {
                            foreach (var f in searchResults)
                                Console.WriteLine($"[{f.FlightID}] {f.DepartureCountry} -> {f.DestinationCountry} | {f.DepartureAirport} to {f.ArrivalAirport} | {f.DepartureDate:d} | Economy: {f.EconomyPrice}, Business: {f.BusinessPrice}, First: {f.FirstClassPrice}");
                        }
                        break;

                    case "2":
                        Console.Write("Enter Flight ID to book: ");
                        if (!int.TryParse(Console.ReadLine(), out int flightIdToBook))
                        {
                            Console.WriteLine("Invalid input. Flight ID must be a number.");
                            break;
                        }

                        var flightToBook = flightService.GetFlightById(flightIdToBook);
                        if (flightToBook == null)
                        {
                            Console.WriteLine("Flight not found.");
                            break;
                        }

                        Console.Write("Class (Economy / Business / FirstClass): ");
                        if (!Enum.TryParse<FlightClass>(Console.ReadLine(), true, out FlightClass classChoice))
                        {
                            Console.WriteLine("Invalid class. Must be Economy, Business, or FirstClass.");
                            break;
                        }

                        var newBooking = bookingService.BookFlight(flightToBook, passengerId, classChoice);
                        bookingRepo.Save(bookings);
                        Console.WriteLine($"Booked! Booking ID: {newBooking.BookingId}, Price: {newBooking.Price}");
                        break;

                    case "3":
                        var myBookings = bookingService.ViewBookings(passengerId);
                        if (myBookings.Count == 0)
                        {
                            Console.WriteLine("You have no bookings.");
                        }
                        else
                        {
                            foreach (var b in myBookings)
                                Console.WriteLine($"[{b.BookingId}] Flight {b.FlightID} | {b.FlightClass} | {b.Price} | {b.Status}");
                        }
                        break;

                    case "4":
                        Console.Write("Enter Booking ID to cancel: ");
                        if (!int.TryParse(Console.ReadLine(), out int cancelId))
                        {
                            Console.WriteLine("Invalid input. Booking ID must be a number.");
                            break;
                        }

                        bookingService.CancelBooking(cancelId, passengerId);
                        bookingRepo.Save(bookings);
                        break;

                    case "5":
                        Console.Write("Enter Booking ID to modify: ");
                        if (!int.TryParse(Console.ReadLine(), out int modifyId))
                        {
                            Console.WriteLine("Invalid input. Booking ID must be a number.");
                            break;
                        }

                        var bookingToModify = bookings.FirstOrDefault(b => b.BookingId == modifyId);
                        if (bookingToModify == null)
                        {
                            Console.WriteLine("Booking not found.");
                            break;
                        }

                        var flightForModify = flightService.GetFlightById(bookingToModify.FlightID);
                        Console.Write("New Class (Economy / Business / FirstClass): ");
                        if (!Enum.TryParse<FlightClass>(Console.ReadLine(), true, out FlightClass newClass))
                        {
                            Console.WriteLine("Invalid class. Must be Economy, Business, or FirstClass.");
                            break;
                        }

                        bookingService.ModifyBooking(modifyId, newClass, flightForModify);
                        bookingRepo.Save(bookings);
                        break;

                    case "6":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        static void ManagerMenu()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n--- Manager Menu ---");
                Console.WriteLine("1. Filter Bookings");
                Console.WriteLine("2. Import Flights from CSV");
                Console.WriteLine("3. View Flight Model Validation Details");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Filter by Passenger ID (or leave blank): ");
                        string passengerInput = Console.ReadLine();
                        int? passengerIdFilter = null;

                        if (!string.IsNullOrWhiteSpace(passengerInput))
                        {
                            if (!int.TryParse(passengerInput, out int parsedPassengerId))
                            {
                                Console.WriteLine("Invalid input. Passenger ID must be a number.");
                                break;
                            }
                            passengerIdFilter = parsedPassengerId;
                        }

                        var filtered = bookingService.FilterBookings(passengerId: passengerIdFilter);

                        if (filtered.Count == 0)
                        {
                            Console.WriteLine("No bookings found.");
                        }
                        else
                        {
                            foreach (var b in filtered)
                                Console.WriteLine($"[{b.BookingId}] Passenger {b.PassengerId} | Flight {b.FlightID} | {b.FlightClass} | {b.Price} | {b.Status}");
                        }
                        break;

                    case "2":
                        Console.Write("Enter full path to CSV file: ");
                        string filePath = Console.ReadLine().Trim('"');

                        if (!System.IO.File.Exists(filePath))
                        {
                            Console.WriteLine("File not found.");
                            break;
                        }

                        var importResult = csvImportService.ImportFlights(filePath);

                        Console.WriteLine($"\nImported {importResult.ValidFlights.Count} valid flight(s).");
                        flights.AddRange(importResult.ValidFlights);
                        flightRepo.Save(flights);

                        if (importResult.Errors.Count > 0)
                        {
                            Console.WriteLine($"\n{importResult.Errors.Count} error(s) found:");
                            foreach (var error in importResult.Errors)
                                Console.WriteLine($"  - {error}");
                        }
                        break;

                    case "3":
                        var details = validationDetailsService.GetValidationDetails<Flight>();
                        foreach (var field in details)
                        {
                            Console.WriteLine($"\n{field.FieldName}:");
                            Console.WriteLine($"  Type: {field.Type}");
                            Console.WriteLine($"  Constraint: {string.Join(", ", field.Constraints)}");
                        }
                        break;

                    case "4":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}