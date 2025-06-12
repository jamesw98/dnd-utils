using System.Dynamic;
using Blazored.SessionStorage;

namespace Compendium.Blazor.Utils;

public class TokenUtil
{
    public string Jwt { get; set; } = string.Empty;

    /// <summary>
    /// Get the user's JWT from session storage.
    /// Is this a hack? I don't know, but it does work.
    /// </summary>
    /// <param name="storage"></param>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task GetJwt(ISessionStorageService storage)
    {
        var storageValue = await storage.GetItemAsync<ExpandoObject>("oidc.user:https://dev-ynk4u5ds6uw2ccfl.us.auth0.com:dvWTe0G1XvXSQoFOVNw0GbdnmSzXFLsQ");
        if (storageValue is null)
        {
            throw new UnauthorizedAccessException("Not authenticated");
        }

        var token = storageValue.FirstOrDefault(x => x.Key == "id_token").Value;
        if (token is null)
        {
            throw new UnauthorizedAccessException("Not authenticated");
        }
        
        Jwt = token.ToString()!;
        Console.WriteLine(Jwt);
    }
}