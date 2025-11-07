using Firmeza.Models;

namespace Firmeza.Data.Entities;

public class Admin : Person
{
    public DateTime LastLogin { get; set; }
    
    public void ShowInfo()
    {
        Console.WriteLine($"Admin: {FullName}, Last login: {LastLogin}");
    }
}