using Compendium.Functions.Models;
using Compendium.Functions.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Compendium.Functions.Functions;

public class BasicItems(ILogger<BasicItems> logger, AuthUtil authUtil, MongoUtil mongoUtil)
{
    private readonly ILogger<BasicItems> _logger = logger;

    [Function("Item")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req, [FromBody] CustomItem customItem)
    {
        var user = await authUtil.GetUser(req.Headers);
        if (user is null)
        {
            return new UnauthorizedResult();
        }

        return req.Method switch
        {
            "GET" => GetItems(user),
            "POST" => Post(user, customItem),
            _ => new ForbidResult()
        };
    }

    /// <summary>
    /// Gets all items a user has created.
    /// </summary>
    /// <param name="user">The user making the request.</param>
    /// <returns>All items the user has created.</returns>
    private OkObjectResult GetItems(User user)
    {
        return new OkObjectResult(mongoUtil.GetItemsForUser(user.UserId));
    }

    /// <summary>
    /// Create a new item for a user.
    /// </summary>
    /// <param name="user">The user making the request.</param>
    /// <param name="item">The item the user wants to make.</param>
    /// <returns></returns>
    private OkResult Post(User user, CustomItem item)
    {
        mongoUtil.CreateItem(user.UserId, item);
        return new OkResult();
    }

}