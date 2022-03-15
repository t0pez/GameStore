namespace GameStore.Core.Helpers.AliasCrafting;

internal class AliasConfigBuilder
{
    private readonly AliasCraftBuilder _builder;
    private readonly string[] _oldValues;

    public AliasConfigBuilder(AliasCraftBuilder builder, string[] oldValues)
    {
        _builder = builder;
        _oldValues = oldValues;
    }

    public AliasCraftBuilder ReplaceWith(string newValue)
    {
        foreach (var oldValue in _oldValues)
        {
            _builder.Config.ReplacingPairs.Add(oldValue, newValue);
        }
        
        return _builder;
    }

    public AliasCraftBuilder Delete()
    {
        foreach (var oldValue in _oldValues)
        {
            _builder.Config.ReplacingPairs.Add(oldValue, string.Empty);
        }
        
        return _builder;
    }
}