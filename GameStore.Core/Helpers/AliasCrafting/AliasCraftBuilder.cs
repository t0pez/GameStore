using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.Core.Helpers.AliasCrafting
{
    internal class AliasCraftBuilder
    {
        private readonly Dictionary<char, char> _replacingPairs = new();
        private readonly List<char> _symbolsToDelete = new();

        public AliasCraft Build()
        {
            return new AliasCraft(_replacingPairs, _symbolsToDelete);
        }

        public AliasCraftBuilder AddPairToReplace(char oldSymbol, char newSymbol)
        {
            _replacingPairs.Add(oldSymbol, newSymbol);

            return this;
        }
        
        public AliasCraftBuilder AddSymbolsToRemove(params char[] symbols)
        {
            _symbolsToDelete.AddRange(symbols);

            return this;
        }
    }
}
