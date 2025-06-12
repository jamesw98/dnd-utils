using System.Net.Http.Json;
using Compendium.Exceptions;
using Compendium.Models;

namespace Compendium.Services;

public class SearchService
{
    private HttpClient _client;

    private List<DndItem> _items;
    private List<Spell> _spells;

    public SearchService(HttpClient client)
    {
        _client = client;
        _items = new();
        _spells = new();
    }

    /// <summary>
    /// Gets a specific, fully detailed, spell
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<Spell> GetSpell(string name)
    {
        var fixedSpellName = name.CleanQuery();
        return await GetFromJson<Spell>($"spells/{fixedSpellName}.json");
    }

    /// <summary>
    /// Gets a specific, full detailed, item
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<DndItem> GetItem(string name)
    {
        var fixedItemName = name.CleanQuery();

        return await GetFromJson<DndItem>($"items/{fixedItemName}.json");
    }

    /// <summary>
    /// Gets a detaild item for an id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<DndItem> GetItemForId(int id)
    {
        if (!_items.Any())
        {
            _items = await GetAllItems();
        }
        
        var itemNoContent = _items.FirstOrDefault(x => x.Id == id);

        if (itemNoContent == null)
        {
            throw new NotFoundException();
        }

        return await GetItem(itemNoContent.Name);
    }
    
    /// <summary>
    /// Gets a detailed spell for an id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Spell> GetSpellForId(int id)
    {
        if (!_spells.Any())
        {
            _spells = await GetAllSpells();
        }
        var spellNoContent = _spells.FirstOrDefault(x => x.SpellId == id);

        if (spellNoContent == null)
        {
            throw new NotFoundException();
        }

        return await GetSpell(spellNoContent.Name);
    }

    /// <summary>
    /// Gets all spells with no descriptions
    /// </summary>
    /// <returns></returns>
    public async Task<List<Spell>> GetAllSpells()
    {
        return await GetFromJson<List<Spell>>("spells/spells-no-content.json");
    }
    
    /// <summary>
    /// Gets spell related options
    /// </summary>
    /// <returns></returns>
    public async Task<SpellOptions> GetSpellOptions()
    {
        return await GetFromJson<SpellOptions>("spells/options.json");
    }

    /// <summary>
    /// Gets the list of spells
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetSpellList()
    {
        return await GetFromJson<List<string>>("spells/spell-list.json");
    }
    
    /// <summary>
    /// Gets all item data with no descriptions
    /// </summary>
    /// <returns></returns>
    public async Task<List<DndItem>> GetAllItems()
    {
        var items = await GetFromJson<List<DndItem>>("items/items-no-content.json");
        return items.DistinctBy(x => x.Name).ToList();
    }
    
    /// <summary>
    /// Gets item option data
    /// </summary>
    /// <returns></returns>
    public async Task<ItemOptions> GetItemOptions()
    {
        return await GetFromJson<ItemOptions>("items/options.json");
    }

    /// <summary>
    /// Gets spell slot info for a given class
    /// </summary>
    /// <param name="className">The name of the class to get info for</param>
    /// <returns>Spell slot info for a class</returns>
    public async Task<Dictionary<int, Dictionary<int, int>>> GetSpellSlotsForClass(string className)
    {
        return await GetFromJson<Dictionary<int, Dictionary<int, int>>>($"classinfo/spellslots/{className.ToLower()}.json");
    }
    
    /// <summary>
    /// Gets all classes
    /// </summary>
    /// <returns>A list of strings representing all classes in D&D</returns>
    public async Task<List<string>> GetClassList()
    {
        return await GetFromJson<List<string>>("classinfo/all-classes.json");
    }
    
    /// <summary>
    /// Gets values from a json file
    /// </summary>
    /// <param name="path">The path to the json file</param>
    /// <typeparam name="T">The type to parse data into</typeparam>
    /// <returns>The parsed data</returns>
    private async Task<T> GetFromJson<T>(string path)
    {
        T result;
        try
        {
            result = await _client.GetFromJsonAsync<T>(path) 
                     ?? throw new NotFoundException($"Could not find {path}");
        }
        catch (Exception e)
        {
            throw new IllegalStateException($"Could not parse {path}");
        }

        return result;
    }
}