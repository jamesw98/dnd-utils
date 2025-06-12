using System.Security.Claims;
using MongoDB.Bson.Serialization.Attributes;

namespace Compendium.Functions.Models;

[BsonIgnoreExtraElements]
public class User
{
    private const string InvalidJwtMessage = "Token is invalid.";
    
    public string UserId { get; set; }
    public string Nickname { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }

    public User(IEnumerable<Claim> claims)
    {
        UserId = claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? throw new ArgumentException(InvalidJwtMessage);
        Nickname = claims.FirstOrDefault(x => x.Type == "nickname")?.Value ?? throw new ArgumentException(InvalidJwtMessage);
        FirstName = claims.FirstOrDefault(x => x.Type == "given_name")?.Value ?? Nickname;
        LastName = claims.FirstOrDefault(x => x.Type == "family_name")?.Value ?? string.Empty;
    }
}