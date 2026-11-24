namespace SkyFlow.Console.Domain.Users;

public abstract class User
{
    public int Id { get; }
    public string Username { get; }
    public string FullName { get; }

    protected User(int id, string username, string fullName)
    {
        Id = id;
        Username = username;
        FullName = fullName;
    }

    public abstract void DisplayDashboard();
}
