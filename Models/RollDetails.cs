namespace dnd_utils.Models;

public class RollDetails
{
    public int? Num { get; set; }
    public int? Die { get; set; }
    public int? Mod { get; set; }
    public List<int>? Rolls { get; set; } = new();
    public int Total { get; set; }
    public bool Fixed { get; set; }
    public bool ModOnEvery { get; set; } = false;
    public string? Formula { get; set; }
    public int? RollNum { get; set; }

    public string ToSimpleString()
    {
        var result = "";
        if (Formula != null)
        {
            return result;
        }
        
        if (Rolls == null)
        {
            return result;
        }

        var sign = Mod < 0
            ? "-"
            : "+";
        
        var modString = $"{sign} {Math.Abs(Mod ?? 0)} ";
        
        foreach (var r in Rolls)
        {
            result += $"[{r}] ";
            if (ModOnEvery)
            {
                result += modString;
            }
        }

        if (!ModOnEvery)
        {
            result += modString;
        }
        
        return result;
    }
    
    public override string ToString()
    {
        if (Fixed)
        {
            return $"Fixed roll, total: {Total}";
        }
        
        var rolls = Rolls.Aggregate("", (current, i) => current + $"[{i}] + ");
        return $"{Num}d{Die} + {Mod} -> {rolls}{Mod} = {Total}";
    }
}