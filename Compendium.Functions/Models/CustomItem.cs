using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Compendium.Functions.Models;

[BsonIgnoreExtraElements]
public class CustomItem
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public required Guid ItemId { get; set; }
    public required string Name { get; set; }
    public required string FullText { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool Public { get; set; }
}