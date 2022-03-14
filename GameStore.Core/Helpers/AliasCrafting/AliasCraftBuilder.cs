using System.Linq;

namespace GameStore.Core.Helpers.AliasCrafting;

internal class AliasCraftBuilder
{
    private readonly AliasCraftConfig _config = new();

    public AliasCraft Build()
    {
        return new AliasCraft(_config);
    }

    public AliasCraftBuilder AddPairToReplace(string oldValue, string newValue)
    {
        _config.ReplacingPairs.Add(oldValue, newValue);

        return this;
    }
    
    public AliasCraftBuilder AddSymbolsToRemove(params string[] symbols)
    {
        var pairs = symbols.Select(s => new { Key = s, Value = string.Empty});

        foreach (var pair in pairs)
            _config.ReplacingPairs.Add(pair.Key, pair.Value);

        return this;
    }
}