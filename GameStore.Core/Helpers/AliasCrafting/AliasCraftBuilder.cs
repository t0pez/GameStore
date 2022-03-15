using System.Linq;

namespace GameStore.Core.Helpers.AliasCrafting;

internal class AliasCraftBuilder
{
    public AliasCraftConfig Config { get; set; } = new();
    
    public AliasCraft Build()
    {
        return new AliasCraft(Config);
    }

    public AliasConfigBuilder Values(params string[] values)
    {
        return new AliasConfigBuilder(this, values);
    }
}