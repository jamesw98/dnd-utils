using System.IdentityModel.Tokens.Jwt;
using Compendium.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Compendium.Functions.Utils;

public class AuthUtil
{
    public async Task<User?> GetUser(IHeaderDictionary headers)
    {
        var token = await ValidateToken(headers);
        if (token is JwtSecurityToken jwt)
        {
            try
            {
                return new User(jwt.Claims);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
        
        return null;
    }
    
    private async Task<SecurityToken?> ValidateToken(IHeaderDictionary headers) 
    {
        // Attempt to get the Authorization header.
        if (!headers.TryGetValue("Authorization", out var authToken))
        {
            return null;
        }

        // Get the JWT from the header.
        var jwt = authToken.First()!.Split(" ")[1].Trim();
        
        // Do some setup. 
        // https://auth0.com/blog/how-to-validate-jwt-dotnet/
        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"https://dev-ynk4u5ds6uw2ccfl.us.auth0.com/.well-known/openid-configuration",
        new OpenIdConnectConfigurationRetriever(),
        new HttpDocumentRetriever());

        var discoveryDocument = await configurationManager.GetConfigurationAsync();
        var signingKeys = discoveryDocument.SigningKeys;
        
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://dev-ynk4u5ds6uw2ccfl.us.auth0.com/",
            ValidateAudience = true,
            ValidAudience = "dvWTe0G1XvXSQoFOVNw0GbdnmSzXFLsQ",
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = signingKeys,
            ValidateLifetime = true
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(jwt, validationParameters, out var token);
            return token;
        }
        catch (SecurityTokenValidationException ex)
        {
            return null;
        }
    }
}