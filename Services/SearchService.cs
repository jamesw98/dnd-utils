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

    public async Task<Item> GetItem(string name)
    {
        var fixedItemName = name.ToLower()
            .Replace(": ", "-")
            .Replace(' ', '-')
            .Replace("/", "-");

        return await GetFromJson<Item>($"items/{fixedItemName}.json");
    }

    public async Task<Item> GetItemForId(int id)
    {
        var itemsNoContext = await GetAllItems();
        var itemNoContent = itemsNoContext.FirstOrDefault(x => x.Id == id);

        if (itemNoContent == null)
            throw new NotFoundException();

        return await GetItem(itemNoContent.Name);
    }

    public async Task<Spell> GetSpellForId(int id)
    {
        var spellsNoContent = await GetAllSpells();
        var spellNoContent = spellsNoContent.FirstOrDefault(x => x.SpellId == id);

        if (spellNoContent == null)
            throw new NotFoundException();

        return await GetSpell(spellNoContent.Name);
    }

    public async Task<List<Spell>> GetAllSpells()
    {
        return await GetFromJson<List<Spell>>("spells/spells-no-content.json");
    }

    public async Task<SpellOptions> GetSpellOptions()
    {
        return await GetFromJson<SpellOptions>("spells/options.json");
    }

    public async Task<List<string>> GetSpellList()
    {
        return await GetFromJson<List<string>>("spells/spell-list.json");
    }

    public async Task<List<Item>> GetAllItems()
    {
        var items = await GetFromJson<List<Item>>("items/items-no-content.json");
        return items.DistinctBy(x => x.Name).ToList();
    }

    public async Task<ItemOptions> GetItemOptions()
    {
        return await GetFromJson<ItemOptions>("items/options.json");
    }

    public async Task<Dictionary<int, Dictionary<int, int>>> GetSpellSlotsForClass(string className)
    {
        return await GetFromJson<Dictionary<int, Dictionary<int, int>>>($"classinfo/spellslots/{className.ToLower()}.json");
    }

    public async Task<List<string>> GetClassList()
    {
        return await GetFromJson<List<string>>("classinfo/all-classes.json");
    }

    private async Task<T> GetFromJson<T>(string path)
    {
        T result;
        try
        {
            result = await _client.GetFromJsonAsync<T>(path);
        }
        catch (Exception e)
        {
            throw new IllegalStateException($"Could not parse {path}");
        }

        if (result == null)
            throw new NotFoundException($"Could not find {path}");

        return result;
    }
}