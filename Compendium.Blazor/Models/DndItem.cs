namespace Compendium.Models;

public class DndItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PageContent { get; set; }
    public string Source { get; set; }
    public string Type { get; set; }
    public string Rarity { get; set; }
    public bool RequiresAttunment { get; set; }
}