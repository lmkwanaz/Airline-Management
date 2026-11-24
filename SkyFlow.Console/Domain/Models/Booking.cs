namespace SkyFlow.Console.Domain.Models;

public class Booking
{
    private string _bookingStatus;

    public int Id { get; }
    public int FlightId { get; }
    public int PassengerId { get; }
    public string SeatNumber { get; }
    public string BookingStatus => _bookingStatus;

    public Booking(int id, int flightId, int passengerId, string seatNumber, string bookingStatus)
    {
        Id = id;
        FlightId = flightId;
        PassengerId = passengerId;
        SeatNumber = seatNumber;
        _bookingStatus = bookingStatus;
    }

    public void CheckIn()
    {
        if (_bookingStatus != "Booked")
        {
            throw new InvalidOperationException("Only booked passengers can be checked in.");
        }

        _bookingStatus = "CheckedIn";
    }

    public void MarkBoarded()
    {
        if (_bookingStatus != "CheckedIn")
        {
            throw new InvalidOperationException("Only checked-in passengers can be boarded.");
        }

        _bookingStatus = "Boarded";
    }
}
