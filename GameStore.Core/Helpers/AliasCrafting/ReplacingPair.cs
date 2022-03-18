namespace GameStore.Core.Helpers.AliasCrafting;

public class ReplacingPair
{
    public ReplacingPair(string[] oldValues, string newValue)
    {
        OldValues = oldValues;
        NewValue = newValue;
    }

    public ReplacingPair()
    {
    }

    public string[] OldValues { get; set; }
    public string NewValue { get; set; }
}