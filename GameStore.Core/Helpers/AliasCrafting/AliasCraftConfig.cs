using System.Collections.Generic;

namespace GameStore.Core.Helpers.AliasCrafting;

public class AliasCraftConfig
{
    public Dictionary<List<string>, string> ReplacingPairs { get; set; } = new();
}