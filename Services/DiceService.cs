using dnd_utils.Enums;
using dnd_utils.Models;

namespace dnd_utils.Services;

public class DiceService
{
    private Random _rand = new();

    public (RollDetails, RollDetails) RollDetailsNonStandard(int num, int type, int mod, AdvantageState state, bool modOnEvery=false)
    {
        if (type != 20)
            throw new Exception("(Dis)Advantage can only be used with d20s");

        List<RollDetails> rolls = new (new[] { RollDetailed(num, type, mod), RollDetailed(num, type, mod) });

        if (state == AdvantageState.Advantage)
            rolls = rolls.OrderByDescending(x => x.Total).ToList();
        else 
            rolls = rolls.OrderBy(x => x.Total).ToList();    
        
        return (rolls[0], rolls[1]);
    }
    
    public RollDetails RollDetailed(int num, int type, int mod, bool modOnEvery=false)
    {
        var total = 0;
        List<int> rolls = new();
        for (var i = 0; i < num; i++)
        {
            var currentRoll = Roll(1, type, 0);
            total += currentRoll;
            rolls.Add(currentRoll);
        }

        total += modOnEvery ? mod * num : mod;

        return new RollDetails
        {
            Rolls = rolls,
            Num = num,
            Die = type,
            Mod = mod,
            Total = total,
            Fixed = false
        };
    }
    
    public int Roll(int num, int type, int mod, bool modOnEvery=false)
    {
        var result = 0;

        for (var i = 0; i < num; i++)
            result += _rand.Next(1, type + 1);

        result += modOnEvery ? mod * num : mod;
        return result;
    }
}