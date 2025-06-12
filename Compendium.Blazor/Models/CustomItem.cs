namespace Compendium.Blazor.Models;

public class CustomItem
{
    public required Guid ItemId { get; set; }
    public required string Name { get; set; }
    public required string FullText { get; set; }
    public bool Public { get; set; }
}