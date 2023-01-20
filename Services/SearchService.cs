using System.Net.Http.Json;
using dnd_utils.Exceptions;
using dnd_utils.Models;

namespace dnd_utils.Services;

public class SearchService
{
    private HttpClient _client;
    
    public SearchService(HttpClient client)
    {
        _client = client;
    } 
    
    public async Task<Spell> GetSpell(string name)
    {
        var fixedSpellName = name.ToLower()
            .Replace(": ", "-")
            .Replace(' ', '-')
            .Replace("/", "-");
        
        return await GetFromJson<Spell>($"spells/{fixedSpellName}.json");
    }

    public async Task<List<Spell>> GetAllSpells()
    {
        List<Spell> spells = new(); 
        var spellList = await GetSpellList();
        
        foreach (var spell in spellList)
            spells.Add(await GetSpell(spell));

        return spells;
    }

    public async Task<Options> GetSpellOptions()
    {
        return await GetFromJson<Options>("spells/options.json");
    }

    public async Task<List<string>> GetSpellList()
    {
        return await GetFromJson<List<string>>("spells/spell-list.json");
    }

    private async Task<T> GetFromJson<T>(string path)
    {
        var result = await _client.GetFromJsonAsync<T>(path);
        
        if (result == null)
            throw new NotFoundException($"Could not find {path}");
        
        return result;
    }
}