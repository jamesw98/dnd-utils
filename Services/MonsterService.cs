using dnd_utils.Models;

namespace dnd_utils.Services;

public class MonsterService
{
    public List<Monster> GenerateMonsters(string name, int number, int passive, int initMod, int ac, int hitDieType, 
        int hitDieNum, int hitDieMod)
    {
        List<Monster> result = new();

        for (int i = 0; i < number; i++)
        {
            var init = Roll(1, 20, initMod);
            var hp = Roll(hitDieNum, hitDieType, hitDieMod);
            
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

    private int Roll(int num, int type, int mod)
    {
        Random rand = new();
        var result = 0;

        for (var i = 0; i < num; i++)
            result += rand.Next(type);

        return result + mod;
    }
}