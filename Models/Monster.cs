using dnd_utils.Enums;

namespace dnd_utils.Models;

public class Monster
{
    public required string Name { get; set; }
    public int Initiative { get; set; }
    public int PassivePerception { get; set; }
    public int MaxHitPoints { get; set; }

    public int HitPoints { get; set; }
    public int ArmorClass { get; set; }
    public bool PassedSavingThrow { get; set; } = false;
    public List<DamageType> Resistances { get; set; } = new();
    public List<DamageType> Vulnerabilities { get; set; } = new();
    public List<DamageType> Immunities { get; set; } = new();
    public bool Dead { get; set; }

    public override string ToString()
    {
        return Name;
    }
}