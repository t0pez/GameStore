using System.Collections.Generic;
using System.Linq;

namespace GameStore.Core.Helpers.AliasCrafting
{
    public class AliasCraft : IAliasCraft
    {
        private readonly Dictionary<char, char> _replacingPairs;
        private readonly List<char> _symbolsToDelete;

        public AliasCraft(Dictionary<char, char> replacingPairs, List<char> symbolsToDelete)
        {
            _replacingPairs = replacingPairs;
            _symbolsToDelete = symbolsToDelete;
        }

        public string CreateAlias(string source)
        {
            var result = ""; // TODO: Change for StringBuilder

            foreach (var symbol in source.ToLower()) // TODO: extract methods
                if (_symbolsToDelete.Contains(symbol) == false)
                    result += symbol;

            foreach (var pair in _replacingPairs)
                result = result.Replace(pair.Key, pair.Value);

            return result;
        }
    }
}
