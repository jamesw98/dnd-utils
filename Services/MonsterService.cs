using dnd_utils.Models;

namespace dnd_utils.Services;

public class MonsterService
{
    private readonly DiceService _dice;

    public MonsterService(DiceService dice)
    {
        _dice = dice;
    }
    
    /// <summary>
    /// Generates monsters based on params
    ///
    /// TODO - this is a lot of params, should probably just make a class, but that's a task for another day
    /// </summary>
    /// <param name="name"></param>
    /// <param name="number"></param>
    /// <param name="passive"></param>
    /// <param name="initMod"></param>
    /// <param name="ac"></param>
    /// <param name="hitDieType"></param>
    /// <param name="hitDieNum"></param>
    /// <param name="hitDieMod"></param>
    /// <param name="fixedHp"></param>
    /// <param name="fixedInit"></param>
    /// <returns></returns>
    public List<Monster> GenerateMonsters(string name, int number, int passive, int initMod, int ac, int hitDieType, 
        int hitDieNum, int hitDieMod, int fixedHp=-1, int fixedInit=-1)
    {
        List<Monster> result = new();

        for (var i = 0; i < number; i++)
        {
            var init = fixedInit == -1 ? _dice.Roll(1, 20, initMod) : fixedInit;
            var hp = fixedHp == -1 ? _dice.Roll(hitDieNum, hitDieType, hitDieMod) : fixedHp;
            
            result.Add(new Monster
            {
                Name = "temp",
                Initiative = init,
                HitPoints = hp,
                MaxHitPoints = hp,
                PassivePerception = passive,
                ArmorClass = ac
            });
        }

        if (number == 1)
        {
            result.First().Name = name;
            return result;
        }
            
        var count = 1;
        foreach (var r in result)
        {
            r.Name = $"{name} {count++}";   
        }

        return result;
    }
}