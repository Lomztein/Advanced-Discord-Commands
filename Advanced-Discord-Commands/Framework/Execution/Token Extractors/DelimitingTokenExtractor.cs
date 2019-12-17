using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenExtractors
{
    public class DelimitingTokenExtractor : IgnoreInsideBaseTokenExtractor
    {
        private char _delimiter;

        public DelimitingTokenExtractor (char delimiter)
        {
            _delimiter = delimiter;
        }

        public override string InnerExtract(string fullInput, int currentIndex, ref int lastCut, char currentChar, bool isImbalanced)
        {
            if (!isImbalanced)
            {
                if (currentChar == _delimiter)
                {
                    string arg = fullInput.Substring(lastCut, currentIndex - lastCut);
                    lastCut = currentIndex + 1;
                    return arg;
                }
            }
            return null;
        }
    }
}
