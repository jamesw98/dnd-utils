using dnd_utils.Enums;
using dnd_utils.Exceptions;
using dnd_utils.Models;

namespace dnd_utils.Services;

public class AttackService
{
    private readonly DiceService _dice;

    public AttackService(DiceService dice)
    {
        _dice = dice;
    }
    
    /// <summary>
    /// Re-rolls an attack
    /// </summary>
    /// <param name="ar">An attack that's already been rolled</param>
    /// <param name="creaturesToHit">The creatures to hit with this attack</param>
    /// <returns>A newly re-rolled attack</returns>
    public AttackResult ReRoll(AttackResult ar, List<Monster> creaturesToHit)
    {
        // these will never be null in this case                
        var newAttackDetails = _dice.RollDetailed(ar.AttackDetail.Num ?? -1, ar.AttackDetail.Die ?? -1,
            ar.AttackDetail.Mod ?? -1);
        
        var newDamageDetails = _dice.RollDetailed(ar.DamageDetail.Num ?? -1, ar.DamageDetail.Die ?? -1,
            ar.DamageDetail.Mod ?? -1);

        ar.AttackDetail = newAttackDetails;
        ar.DamageDetail = newDamageDetails;
        ar.Damange = newDamageDetails.Total;
        ar.CreaturesTargeted = creaturesToHit;

        return ar;
    }

    /// <summary>
    /// used for dealing damage in the combat manager
    /// </summary>
    /// <param name="creatures">all the creatures in the manager</param>
    /// <param name="creaturesToHit">the creatures affected by this attack</param>
    /// <param name="attacks">the attacks to be dealt</param>
    /// <param name="savingThrow">whether or not this is a saving throw</param>
    /// <returns>an updated list of creatures with reduced hp (if the attacks hit)</returns>
    public List<Monster> DealDamage(List<Monster> creatures, IEnumerable<Monster> creaturesToHit,
        List<AttackResult> attacks, bool savingThrow)
    {
        foreach (var a in attacks)
        {
            var creaturesHit =
                savingThrow ? creatures : creatures.Where(x => x.ArmorClass <= a.AttackDetail.Total).ToList();
            a.CreaturesTargeted = creaturesToHit.ToList();

            foreach (var c in creaturesHit)
            {
                if (!creaturesToHit.Select(x => x.Name).Contains(c.Name))
                    continue;

                a.CreaturesHit.Add(c);

                var damage = a.DamageDetail.Total;

                if (c.Immunities.Contains(a.Type))
                {
                    damage = 0;
                }
                else if (c.Vulnerabilities.Contains(a.Type))
                {
                    damage *= 2;
                }
                else if (c.Resistances.Contains(a.Type))
                {
                    damage /= 2;
                }

                if (savingThrow && c.PassedSavingThrow)
                {
                    damage /= 2;
                }

                c.HitPoints -= damage;

                if (c.HitPoints < 0)
                {
                    c.HitPoints = 0;
                }
            }
        }

        return creatures;
    }

    /// <summary>
    /// gets attack results for the given inputs
    ///
    /// TODO - this is a lot of params, should probably just make a class, but that's a task for another day
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
    /// <param name="fixedDamage">if not -1, do not roll for damage</param>
    /// <returns></returns>
    /// <exception cref="IllegalStateException"></exception>
    public List<AttackResult> CalculateAttackResults(
        string attackName,
        int numberOfAttacks,
        int toHitModifier,
        int acToBeat,
        int damageDieNum,
        int damageDieType,
        int damageModifier,
        DamageType type,
        bool hideMisses,
        bool hasAdvantage,
        bool hasDisadvantage,
        bool autoCrit,
        int fixedDamage = -1
    )
    {
        // check for the impossible state
        if (hasAdvantage && hasDisadvantage)
        {
            throw new IllegalStateException("Cannot have both advantage and disadvantage!");
        }

        List<AttackResult> results = new();

        for (var i = 0; i < numberOfAttacks; i++)
        {
            var currentAttack = new AttackResult
            {
                Name = attackName,
                AcToBeat = acToBeat,
                Type = type
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
            var damage = fixedDamage == -1
                ? _dice.RollDetailed(diceNum, damageDieType, damageModifier)
                : new RollDetails
                {
                    Total = fixedDamage,
                    Fixed = true
                };

            currentAttack.DamageDetail = damage;
            currentAttack.Damange = damage.Total;

            results.Add(currentAttack);
        }

        results = results.OrderByDescending(x => x.AttackDetail.Total).ToList();
        if (hideMisses)
        {
            results = results.Where(x => x.Hit).ToList();
        }

        return results;
    }
}