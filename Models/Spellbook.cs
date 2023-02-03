﻿namespace dnd_utils.Models;

public class Spellbook
{
    public string Name { get; set; }
    public string Class { get; set; }
    public int Level { get; set; }
    public Dictionary<int, Spell> SpellsForLevel { get; set; } = new();
    public List<Spell> AllSpells { get; set; } = new();
}