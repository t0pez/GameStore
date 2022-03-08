using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameStore.Core.Helpers.AliasCrafting
{
    public class AliasCraft : IAliasCraft
    {
        private readonly AliasCraftConfig _config;

        public AliasCraft(AliasCraftConfig config)
        {
            _config = config;
        }

        public string CreateAlias(string source)
        {
            var builder = new StringBuilder();

            SetSourceString(builder, source);
            ReplaceSymbols(builder);

            return builder.ToString();
        }

        private void SetSourceString(StringBuilder builder, string source)
        {
            var resultString = source;

            if (_config.SourceCaseChanges == StringCaseChanges.Lower)
            {
                resultString = resultString.ToLower();
            }
            else if (_config.SourceCaseChanges == StringCaseChanges.Upper)
            {
                resultString = resultString.ToUpper();
            }

            builder.Append(resultString);
        }

        private void ReplaceSymbols(StringBuilder builder)
        {
            foreach (var pair in _config.ReplacingPairs)
                builder.Replace(pair.Key, pair.Value);
        }
    }
}
