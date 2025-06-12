namespace Compendium.Services;

public static class Methods
{
    /// <summary>
    /// Cleans up a query string
    /// </summary>
    /// <param name="query">The string to clean up</param>
    /// <returns>A squeaky clean query string</returns>
    public static string CleanQuery(this string query)
    {
        return query.ToLower()
            .Replace(": ", "-")
            .Replace(' ', '-')
            .Replace("/", "-");
    }
}