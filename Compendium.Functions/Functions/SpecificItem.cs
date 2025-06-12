using Compendium.Functions.Models;
using Compendium.Functions.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Compendium.Functions.Functions;

public class SpecificItem(ILogger<SpecificItem> logger, AuthUtil authUtil, MongoUtil mongoUtil)
{
    private readonly ILogger<SpecificItem> _logger = logger;

    [Function("ItemSpecific")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", "delete", Route = "Item/{ItemGuid}")] HttpRequest req, [FromBody] CustomItem customItem, [FromRoute] Guid itemGuid)
    {
        var user = await authUtil.GetUser(req.Headers);
        if (user is null)
        {
            return new UnauthorizedResult();
        }

        return req.Method switch
        {
            "GET" => GetItem(user, itemGuid),
            "PUT" => UpdateItem(user, customItem),
            _ => new ForbidResult()
        };
    }

    /// <summary>
    /// Gets all items a user has created.
    /// </summary>
    /// <param name="user">The user making the request.</param>
    /// <returns>All items the user has created.</returns>
    private OkObjectResult GetItem(User? user, Guid guid)
    {
        return new OkObjectResult(mongoUtil.GetItem(user?.UserId, guid));
    }

    /// <summary>
    /// Create a new item for a user.
    /// </summary>
    /// <param name="user">The user making the request.</param>
    /// <param name="item">The item the user wants to make.</param>
    /// <returns></returns>
    private OkResult UpdateItem(User user, CustomItem item)
    {
        // mongoUtil.CreateItem(user.UserId, item);
        // TODO: Add update item method.
        return new OkResult();
    }
}