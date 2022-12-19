using dnd_utils.Models;

namespace dnd_utils.Services;

public class MonsterService
{
    private DiceService _dice;

    public MonsterService(DiceService dice)
    {
        _dice = dice;
    }
    
    public List<Monster> GenerateMonsters(string name, int number, int passive, int initMod, int ac, int hitDieType, 
        int hitDieNum, int hitDieMod, int fixedHp=-1, int fixedInit=-1)
    {
        List<Monster> result = new();

        for (int i = 0; i < number; i++)
        {
            var init = fixedInit == -1 ? _dice.Roll(1, 20, initMod) : fixedInit;
            var hp = fixedHp == -1 ? _dice.Roll(hitDieNum, hitDieType, hitDieMod) : fixedHp;
            
            result.Add(new Monster
            {
                Initiative = init,
                HitPoints = hp,
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
            r.Name = $"{name} {count++}";   

        return result;
    }
}