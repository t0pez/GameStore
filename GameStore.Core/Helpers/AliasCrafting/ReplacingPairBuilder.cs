using System.Collections.Generic;

namespace GameStore.Core.Helpers.AliasCrafting;

public class ReplacingPairBuilder
{
    private readonly AliasCraftBuilder _builder;
    private readonly List<string> _oldValues;
    private string _newValue;

    public ReplacingPairBuilder(AliasCraftBuilder builder, List<string> oldValues)
    {
        _builder = builder;
        _oldValues = oldValues;
    }

    public AliasCraftBuilder ReplaceWith(string newValue)
    {
        _newValue = newValue;
        AddNewPairToConfig();
        
        return _builder;
    }

    public AliasCraftBuilder Delete()
    {
        return ReplaceWith(string.Empty);
    }

    private void AddNewPairToConfig()
    {
        _builder.Config.ReplacingPairs.Add(_oldValues, _newValue);
    }
}