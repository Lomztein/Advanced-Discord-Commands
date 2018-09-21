using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution
{
    public class DefaultSplitter : ISplitter
    {
        public const char lineSeperator = ';';

        // TODO: Find a way for these and those in DefaultExtractor to be the same.
        public const char forcedStringStart = '[';
        public const char forcedStringEnd = ']';

        public const char nestedCommandStart = '(';
        public const char nestedCommandEnd = ')';

        public string[] SplitMultiline (string input) {

            List<string> result = new List<string> ();

            string arg;
            int quatationBalance = 0;
            int lastCut = 0;

            for (int i = 0; i < input.Length; i++) {
                char cur = input[i];

                if (cur == forcedStringStart)
                    quatationBalance++;
                if (cur == forcedStringEnd)
                    quatationBalance--;

                if (cur == lineSeperator && quatationBalance == 0) {
                    arg = input.Substring (lastCut, i - lastCut);
                    result.Add (arg);
                    lastCut = i + 1;
                }
            }

            if (input.Length > 0) {
                arg = input.Substring (lastCut);
                if (arg.Length > 0)
                    result.Add (arg);
            }

            for (int i = 0; i < result.Count; i++) {
                result[i] = result[i].Trim (CharExtensions.WhitespaceChars);
            }

            return result.ToArray ();
        }
    }
}
