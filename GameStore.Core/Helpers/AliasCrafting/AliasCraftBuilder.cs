using System.Linq;

namespace GameStore.Core.Helpers.AliasCrafting;

internal class AliasCraftBuilder
{
    public AliasCraftConfig Config { get; set; } = new();
    
    public AliasCraft Build()
    {
        return new AliasCraft(Config);
    }

    public ReplacingPairBuilder Values(params string[] values)
    {
        return new ReplacingPairBuilder(this, values);
    }
}