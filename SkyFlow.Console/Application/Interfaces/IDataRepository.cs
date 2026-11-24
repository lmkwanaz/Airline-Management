using SkyFlow.Console.Domain.Models;
using SkyFlow.Console.Domain.Users;

namespace SkyFlow.Console.Application.Interfaces;

public interface IDataRepository
{
    User? AuthenticateUser(string username, string password);
    int AddStaffMember(string username, string password, string role, string email, string firstName, string lastName);

    List<Flight> GetFlights();
    Flight? GetFlightById(int flightId);
    int AddFlight(Flight flight);
    void UpdateFlightStatus(int flightId, string newStatus, int currentOccupancy);

    List<(Booking Booking, Passenger Passenger)> GetManifest(int flightId);
    (Passenger Passenger, Booking Booking)? FindPassengerForCheckIn(string passengerIdOrPassport);
    void UpdateBookingStatus(int bookingId, string newStatus);
}
