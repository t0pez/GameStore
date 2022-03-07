using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            var builder = new StringBuilder();

            AppendWithDeletionFilter(builder, source.ToLower());
            ReplaceSymbols(builder);

            return builder.ToString();
        }

        private void AppendWithDeletionFilter(StringBuilder builder, string source)
        {
            foreach (var symbol in source)
                if (_symbolsToDelete.Contains(symbol) == false)
                    builder.Append(symbol);
        }

        private void ReplaceSymbols(StringBuilder builder)
        {
            foreach (var pair in _replacingPairs)
                builder.Replace(pair.Key, pair.Value);
        }
    }
}
