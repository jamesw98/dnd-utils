using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Auth0.Core.Exceptions;
using Blazored.SessionStorage;
using Compendium.Blazor.Models;

namespace Compendium.Blazor.Utils;

/// <summary>
/// Class to call Azure functions and Scryfall.
/// </summary>
public class ApiUtil
{
    /// <summary>
    /// JSON serialization options to use throughout this util.
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Lets us get a JWT for the user.
    /// </summary>
    private readonly TokenUtil _tokenUtil;
    
    /// <summary>
    /// Lets us access session storage.
    /// </summary>
    private readonly ISessionStorageService _storageService;
    
    /// <summary>
    /// An HTTP client.
    /// </summary>
    private readonly HttpClient _http;

    /// <summary>
    /// URL for this environment's Azure functions.
    /// </summary>
    private readonly string _functionsUri; 

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="tokenUtil">DI token service.</param>
    /// <param name="storageService">DI storage service.</param>
    /// <param name="http">DI HTTP client.</param>
    /// <param name="config">Configuration</param>
    public ApiUtil(TokenUtil tokenUtil, ISessionStorageService storageService, HttpClient http, IConfiguration config)
    {
        _tokenUtil = tokenUtil;
        _storageService = storageService;
        _http = http;

        // Attempts to get the function URI from the config.  
        var uri = config.GetSection("FunctionsUri").Value
            ?? throw new DataException("Could not find FunctionUri configuration section.");
        _functionsUri = uri;
    }

    public async Task<Guid> CreateItem(string itemName, string fullText)
    {
        if (itemName.Trim() == string.Empty)
        {
            throw new ArgumentException("Name cannot be empty or whitespace");
        }

        if (fullText.Trim() == string.Empty)
        {
            throw new ArgumentException("Item description cannot be empty or whitespace.");
        }

        var guid = Guid.NewGuid();
        await CallApiVoidWithParam<CustomItemRequest>(HttpMethod.Post, "Item", new CustomItemRequest
        {
            CustomItem = new CustomItem {
                ItemId = guid,
                Name = itemName,
                FullText = fullText
            }
        });
        return guid;
    }

    public async Task<List<CustomItem>> GetItemsForUser()
    {
        return await CallApi<List<CustomItem>>(HttpMethod.Get, "Item");
    }

    public async Task<CustomItem> GetItem(Guid id)
    {
        return await CallApi<CustomItem>(HttpMethod.Get, $"Item/{id}");
    }

    // /// <summary>
    // /// Gets all decks.
    // /// </summary>
    // /// <returns>A list of all decks.</returns>
    // public async Task<List<Deck>> GetDecks()
    // {
    //     var decks = await CallApi<List<Deck>>(HttpMethod.Get, "Deck");
    //     return decks;
    // }
    //
    // /// <summary>
    // /// Create a deck.
    // /// </summary>
    // /// <param name="newDeck">The deck to create.</param>
    // public async Task CreateDeck(Deck newDeck)
    // {
    //     await CallApiVoidWithParam(HttpMethod.Post, "Deck", new CreateDeckRequest
    //     {
    //         NewDeck = newDeck
    //     });
    // }
    //
    // /// <summary>
    // /// Gets a single deck.
    // /// </summary>
    // /// <param name="deckId">The ID of the deck.</param>
    // /// <returns>A deck.</returns>
    // public async Task<Deck> GetDeck(Guid deckId)
    // {
    //     var deck = await CallApiWithParam<Deck, Guid>(HttpMethod.Get, "Deck", deckId);
    //     return deck;
    // }
    //
    // /// <summary>
    // /// Gets all decks a user has created.
    // /// </summary>
    // /// <returns>A list of decks.</returns>
    // public async Task<List<Deck>> GetDecksForUser()
    // {
    //     var decks = await CallApi<List<Deck>>(HttpMethod.Get, "Deck/Mine");
    //     return decks;
    // }
    //
    // /// <summary>
    // /// Searches our database for decks that match the query.
    // /// </summary>
    // /// <param name="query">The query from the user.</param>
    // /// <returns>A list of decks that match.</returns>
    // public async Task<List<Deck>> SearchDecks(string query)
    // {
    //     var decks = await CallApi<List<Deck>>(HttpMethod.Get, $"Search?query={query}");
    //     return decks;
    // }
    //
    // /// <summary>
    // /// Creates a match for a user.
    // /// </summary>
    // /// <param name="match">The match to create.</param>
    // public async Task CreateMatch(Match match)
    // {
    //     await CallApiVoidWithParam<CreateMatchRequest>(HttpMethod.Post, "Match", new CreateMatchRequest
    //     {
    //         NewMatch = match
    //     });
    // }
    //
    // /// <summary>
    // /// Get all matches.
    // /// </summary>
    // /// <returns>A list of matches.</returns>
    // public async Task<List<Match>> GetMatches()
    // {
    //     var response = await CallApi<List<Match>>(HttpMethod.Get, "Match");
    //     return response;
    // }
    
    #region PRIVATE
    
    private async Task<TReturn> CallApiWithParam<TReturn,TParam>(HttpMethod method, string uri, TParam param)
    {
        var response = await CallApiBaseWithParam(method, uri, param);
        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted && response.StatusCode != HttpStatusCode.NoContent)
        {
            throw new DataException($"API returned {response.StatusCode}");
        }
        
        var responseStr = await response.Content.ReadAsStringAsync();
        var responseObj = JsonSerializer.Deserialize<TReturn>(responseStr, _jsonOptions);

        if (responseObj is null)
        {
            throw new Exception();
        }
        
        return responseObj;
    }

    private async Task CallApiVoidWithParam<TParam>(HttpMethod method, string uri, TParam parameters)
    {
        var response = await CallApiBaseWithParam(method, uri, parameters);
    }

    private async Task<HttpResponseMessage> CallApiBaseWithParam<TParam>(HttpMethod method, string uri, TParam parameters)
    {
        if (_tokenUtil.Jwt == string.Empty)
        {
            await _tokenUtil.GetJwt(_storageService);
        }

        var req = CreateBaseRequestWithParam(method, uri, parameters);
        var res = await _http.SendAsync(req);
        return res;
    }
    
    private HttpRequestMessage CreateBaseRequestWithParam<TParam>(HttpMethod method, string uri, TParam parameters, bool authenticated=true)
    {
        var request = new HttpRequestMessage(method, $"{_functionsUri}{uri}");
        request.Headers.TryAddWithoutValidation("accept", "*/*");

        if (authenticated)
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_tokenUtil.Jwt}");
        }

        request.Content = new StringContent(JsonSerializer.Serialize(parameters));
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        
        return request;
    }
    
    private async Task<TReturn> CallApi<TReturn>(HttpMethod method, string uri)
    {
        var response = await CallApiBase(method, uri);
        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted && response.StatusCode != HttpStatusCode.NoContent)
        {
            throw new DataException($"API returned {response.StatusCode}");
        }
        
        var responseStr = await response.Content.ReadAsStringAsync();
        var responseObj = JsonSerializer.Deserialize<TReturn>(responseStr, _jsonOptions);

        if (responseObj is null)
        {
            throw new Exception();
        }
        
        return responseObj;
    }

    private async Task CallApiVoid(HttpMethod method, string uri)
    {
        var response = await CallApiBase(method, uri);
    }
    
    private async Task<HttpResponseMessage> CallApiBase(HttpMethod method, string uri)
    {
        if (_tokenUtil.Jwt == string.Empty)
        {
            await _tokenUtil.GetJwt(_storageService);
        }

        var req = CreateBaseRequest(method, uri);
        var res = await _http.SendAsync(req);
        return res;
    }
    
    /// <summary>
    /// Create the base of our HTTP requests.
    /// </summary>
    /// <param name="method">The HTTP method to use.</param>
    /// <param name="uri">The URI to call.</param>
    /// <param name="authenticated">Whether this call is authenticated.</param>
    /// <returns>A HttpRequestMessage.</returns>
    private HttpRequestMessage CreateBaseRequest(HttpMethod method, string uri, bool authenticated=true)
    {
        var request = new HttpRequestMessage(method, $"{_functionsUri}{uri}");
        request.Headers.TryAddWithoutValidation("accept", "*/*");

        if (authenticated && _tokenUtil.Jwt != string.Empty)
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_tokenUtil.Jwt}");
        }

        return request;
    }
    
    #endregion
}