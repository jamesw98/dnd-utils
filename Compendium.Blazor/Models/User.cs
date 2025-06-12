namespace Compendium.Blazor.Models;

public class User
{
    private const string InvalidJwtMessage = "Token is invalid.";
    
    public string UserId { get; set; }
    public string Nickname { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
}