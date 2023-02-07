namespace dnd_utils.Models;

public class Spellbook
{
    public string Name { get; set; }
    public string Class { get; set; }
    public int Level { get; set; }
    public int? CastingStatValue { get; set; }
    public Dictionary<int, int>? SlotsForLevel { get; set; }  
    public Dictionary<int, List<Spell>> SpellsForLevel { get; set; } = new();
    public List<Spell> AllSpells { get; set; } = new();

    public override string ToString()
    {
        return $"{Name}, {Class}";
    }
}