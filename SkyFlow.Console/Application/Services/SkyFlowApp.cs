using SkyFlow.Console.Application.Interfaces;
using SkyFlow.Console.Application.Helpers;
using SkyFlow.Console.Domain.Enums;
using SkyFlow.Console.Domain.Models;
using SkyFlow.Console.Domain.Users;

namespace SkyFlow.Console.Application.Services;

public class SkyFlowApp
{
    private readonly IDataRepository _repository;

    public SkyFlowApp(IDataRepository repository)
    {
        _repository = repository;
    }

    public void Run()
    {
        while (true)
        {
            var user = Login();
            if (user is null)
            {
                break;
            }

            RunDashboard(user);
        }

        global::System.Console.WriteLine("Goodbye from SkyFlow.");
    }

    private User? Login()
    {
        global::System.Console.Clear();
        global::System.Console.WriteLine("SkyFlow > Welcome to SkyFlow Terminal Manager");
        global::System.Console.WriteLine("Type EXIT as username to close the application.");
        global::System.Console.WriteLine();
        global::System.Console.Write("Username: ");
        var username = global::System.Console.ReadLine()?.Trim() ?? string.Empty;

        if (username.Equals("EXIT", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        global::System.Console.Write("Password: ");
        var password = global::System.Console.ReadLine()?.Trim() ?? string.Empty;

        var user = _repository.AuthenticateUser(username, password);
        if (user is null)
        {
            global::System.Console.WriteLine();
            global::System.Console.WriteLine("Authentication failed.");
            Pause();
        }

        return user;
    }

    private void RunDashboard(User user)
    {
        var running = true;

        while (running)
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine($"Logged in as: {user.FullName} ({user.Username})");
            global::System.Console.WriteLine();
            user.DisplayDashboard();
            global::System.Console.Write("Select option: ");

            var choice = global::System.Console.ReadLine();

            if (user is AdminUser)
            {
                running = HandleAdminChoice(choice);
            }
            else if (user is GateAgentUser)
            {
                running = HandleGateAgentChoice(choice);
            }

            if (running)
            {
                Pause();
            }
        }
    }

    private bool HandleAdminChoice(string? choice)
    {
        switch (choice)
        {
            case "1":
                AddFlight();
                return true;
            case "2":
                ShowSystemOverview();
                return true;
            case "3":
                AddStaff();
                return true;
            case "4":
                UpdateFlightStatus();
                return true;
            case "0":
                return false;
            default:
                global::System.Console.WriteLine("Invalid option.");
                return true;
        }
    }

    private bool HandleGateAgentChoice(string? choice)
    {
        switch (choice)
        {
            case "1":
                ShowManifest();
                return true;
            case "2":
                PassengerCheckIn();
                return true;
            case "3":
                BoardingGate();
                return true;
            case "0":
                return false;
            default:
                global::System.Console.WriteLine("Invalid option.");
                return true;
        }
    }

    private void ShowSystemOverview()
    {
        var flights = _repository.GetFlights();
        global::System.Console.WriteLine();
        global::System.Console.WriteLine("SkyFlow > System Overview");

        var rows = flights
            .Select(f => (IReadOnlyList<string>)
            [
                f.Id.ToString(),
                f.FlightNumber,
                f.Origin,
                f.Destination,
                f.DepartureTime.ToString("HH:mm"),
                f.CurrentOccupancy + "/" + f.Capacity,
                f.Status.ToString()
            ])
            .ToList();

        AsciiTableRenderer.Render(
            ["FlightId", "Flight No", "Origin", "Destination", "Departure", "Occupancy", "Status"],
            rows);

        if (flights.Count == 0)
        {
            global::System.Console.WriteLine("No flights found.");
        }
    }

    private void AddFlight()
    {
        global::System.Console.WriteLine("SkyFlow > Manage Flights > Add New Flight");
        global::System.Console.Write("Flight number: ");
        var flightNumber = global::System.Console.ReadLine() ?? string.Empty;

        global::System.Console.Write("Origin: ");
        var origin = global::System.Console.ReadLine() ?? string.Empty;

        global::System.Console.Write("Destination: ");
        var destination = global::System.Console.ReadLine() ?? string.Empty;

        global::System.Console.Write("Departure date/time (yyyy-MM-dd HH:mm): ");
        if (!DateTime.TryParse(global::System.Console.ReadLine(), out var departureTime))
        {
            global::System.Console.WriteLine("Invalid date/time format.");
            return;
        }

        global::System.Console.Write("Aircraft capacity: ");
        if (!int.TryParse(global::System.Console.ReadLine(), out var capacity) || capacity <= 0)
        {
            global::System.Console.WriteLine("Invalid capacity.");
            return;
        }

        var flight = new Flight(0, flightNumber, origin, destination, departureTime, capacity, 0, FlightStatus.Scheduled);
        var id = _repository.AddFlight(flight);
        global::System.Console.WriteLine($"Flight created with ID: {id}");
    }

    private void AddStaff()
    {
        global::System.Console.WriteLine("SkyFlow > Manage Staff > Add New Staff Member");
        global::System.Console.Write("Username: ");
        var username = global::System.Console.ReadLine() ?? string.Empty;

        global::System.Console.Write("Password: ");
        var password = global::System.Console.ReadLine() ?? string.Empty;

        global::System.Console.Write("Role (Administrator/GateAgent): ");
        var role = global::System.Console.ReadLine() ?? string.Empty;

        global::System.Console.Write("Email: ");
        var email = global::System.Console.ReadLine() ?? string.Empty;

        global::System.Console.Write("First name: ");
        var firstName = global::System.Console.ReadLine() ?? string.Empty;

        global::System.Console.Write("Last name: ");
        var lastName = global::System.Console.ReadLine() ?? string.Empty;

        var id = _repository.AddStaffMember(username, password, role, email, firstName, lastName);
        global::System.Console.WriteLine($"Staff user created with UserId: {id}");
    }

    private void UpdateFlightStatus()
    {
        var flight = RequireFlight();
        if (flight is null)
        {
            return;
        }

        global::System.Console.WriteLine("1. Start Boarding");
        global::System.Console.WriteLine("2. Mark Boarded");
        global::System.Console.WriteLine("3. Depart");
        global::System.Console.WriteLine("4. Cancel");
        global::System.Console.Write("Choose status operation: ");
        var statusChoice = global::System.Console.ReadLine();

        try
        {
            switch (statusChoice)
            {
                case "1":
                    flight.StartBoarding();
                    break;
                case "2":
                    flight.MarkBoarded();
                    break;
                case "3":
                    flight.DepartFlight();
                    break;
                case "4":
                    flight.CancelFlight();
                    break;
                default:
                    global::System.Console.WriteLine("Invalid option.");
                    return;
            }

            _repository.UpdateFlightStatus(flight.Id, flight.Status.ToString(), flight.CurrentOccupancy);
            global::System.Console.WriteLine($"Flight status updated to {flight.Status}.");
        }
        catch (InvalidOperationException ex)
        {
            global::System.Console.WriteLine(ex.Message);
        }
    }

    private void ShowManifest()
    {
        global::System.Console.Write("Flight ID: ");
        if (!int.TryParse(global::System.Console.ReadLine(), out var flightId))
        {
            global::System.Console.WriteLine("Invalid flight ID.");
            return;
        }

        var manifest = _repository.GetManifest(flightId);
        global::System.Console.WriteLine();
        global::System.Console.WriteLine("SkyFlow > Flight Manifest");

        var rows = manifest
            .Select(m => (IReadOnlyList<string>)
            [
                m.Passenger.Id.ToString(),
                m.Passenger.FullName,
                m.Passenger.PassportNumber,
                m.Booking.SeatNumber,
                m.Booking.BookingStatus
            ])
            .ToList();

        AsciiTableRenderer.Render(
            ["PassengerId", "Passenger Name", "Passport", "Seat", "Status"],
            rows);

        if (manifest.Count == 0)
        {
            global::System.Console.WriteLine("No bookings for this flight.");
        }
    }

    private void PassengerCheckIn()
    {
        global::System.Console.WriteLine("Gate Agent > Enter passenger ID or Passport Number:");
        var search = global::System.Console.ReadLine() ?? string.Empty;

        var found = _repository.FindPassengerForCheckIn(search);
        if (found is null)
        {
            global::System.Console.WriteLine("Passenger not found.");
            return;
        }

        var (passenger, booking) = found.Value;

        global::System.Console.WriteLine($"Passenger found: {passenger.FullName} (Seat {booking.SeatNumber})");
        global::System.Console.WriteLine($"Current status: {booking.BookingStatus}");

        if (!booking.BookingStatus.Equals("Booked", StringComparison.OrdinalIgnoreCase))
        {
            global::System.Console.WriteLine("Passenger is not in a Booked state, no check-in performed.");
            return;
        }

        booking.CheckIn();
        _repository.UpdateBookingStatus(booking.Id, booking.BookingStatus);
        global::System.Console.WriteLine("Status updated successfully to CheckedIn.");
    }

    private void BoardingGate()
    {
        var flight = RequireFlight();
        if (flight is null)
        {
            return;
        }

        global::System.Console.WriteLine("1. Finalize as Boarded");
        global::System.Console.WriteLine("2. Finalize as Departed");
        global::System.Console.Write("Choose option: ");

        var option = global::System.Console.ReadLine();
        try
        {
            if (option == "1")
            {
                if (flight.Status == FlightStatus.Scheduled)
                {
                    flight.StartBoarding();
                }

                flight.MarkBoarded();
            }
            else if (option == "2")
            {
                if (flight.Status == FlightStatus.Scheduled)
                {
                    flight.StartBoarding();
                    flight.MarkBoarded();
                }
                else if (flight.Status == FlightStatus.Boarding)
                {
                    flight.MarkBoarded();
                }

                flight.DepartFlight();
            }
            else
            {
                global::System.Console.WriteLine("Invalid option.");
                return;
            }

            _repository.UpdateFlightStatus(flight.Id, flight.Status.ToString(), flight.CurrentOccupancy);
            global::System.Console.WriteLine($"Flight finalized as {flight.Status}.");
        }
        catch (InvalidOperationException ex)
        {
            global::System.Console.WriteLine(ex.Message);
        }
    }

    private Flight? RequireFlight()
    {
        global::System.Console.Write("Flight ID: ");
        if (!int.TryParse(global::System.Console.ReadLine(), out var flightId))
        {
            global::System.Console.WriteLine("Invalid flight ID.");
            return null;
        }

        var flight = _repository.GetFlightById(flightId);
        if (flight is null)
        {
            global::System.Console.WriteLine("Flight not found.");
            return null;
        }

        return flight;
    }

    private static void Pause()
    {
        global::System.Console.WriteLine();
        global::System.Console.WriteLine("Press Enter to continue...");
        global::System.Console.ReadLine();
    }
}
