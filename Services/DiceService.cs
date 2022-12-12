using dnd_utils.Models;

namespace dnd_utils.Services;

public class DiceService
{
    private Random _rand = new();

    public RollDetails? RollDetailedAdvantage(int num, int type, int mod)
    {
        return RollDetailsNonStandardStart(num, type, mod).MaxBy(x => x.Total);
    }
    
    public RollDetails? RollDetailedDisadvantage(int num, int type, int mod)
    {
        return RollDetailsNonStandardStart(num, type, mod).MinBy(x => x.Total);
    }

    private RollDetails?[] RollDetailsNonStandardStart(int num, int type, int mod)
    {
        if (type != 20)
            throw new Exception("Advantage can only be used with d20s");

        RollDetails?[] rolls = { RollDetailed(num, type, mod), RollDetailed(num, type, mod) };
        return rolls;
    }
    
    public RollDetails RollDetailed(int num, int type, int mod)
    {
        var total = 0;
        List<int> rolls = new();
        for (var i = 0; i < num; i++)
        {
            var currentRoll = Roll(1, type, 0);
            total += currentRoll;
            rolls.Add(currentRoll);
        }

        total += mod;

        return new RollDetails
        {
            Rolls = rolls,
            Num = num,
            Die = type,
            Mod = mod,
            Total = total
        };
    }
    
    public int Roll(int num, int type, int mod)
    {
        var result = 0;

        for (var i = 0; i < num; i++)
            result += _rand.Next(type + 1);

        return result + mod;
    }
}