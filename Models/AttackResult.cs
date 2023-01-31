using dnd_utils.Enums;

namespace dnd_utils.Models;

public class AttackResult
{
    public string Name { get; set; }
    public RollDetails AttackDetail { get; set; } // details about the attack roll
    public RollDetails? UnusedAttackDetail { get; set; } // this is used in the case of (dis)advantage
    public RollDetails DamageDetail { get; set; } // details about the damage roll
    public DamageType Type { get; set; }
    public bool Crit { get; set; }
    public int AcToBeat { get; set; }
    public int Damange { get; set; }
    public bool Hit { get; set; }
    public List<Monster> CreaturesTargeted { get; set; } = new();
    public List<Monster> CreaturesHit { get; set; } = new();
}