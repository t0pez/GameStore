using System.Text.RegularExpressions;

namespace GameStore.Core.Helpers.GameKeyGeneration;

public static class GameKeyGenerator
{
    public static string GenerateGameKey(string name)
    {
        return Regex.Replace(name.Trim().ToLower(), @"\s{2,}", " ").Replace(' ', '-');
    }
}