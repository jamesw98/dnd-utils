namespace Compendium.Models;

public class SpellOptions
{
    public List<string> CastingTimes { get; set; } = new();
    public List<string> Durations { get; set; } = new();
    public List<string> Ranges { get; set; } = new();
    public List<string> Sources { get; set; } = new();
}