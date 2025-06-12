using Compendium.Exceptions;
using Compendium.Models;

namespace Compendium.Services;

public class SpellService
{
    /// <summary>
    /// gets the number of known spells for a given class
    /// </summary>
    /// <param name="className">the class of the character</param>
    /// <param name="level">the level of the character</param>
    /// <param name="castingStatValue">casting stat (wis, int) for the character</param>
    /// <returns>an int representing the # of known spells for the character</returns>
    /// <exception cref="NotFoundException">if the class was not found</exception>
    public int GetNumberOfKnownSpells(string className, int level, int castingStatValue)
    {
        return className.ToLower() switch
        {
            "artificer" => GetHalfCaster(level, castingStatValue),
            "bard" => GetBard(level),
            "cleric" => GetFullCaster(level, castingStatValue), 
            "druid" => GetFullCaster(level, castingStatValue),
            "paladin" => GetHalfCaster(level, castingStatValue),
            "ranger" => GetRanger(level),
            "sorcerer" => GetSorcerer(level),
            "warlock" => GetWarlock(level),
            "wizard" => GetFullCaster(level, castingStatValue),
            _ => throw new NotFoundException($"Could not find class {className}")
        };
    }
    
    /// <summary>
    /// Gets number of known spells for full casters 
    /// </summary>
    /// <param name="level">the level</param>
    /// <param name="castingStatValue">the value of the casters casting stat (wis, int, or cha)</param>
    /// <returns></returns>
    private static int GetFullCaster(int level, int castingStatValue)
    {
        return level + GetModifier(castingStatValue);
    }
    
    /// <summary>
    /// Gets number of known spells for half casters 
    /// </summary>
    /// <param name="level">the level</param>
    /// <param name="castingStatValue">the value of the casters casting stat (wis, int, or cha)</param>
    /// <returns></returns>
    private int GetHalfCaster(int level, int castingStatValue)
    {
        return level / 2 + GetModifier(castingStatValue);
    }
    
    /// <summary>
    /// Gets known spells for bards
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    /// <exception cref="IllegalStateException"></exception>
    private static int GetBard(int level)
    {
        return level switch
        {
            1 => 4,
            2 => 5,
            3 => 6,
            4 => 7,
            5 => 8,
            6 => 9,
            7 => 10,
            8 => 11,
            9 => 12,
            10 => 14,
            11 => 15,
            12 => 15,
            13 => 16,
            14 => 18,
            15 => 19,
            16 => 19,
            17 => 20,
            18 => 22,
            19 => 22,
            20 => 22,
            _ => throw new IllegalStateException($"Level {level} is invalid"),
        };
    }

    /// <summary>
    /// Gets known spells for rangers
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    /// <exception cref="IllegalStateException"></exception>
    private static int GetRanger(int level)
    {
        return level switch
        {
            1 => 0,
            2 => 2,
            3 => 3,
            4 => 3,
            5 => 4,
            6 => 4,
            7 => 5,
            8 => 5,
            9 => 6,
            10 => 6,
            11 => 7,
            12 => 7,
            13 => 8,
            14 => 8,
            15 => 9,
            16 => 9,
            17 => 10,
            18 => 10,
            19 => 11,
            20 => 11,
            _ => throw new IllegalStateException($"Level {level} is invalid"),
        };
    }
    
    /// <summary>
    /// Gets known spells for sorcerers
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    /// <exception cref="IllegalStateException"></exception>
    private static int GetSorcerer(int level)
    {
        return level switch
        {
            1 => 2,
            2 => 3,
            3 => 4,
            4 => 5,
            5 => 6,
            6 => 7,
            7 => 8,
            8 => 9,
            9 => 10,
            10 => 11,
            11 => 12,
            12 => 12,
            13 => 13,
            14 => 13,
            15 => 14,
            16 => 14,
            17 => 15,
            18 => 15,
            19 => 15,
            20 => 15,
            _ => throw new IllegalStateException($"Level {level} is invalid"),
        };
    }
    
    /// <summary>
    /// Gets known spells for warlocks
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    /// <exception cref="IllegalStateException"></exception>
    private static int GetWarlock(int level)
    {
        return level switch
        {
            1 => 2,
            2 => 3,
            3 => 4,
            4 => 5,
            5 => 6,
            6 => 7,
            7 => 8,
            8 => 9,
            9 => 10,
            10 => 10,
            11 => 11,
            12 => 11,
            13 => 12,
            14 => 12,
            15 => 13,
            16 => 13,
            17 => 14,
            18 => 14,
            19 => 15,
            20 => 15,
            _ => throw new IllegalStateException($"Level {level} is invalid"),
        };
    }
    
    /// <summary>
    /// Gets the modifier for a given stat value
    /// </summary>
    /// <param name="statValue"></param>
    /// <returns></returns>
    private static int GetModifier(int statValue)
    {
        return statValue / 2 - 5;
    }
}