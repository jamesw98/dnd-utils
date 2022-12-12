namespace dnd_utils.Models;

public class AttackResult
{
    public RollDetails AttackDetail { get; set; }
    public RollDetails DamageDetail { get; set; }
    public bool Crit { get; set; }
    public int AcToBeat { get; set; }
    public int Damange { get; set; }
    public bool Hit { get; set; }
}