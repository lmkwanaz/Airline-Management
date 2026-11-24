namespace SkyFlow.Console.Domain.Users;

public class GateAgentUser : User
{
    public GateAgentUser(int id, string username, string fullName)
        : base(id, username, fullName)
    {
    }

    public override void DisplayDashboard()
    {
        System.Console.WriteLine("Gate Agent Dashboard");
        System.Console.WriteLine("1. Flight Manifest");
        System.Console.WriteLine("2. Passenger Check-in");
        System.Console.WriteLine("3. Boarding Gate");
        System.Console.WriteLine("0. Logout");
    }
}
