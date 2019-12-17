using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenExtractors
{
    public abstract class IgnoreInsideBaseTokenExtractor : ITokenExtractor
    {
        public const char forcedStringStart = '[';
        public const char forcedStringEnd = ']';

        public const char nestedCommandStart = '(';
        public const char nestedCommandEnd = ')';

        private readonly char[] _ignoreInsideStarters =
        {
            '(', '{', '['
        };

        private readonly char[] _ignoreInsideEnders =
        {
            ')', '}', ']'
        };

        private int[] _balances;

        private bool Imbalanced() => _balances.Any(x => x != 0);

        public virtual object[] ExtractTokens (string input)
        {
            _balances = new int[_ignoreInsideStarters.Length];

            if (string.IsNullOrEmpty(input))
                return Array.Empty<object>();

            List<string> arguments = new List<string>();
            int lastCut = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char cur = input[i];
                string extracted = InnerExtract(input, i, ref lastCut, cur, Imbalanced());
                if (extracted != null)
                {
                    arguments.Add(extracted);
                }

                int startIndex = Array.IndexOf(_ignoreInsideStarters, cur);
                if (startIndex != -1)
                {
                    _balances[startIndex]++;
                }

                int endIndex = Array.IndexOf(_ignoreInsideEnders, cur);
                if (endIndex != -1)
                {
                    _balances[endIndex]--;
                }
            }

            if (input.Length > 0)
            {
                arguments.Add(input.Substring(lastCut));
            }

            for (int i = 0; i < arguments.Count; i++)
            {
                arguments[i] = arguments[i].Trim();
            }

            return arguments.ToArray();
        }

        public abstract string InnerExtract(string fullInput, int currentIndex, ref int lastCut, char currentChar, bool isImbalanced);
    }
}
