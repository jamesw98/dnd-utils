using System.Text.RegularExpressions;
using Compendium.Enums;
using Compendium.Models;

namespace Compendium.Services;

public partial class DiceService
{
    private readonly Random _rand = new();

    /// <summary>
    /// Rolls detailed, but with advantage or disadvantage
    /// </summary>
    /// <param name="num">The number of dice to roll</param>
    /// <param name="type">The type of die to roll</param>
    /// <param name="mod">The modifier to add or subtract</param>
    /// <param name="state">Advantage or Disadvantage</param>
    /// <param name="modOnEvery">Whether to use the modifier on every die or just the total</param>
    /// <returns>Roll details</returns>
    public (RollDetails, RollDetails) RollDetailsNonStandard(int num, int type, int mod, AdvantageState state, bool modOnEvery=false)
    {
        if (type != 20)
        {
            throw new ArgumentException("(Dis)Advantage can only be used with d20s");
        }

        List<RollDetails> rolls = new (new[] { RollDetailed(num, type, mod), RollDetailed(num, type, mod) });

        rolls = state == AdvantageState.Advantage 
            ? rolls.OrderByDescending(x => x.Total).ToList() 
            : rolls.OrderBy(x => x.Total).ToList();    
        
        return (rolls[0], rolls[1]);
    }
    
    /// <summary>
    /// Rolls dice with more detail than a normal roll
    /// </summary>
    /// <param name="num">The number of dice to roll</param>
    /// <param name="type">The type of die to roll</param>
    /// <param name="mod">The modifier to add or subtract</param>
    /// <param name="modOnEvery">Whether to use the modifier on every die or just the total</param>
    /// <param name="rollNum">Used for displaying what "group" the roll belongs to</param>
    /// <returns></returns>
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
    /// Rolls dice based on a formula such as:
    ///     1d20
    ///     1d20 + 5
    ///     1d20 + 5 + 1d6
    ///     1d20 + 5 + 1d6 + 10
    ///     etc
    /// </summary>
    /// <param name="rawFormula">A string representing the formula to roll</param>
    /// <param name="rollNum">Optional: Used for displaying what "group" this roll belongs to</param>
    /// <returns>An enumerable of roll results</returns>
    public IEnumerable<RollDetails> RollFromFormula(string rawFormula, int rollNum=0)
    {
        Console.WriteLine(rawFormula);
        
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
    
    /// <summary>
    /// Rolls virtual dice based on the parameters
    /// </summary>
    /// <param name="num">The number of dice to roll</param>
    /// <param name="type">The type of die to roll</param>
    /// <param name="mod">The modifier to add or subtract to the roll</param>
    /// <param name="modOnEvery">Whether to use the modifier on every die, or just on the total</param>
    /// <returns>The result of the roll</returns>
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

    public static bool ValidateFormula(string formula)
    {
        // get rid of all white space
        var trimmed = RemoveWhiteSpace().Replace(formula, "");
        // find matches that match the regex
        var matches = DiceMatch().Matches(trimmed);
        
        // something is wrong with the formula the user sent
        return matches.Any();
    }
    
    /// <summary>
    /// Matches a modifier for a dice roll. 
    /// </summary>
    [GeneratedRegex(@"[+-]\s*\d+")]
    public static partial Regex Modifier();
    
    /// <summary>
    /// Regex to remove all whitespace from a string 
    /// </summary>
    [GeneratedRegex("\\s+")]
    public static partial Regex RemoveWhiteSpace();
    
    /// <summary>
    /// Regex used to help with parsing dice formulas 
    /// </summary>
    [GeneratedRegex(@"([+-]?)(\d+d\d+|\d+)")]
    private static partial Regex DiceMatch();
    
}