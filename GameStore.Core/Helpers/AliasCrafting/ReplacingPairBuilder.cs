namespace GameStore.Core.Helpers.AliasCrafting;

internal class ReplacingPairBuilder
{
    private readonly AliasCraftBuilder _builder;
    private readonly ReplacingPair _pair = new();

    public ReplacingPairBuilder(AliasCraftBuilder builder, string[] oldValues)
    {
        _builder = builder;
        _pair.OldValues = oldValues;
    }

    public AliasCraftBuilder ReplaceWith(string newValue)
    {
        _pair.NewValue = newValue;
        AddNewPairToConfig();
        
        return _builder;
    }

    public AliasCraftBuilder Delete()
    {
        return ReplaceWith(string.Empty);
    }

    private void AddNewPairToConfig()
    {
        foreach (var oldValue in _pair.OldValues) 
            _builder.Config.ReplacingPairs.Add(_pair);
    }
}