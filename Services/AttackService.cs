using dnd_utils.Enums;
using dnd_utils.Exceptions;
using dnd_utils.Models;

namespace dnd_utils.Services;

public class AttackService
{
    private DiceService _dice;

    public AttackService(DiceService dice)
    {
        _dice = dice;
    }

    public List<Monster> DealDamage(List<Monster> creatures, List<AttackResult> attacks)
    {
        foreach (var a in attacks)
        {
            foreach (var c in creatures.Where(x => x.ArmorClass <= a.AttackDetail.Total))
            {
                var damage = a.DamageDetail.Total;
                
                if (c.Immunities.Contains(a.Type))
                    damage = 0;
                else if (c.Vulnerabilities.Contains(a.Type))
                    damage *= 2;
                else if (c.Resistances.Contains(a.Type))
                    damage /= 2;

                c.HitPoints -= damage;
                
                if (c.HitPoints < 0)
                    c.HitPoints = 0;
            }
        }

        return creatures;
    }
    
    /// <summary>
    /// gets attack results for the given inputs
    /// </summary>
    /// <param name="numberOfAttacks"></param>
    /// <param name="toHitModifier"></param>
    /// <param name="acToBeat"></param>
    /// <param name="damageDieNum"></param>
    /// <param name="damageDieType"></param>
    /// <param name="damageModifier"></param>
    /// <param name="hideMisses"></param>
    /// <param name="hasAdvantage"></param>
    /// <param name="hasDisadvantage"></param>
    /// <param name="autoCrit">if true, every attack will be a crit</param>
    /// <returns></returns>
    /// <exception cref="IllegalStateException"></exception>
    public List<AttackResult> CalculateAttackResults(
        int numberOfAttacks,
        int toHitModifier,
        int acToBeat,
        int damageDieNum,
        int damageDieType,
        int damageModifier,
        bool hideMisses,
        bool hasAdvantage,
        bool hasDisadvantage,
        bool autoCrit
    )
    {
        // check for the impossible state
        if (hasAdvantage && hasDisadvantage)
            throw new IllegalStateException("Cannot have both advantage and disadvantage!");
        
        List<AttackResult> results = new();

        for (var i = 0; i < numberOfAttacks; i++)
        {
            var currentAttack = new AttackResult
            {
                AcToBeat = acToBeat
            };

            RollDetails usedToHitRoll;
            RollDetails? unusedToHitRoll = null; // nullable since this var is only used during (dis)advantage
            if (hasAdvantage || hasDisadvantage)
            {
                var state = hasAdvantage ? AdvantageState.Advantage : AdvantageState.Disadvantage;
                (usedToHitRoll, unusedToHitRoll) = _dice.RollDetailsNonStandard(1, 20, toHitModifier, state);
            }
            else
                usedToHitRoll = _dice.RollDetailed(1, 20, toHitModifier);

            currentAttack.Hit = usedToHitRoll.Total >= acToBeat;
            currentAttack.AttackDetail = usedToHitRoll;
            currentAttack.UnusedAttackDetail = unusedToHitRoll;
            currentAttack.Crit = autoCrit || usedToHitRoll.Rolls.Contains(20);

            var diceNum = currentAttack.Crit ? damageDieNum * 2 : damageDieNum;
            var damage = _dice.RollDetailed(diceNum, damageDieType, damageModifier);

            currentAttack.DamageDetail = damage;
            currentAttack.Damange = damage.Total;
            
            results.Add(currentAttack);
        }

        results = results.OrderByDescending(x => x.AttackDetail.Total).ToList();
        if (hideMisses)
            results = results.Where(x => x.Hit).ToList();
        
        return results;
    }
}