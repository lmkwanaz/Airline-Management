namespace SkyFlow.Console.Domain.Models;

public class Passenger
{
    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string PassportNumber { get; }
    public string ContactNumber { get; }

    public string FullName => $"{FirstName} {LastName}";

    public Passenger(int id, string firstName, string lastName, string passportNumber, string contactNumber)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        PassportNumber = passportNumber;
        ContactNumber = contactNumber;
    }
}
