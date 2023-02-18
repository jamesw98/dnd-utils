using dnd_utils.Exceptions;
using dnd_utils.Models;

namespace dnd_utils.Services;

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
    /// returns a book with fully filled out spells 
    /// </summary>
    /// <param name="book">the spellbook without fully filled out spells</param>
    /// <returns></returns>
    // public Spellbook GetSpellbookToDownloadJson(Spellbook book)
    // {
    //     
    // }
    //
    private int GetFullCaster(int level, int castingStatValue)
    {
        return level + GetModifier(castingStatValue);
    }
    
    private int GetHalfCaster(int level, int castingStatValue)
    {
        return level / 2 + GetModifier(castingStatValue);
    }

    private int GetBard(int level)
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

    private int GetRanger(int level)
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
    
    private int GetSorcerer(int level)
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
    
    private int GetWarlock(int level)
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

    private int GetModifier(int statValue)
    {
        return statValue / 2 - 5;
    }
}