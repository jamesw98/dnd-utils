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
        int hitDieNum, int hitDieMod)
    {
        List<Monster> result = new();

        for (int i = 0; i < number; i++)
        {
            var init = _dice.Roll(1, 20, initMod);
            var hp = _dice.Roll(hitDieNum, hitDieType, hitDieMod);
            
            result.Add(new Monster
            {
                Initiative = init,
                HitPoints = hp,
                PassivePerception = passive,
                ArmorClass = ac
            });
        }

        result = result.OrderByDescending(x => x.Initiative).ToList();
        
        var count = 1;
        foreach (var r in result)
            r.Name = name + count++; 

        return result;
    }
}