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
            var builder = new StringBuilder(source.ToLower());

            ReplaceSymbols(builder);

            return builder.ToString();
        }

        private void ReplaceSymbols(StringBuilder builder)
        {
            foreach (var pair in _config.ReplacingPairs)
                builder.Replace(pair.Key, pair.Value);
        }
    }
}
