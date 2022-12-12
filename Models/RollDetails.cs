﻿namespace dnd_utils.Models;

public class RollDetails
{
    public int Num { get; set; }
    public int Die { get; set; }
    public int Mod { get; set; }
    public List<int> Rolls { get; set; } = new();
    public int Total { get; set; }

    public override string ToString()
    {
        var rolls = Rolls.Aggregate("", (current, i) => current + $"[{i}] + ");
        return $"{Num}d{Die} + {Mod} -> {rolls}{Mod} = {Total}";
    }
}