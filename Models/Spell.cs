namespace dnd_utils.Models;

public class Spell
{
    public int SpellId { get; set; }
    public string Name { get; set; }
    public string School { get; set; }
    public int Level { get; set; }
    public string PageContent { get; set; }
    public string[] InLists { get; set; }
    public string Source { get; set; }
    public string CastingTime { get; set; }
    public string[] Components { get; set; }
    public string Range { get; set; }
    public string Duration { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
