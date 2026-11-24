namespace SkyFlow.Console.Domain.Models;

public class StaffMember
{
    public int Id { get; }
    public string Username { get; }
    public string Role { get; }
    public string Email { get; }
    public string FullName { get; }

    public StaffMember(int id, string username, string role, string email, string fullName)
    {
        Id = id;
        Username = username;
        Role = role;
        Email = email;
        FullName = fullName;
    }
}
