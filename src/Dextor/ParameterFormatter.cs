using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dextor
{
    public class ParameterFormatter : IParameterFormatter
    {
        public string Format(string formatString, IDictionary<string, byte[]> values)
        {
            var pattern = new Regex(@"(?<!\$)\$([a-zA-Z0-9_]+)");

            var matches = pattern.Matches(formatString);

            var replacements =
                matches.Cast<Match>()
                    .Select(match => new {match, key = match.Groups[1].Value})
                    .Where(@t => values.ContainsKey(@t.key))
                    .Select(@t => new Tuple<int, int, byte[]>(@t.match.Index, @t.match.Length, values[@t.key]))
                    .Reverse();

            var result = new StringBuilder(formatString);

            foreach (var replacement in replacements)
            {
                var index = replacement.Item1;
                var length = replacement.Item2;
                var value = replacement.Item3;
                result.Remove(index, length);
                result.Insert(index, Encoding.UTF8.GetString(value));
            }

            result.Replace("$$", "$");

            return result.ToString();
        }
    }
}