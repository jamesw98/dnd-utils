using dnd_utils.Models;

namespace dnd_utils.Services;

public class SpellService
{
    public Task<Spell> GetSpell(string name)
    {
        var fixedSpellName = name.ToLower().Replace(' ', '-').Replace('/', '-');

        var spell = await Client.GetFromJsonAsync<Spell>($"spells/{fixedSpellName}.json");

        if (spell == null)
            return;

        _spell = spell;    
    }
}