using Microsoft.Data.SqlClient;
using SkyFlow.Console.Application.Interfaces;
using SkyFlow.Console.Domain.Enums;
using SkyFlow.Console.Domain.Models;
using SkyFlow.Console.Domain.Users;

namespace SkyFlow.Console.Infrastructure.Data;

public class SqlServerDataRepository : IDataRepository
{
    private readonly string _connectionString;

    public SqlServerDataRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public User? AuthenticateUser(string username, string password)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            @"SELECT UserId, Username, Role, FirstName, LastName
              FROM [User]
              WHERE Username = @Username AND PasswordHash = @PasswordHash;",
            connection);

        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@PasswordHash", password);

        connection.Open();
        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        var id = reader.GetInt32(0);
        var userName = reader.GetString(1);
        var role = reader.GetString(2);
        var fullName = $"{reader.GetString(3)} {reader.GetString(4)}";

        return role.Equals("Administrator", StringComparison.OrdinalIgnoreCase)
            ? new AdminUser(id, userName, fullName)
            : new GateAgentUser(id, userName, fullName);
    }

    public int AddStaffMember(string username, string password, string role, string email, string firstName, string lastName)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            @"INSERT INTO [User] (Username, PasswordHash, Role, Email, FirstName, LastName, CreatedAt)
              OUTPUT INSERTED.UserId
              VALUES (@Username, @PasswordHash, @Role, @Email, @FirstName, @LastName, SYSDATETIME());",
            connection);

        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@PasswordHash", password);
        command.Parameters.AddWithValue("@Role", role);
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@FirstName", firstName);
        command.Parameters.AddWithValue("@LastName", lastName);

        connection.Open();
        return (int)command.ExecuteScalar()!;
    }

    public List<Flight> GetFlights()
    {
        var flights = new List<Flight>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            @"SELECT FlightId,
                     FlightNumber,
                     Origin,
                     Destination,
                     DepartureTime,
                     Capacity,
                     CurrentOccupancy,
                     Status
              FROM Flight
              ORDER BY DepartureTime;",
            connection);

        connection.Open();
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            flights.Add(new Flight(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetDateTime(4),
                reader.GetInt32(5),
                reader.GetInt32(6),
                Enum.Parse<FlightStatus>(reader.GetString(7), true)));
        }

        return flights;
    }

    public Flight? GetFlightById(int flightId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            @"SELECT FlightId,
                     FlightNumber,
                     Origin,
                     Destination,
                     DepartureTime,
                     Capacity,
                     CurrentOccupancy,
                     Status
              FROM Flight
              WHERE FlightId = @Id;",
            connection);
        command.Parameters.AddWithValue("@Id", flightId);

        connection.Open();
        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        return new Flight(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetDateTime(4),
                        reader.GetInt32(5),
                        reader.GetInt32(6),
                        Enum.Parse<FlightStatus>(reader.GetString(7), true));
    }

    public int AddFlight(Flight flight)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
                        @"INSERT INTO Flight (FlightNumber, Origin, Destination, DepartureTime, Capacity, Status, CurrentOccupancy)
                            OUTPUT INSERTED.FlightId
                            VALUES (@FlightNumber, @Origin, @Destination, @DepartureTime, @Capacity, @Status, @CurrentOccupancy);",
            connection);

        command.Parameters.AddWithValue("@FlightNumber", flight.FlightNumber);
        command.Parameters.AddWithValue("@Origin", flight.Origin);
        command.Parameters.AddWithValue("@Destination", flight.Destination);
        command.Parameters.AddWithValue("@DepartureTime", flight.DepartureTime);
        command.Parameters.AddWithValue("@Capacity", flight.Capacity);
        command.Parameters.AddWithValue("@Status", flight.Status.ToString());
        command.Parameters.AddWithValue("@CurrentOccupancy", flight.CurrentOccupancy);

        connection.Open();
        return (int)command.ExecuteScalar()!;
    }

    public void UpdateFlightStatus(int flightId, string newStatus, int currentOccupancy)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "UPDATE Flight SET Status = @Status, CurrentOccupancy = @CurrentOccupancy WHERE FlightId = @Id;",
            connection);

        command.Parameters.AddWithValue("@Status", newStatus);
        command.Parameters.AddWithValue("@CurrentOccupancy", currentOccupancy);
        command.Parameters.AddWithValue("@Id", flightId);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public List<(Booking Booking, Passenger Passenger)> GetManifest(int flightId)
    {
        var manifest = new List<(Booking Booking, Passenger Passenger)>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            @"SELECT b.BookingId,
                     b.FlightId,
                     b.PassengerId,
                     b.SeatNumber,
                     b.BookingStatus,
                     p.PassengerId,
                     p.FirstName,
                     p.LastName,
                     p.PassportNumber,
                     p.ContactNumber
              FROM Booking b
              INNER JOIN Passenger p ON p.PassengerId = b.PassengerId
              WHERE b.FlightId = @FlightId
              ORDER BY p.LastName, p.FirstName;",
            connection);

        command.Parameters.AddWithValue("@FlightId", flightId);
        connection.Open();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var booking = new Booking(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetString(4));

            var passenger = new Passenger(
                reader.GetInt32(5),
                reader.GetString(6),
                reader.GetString(7),
                reader.GetString(8),
                reader.GetString(9));

            manifest.Add((booking, passenger));
        }

        return manifest;
    }

    public (Passenger Passenger, Booking Booking)? FindPassengerForCheckIn(string passengerIdOrPassport)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            @"SELECT TOP 1
                     b.BookingId,
                     b.FlightId,
                     b.PassengerId,
                     b.SeatNumber,
                     b.BookingStatus,
                     p.PassengerId,
                     p.FirstName,
                     p.LastName,
                     p.PassportNumber,
                     p.ContactNumber
              FROM Passenger p
              INNER JOIN Booking b ON b.PassengerId = p.PassengerId
              WHERE p.PassengerId = TRY_CAST(@Search AS INT)
                 OR p.PassportNumber = @Search
              ORDER BY b.BookingDate DESC;",
            connection);

        command.Parameters.AddWithValue("@Search", passengerIdOrPassport);

        connection.Open();
        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        var booking = new Booking(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetInt32(2),
            reader.GetString(3),
            reader.GetString(4));

        var passenger = new Passenger(
            reader.GetInt32(5),
            reader.GetString(6),
            reader.GetString(7),
            reader.GetString(8),
            reader.GetString(9));

        return (passenger, booking);
    }

    public void UpdateBookingStatus(int bookingId, string newStatus)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            @"UPDATE Booking
              SET BookingStatus = @Status,
                  CheckInTime = CASE WHEN @Status = 'CheckedIn' THEN SYSDATETIME() ELSE CheckInTime END,
                  BoardingTime = CASE WHEN @Status = 'Boarded' THEN SYSDATETIME() ELSE BoardingTime END
              WHERE BookingId = @Id;

              UPDATE f
              SET f.CurrentOccupancy = x.Occupancy
              FROM Flight f
              INNER JOIN
              (
                  SELECT FlightId, COUNT(*) AS Occupancy
                  FROM Booking
                  WHERE BookingStatus IN ('CheckedIn', 'Boarded')
                  GROUP BY FlightId
              ) x ON x.FlightId = f.FlightId
              WHERE f.FlightId = (SELECT FlightId FROM Booking WHERE BookingId = @Id);

              UPDATE f
              SET f.CurrentOccupancy = 0
              FROM Flight f
              WHERE f.FlightId = (SELECT FlightId FROM Booking WHERE BookingId = @Id)
                AND NOT EXISTS
                (
                    SELECT 1
                    FROM Booking b
                    WHERE b.FlightId = f.FlightId
                      AND b.BookingStatus IN ('CheckedIn', 'Boarded')
                );",
            connection);

        command.Parameters.AddWithValue("@Status", newStatus);
        command.Parameters.AddWithValue("@Id", bookingId);

        connection.Open();
        command.ExecuteNonQuery();
    }
}
