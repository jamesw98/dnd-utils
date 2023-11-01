using System.Text.RegularExpressions;
using dnd_utils.Enums;
using dnd_utils.Models;

namespace dnd_utils.Services;

public partial class DiceService
{
    private readonly Random _rand = new();

    public (RollDetails, RollDetails) RollDetailsNonStandard(int num, int type, int mod, AdvantageState state, bool modOnEvery=false)
    {
        if (type != 20)
        {
            throw new Exception("(Dis)Advantage can only be used with d20s");
        }

        List<RollDetails> rolls = new (new[] { RollDetailed(num, type, mod), RollDetailed(num, type, mod) });

        rolls = state == AdvantageState.Advantage 
            ? rolls.OrderByDescending(x => x.Total).ToList() 
            : rolls.OrderBy(x => x.Total).ToList();    
        
        return (rolls[0], rolls[1]);
    }
    
    public RollDetails RollDetailed(int num, int type, int mod, bool modOnEvery=false, int rollNum=-1)
    {
        var total = 0;
        List<int> rolls = new();
        for (var i = 0; i < num; i++)
        {
            var currentRoll = Roll(1, type, 0);
            total += currentRoll;
            rolls.Add(currentRoll);
        }

        total += modOnEvery 
            ? mod * num 
            : mod;

        return new RollDetails
        {
            Rolls = rolls,
            Num = num,
            Die = type,
            Mod = mod,
            Total = total,
            Fixed = false,
            ModOnEvery = modOnEvery,
            RollNum = rollNum != -1 
                ? rollNum 
                : null
        };
    }

    /// <summary>
    /// WIP
    /// </summary>
    /// <param name="rawFormula"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<RollDetails> RollFromFormula(string rawFormula, int rollNum)
    {
        var runningTotal = 0;
        List<RollDetails> details = new();
        
        // get rid of all white space
        var trimmed = RemoveWhiteSpace().Replace(rawFormula, "");
        // find matches that match the regex
        var matches = DiceMatch().Matches(trimmed);
        
        // something is wrong with the formula the user sent
        if (!matches.Any())
        {
            throw new ArgumentException("Could not parse formula");
        }

        // loop through matches and calculate them
        foreach (var m in matches)
        {
            var subtract = false;
            if (m == null)
            {
                throw new ArgumentException("Could not parse formula");
            }
            
            var match = (Match)m;
            var value = match.Value;

            // determine if we need to subtract from the running total or not
            if (match.Value[0] is '-' or '+')
            {
                value = match.Value[1..];
                subtract = match.Value[0] is '-';
            }
            
            // we're rolling dice! parse the number of dice and die type, then roll it
            if (value.Contains('d'))
            {
                var split = value.Split('d');
                if (!int.TryParse(split[0], out var numDice) || !int.TryParse(split[1], out var dieType))
                {
                    throw new ArgumentException("Could not parse die type or number of dice");
                } 

                var detail = RollDetailed(numDice, dieType, 0);
                detail.RollNum = rollNum;
                
                if (subtract)
                {
                    detail.Total = -detail.Total;
                }

                runningTotal += detail.Total;
                details.Add(detail);
            }
            else
            {
                if (!int.TryParse(match.Value, out var modifier))
                {
                    throw new ArgumentException("Could not parse modifier");
                }

                runningTotal += modifier;
            }
        }
        
        details.Add(new RollDetails
        {
            Formula = rawFormula,
            Total = runningTotal,
            RollNum = rollNum
        });

        return details;
    }
    
    public int Roll(int num, int type, int mod, bool modOnEvery=false)
    {
        var result = 0;

        for (var i = 0; i < num; i++)
        {
            result += _rand.Next(1, type + 1);
        }

        result += modOnEvery 
            ? mod * num 
            : mod;
        
        return result;
    }

    [GeneratedRegex(@"([+-]?)(\d+d\d+|\d+)")]
    private static partial Regex DiceMatch();
    
    [GeneratedRegex("\\s+")]
    private static partial Regex RemoveWhiteSpace();
    [GeneratedRegex("(\\+-)")]
    private static partial Regex SplitOnModifier();
}