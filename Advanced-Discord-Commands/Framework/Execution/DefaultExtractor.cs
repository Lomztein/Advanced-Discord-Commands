using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution
{
    public class DefaultExtractor : IExtractor {

        public const char argSeperator = ',';

        public const char forcedStringStart = '[';
        public const char forcedStringEnd = ']';

        public const char nestedCommandStart = '(';
        public const char nestedCommandEnd = ')';

        public string[] ExtractArguments(string fullCommand) {

            string toSplit = fullCommand.ExtractArgumentPart ();

            if (string.IsNullOrEmpty (toSplit))
                return new string[0];

            List<string> arguments = new List<string> ();
            string arg;

            int quatationBalance = 0;

            int balance = 0;
            int lastCut = 0;

            for (int i = 0; i < toSplit.Length; i++) {
                char cur = toSplit[i];

                if (cur == forcedStringStart)
                    quatationBalance++;
                if (cur == forcedStringEnd)
                    quatationBalance--;

                if (quatationBalance == 0) {
                    switch (cur) {
                        case argSeperator:
                            if (balance == 0) {
                                arg = toSplit.Substring (lastCut, i - lastCut);
                                arguments.Add (arg);
                                lastCut = i + 1;
                            }
                            break;

                        case nestedCommandStart:
                            balance++;
                            break;

                        case nestedCommandEnd:
                            balance--;
                            break;
                    }
                }
            }

            if (toSplit.Length > 0) {
                arguments.Add (toSplit.Substring (lastCut));
            }

            for (int i = 0; i < arguments.Count; i++) {
                arguments[i] = arguments[i].Trim (CharExtensions.WhitespaceChars);
            }

            return arguments.ToArray ();
        }
    }
}
