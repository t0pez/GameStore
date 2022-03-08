using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.Core.Helpers.AliasCrafting
{
    public class AliasCraftConfig
    {
        public Dictionary<string, string> ReplacingPairs { get; set; } = new();
        public StringCaseChanges SourceCaseChanges { get; set; }
    }
}
