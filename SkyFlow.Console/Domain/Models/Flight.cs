using SkyFlow.Console.Domain.Enums;

namespace SkyFlow.Console.Domain.Models;

public class Flight
{
    private FlightStatus _status;
    private int _currentOccupancy;

    public int Id { get; }
    public string FlightNumber { get; }
    public string Origin { get; }
    public string Destination { get; }
    public DateTime DepartureTime { get; }
    public int Capacity { get; }

    public FlightStatus Status => _status;
    public int CurrentOccupancy => _currentOccupancy;

    public Flight(
        int id,
        string flightNumber,
        string origin,
        string destination,
        DateTime departureTime,
        int capacity,
        int currentOccupancy,
        FlightStatus status)
    {
        Id = id;
        FlightNumber = flightNumber;
        Origin = origin;
        Destination = destination;
        DepartureTime = departureTime;
        Capacity = capacity;
        _currentOccupancy = currentOccupancy;
        _status = status;
    }

    public void StartBoarding()
    {
        if (_status != FlightStatus.Scheduled)
        {
            throw new InvalidOperationException("Only scheduled flights can move to boarding.");
        }

        _status = FlightStatus.Boarding;
    }

    public void DepartFlight()
    {
        if (_status != FlightStatus.Boarded)
        {
            throw new InvalidOperationException("A flight can only depart after it is fully boarded.");
        }

        _status = FlightStatus.Departed;
    }

    public void MarkBoarded()
    {
        if (_status != FlightStatus.Boarding)
        {
            throw new InvalidOperationException("A flight can only be marked boarded after boarding starts.");
        }

        _status = FlightStatus.Boarded;
    }

    public void CancelFlight()
    {
        if (_status == FlightStatus.Departed)
        {
            throw new InvalidOperationException("A departed flight cannot be cancelled.");
        }

        _status = FlightStatus.Cancelled;
    }

    public void UpdateOccupancy(int occupancy)
    {
        if (occupancy < 0 || occupancy > Capacity)
        {
            throw new InvalidOperationException("Occupancy must be between 0 and flight capacity.");
        }

        _currentOccupancy = occupancy;
    }
}
