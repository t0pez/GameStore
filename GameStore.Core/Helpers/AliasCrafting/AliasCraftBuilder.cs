using System.Collections.Generic;
using System.Linq;

namespace GameStore.Core.Helpers.AliasCrafting
{
    internal class AliasCraftBuilder
    {
        private readonly AliasCraftConfig _config = new();

        public AliasCraft Build()
        {
            return new AliasCraft(_config);
        }

        public AliasCraftBuilder AddPairToReplace(char oldSymbol, char newSymbol)
        {
            _config.ReplacingPairs.Add($"{oldSymbol}", $"{newSymbol}");

            return this;
        }

        public AliasCraftBuilder AddSymbolsToRemove(params char[] symbols)
        {
            var pairs = symbols.Select(s => new { Key = $"{s}", Value = ""});

            foreach (var pair in pairs)
                _config.ReplacingPairs.Add(pair.Key, pair.Value);

            return this;
        }

        public AliasCraftBuilder SetCaseChanges(StringCaseChanges stringCase)
        {
            _config.SourceCaseChanges = stringCase;

            return this;
        }
    }
}
