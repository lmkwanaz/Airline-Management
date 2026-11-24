namespace SkyFlow.Console.Domain.Users;

public class AdminUser : User
{
    public AdminUser(int id, string username, string fullName)
        : base(id, username, fullName)
    {
    }

    public override void DisplayDashboard()
    {
        System.Console.WriteLine("Admin Dashboard");
        System.Console.WriteLine("1. Manage Flights");
        System.Console.WriteLine("2. View System Overview");
        System.Console.WriteLine("3. Manage Staff");
        System.Console.WriteLine("4. Update Flight Status");
        System.Console.WriteLine("0. Logout");
    }
}
