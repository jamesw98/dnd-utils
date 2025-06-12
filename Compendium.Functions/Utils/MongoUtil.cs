using System.Data;
using Compendium.Functions.Models;
using MongoDB.Driver;

namespace Compendium.Functions.Utils;

public class MongoUtil
{
    private readonly IMongoDatabase _db;
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <exception cref="DataException"></exception>
    public MongoUtil()
    {
        var connString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        if (connString is null)
        {
            throw new DataException("Could not find MongoDbConnectionString environment variable.");
        }
        
        var mongo = new MongoClient(connString);
        _db = mongo.GetDatabase("jw-dev");
    }

    public void CreateItem(string userId, CustomItem item)
    {
        item.UserId = userId;
        GetCollection<CustomItem>().InsertOne(item);
    }

    public CustomItem? GetItem(string? userId, Guid itemId)
    {
        return GetCollection<CustomItem>()
            .AsQueryable()
            .FirstOrDefault(x => x.ItemId == itemId && (x.UserId == userId || x.Public));
    }

    public IEnumerable<CustomItem> GetItemsForUser(string userId)
    {
        var items = GetCollection<CustomItem>()
            .AsQueryable()
            .Where(x => x.UserId == userId);
        
        // Empty out the full text field to make the response smaller. 
        foreach (var i in items)
        {
            i.FullText = string.Empty;
        }

        return items;
    }
    
    private IMongoCollection<T> GetCollection<T>()
    {
        return _db.GetCollection<T>(typeof(T).ToString());
    }
}