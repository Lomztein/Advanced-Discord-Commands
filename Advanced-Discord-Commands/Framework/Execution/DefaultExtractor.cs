using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Execution;
using Lomztein.AdvDiscordCommands.Framework.Execution.TokenExtractors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution
{
    public class DefaultExtractor : IExtractor
    {
        private readonly ITokenExtractor[] _extractors =
        {
            new DelimitingTokenExtractor (','),
            new NumberTextTokenExtractor (),
            new DelimitingTokenExtractor (' '),
            new NoTokenExtractor (),
        };

        public Arguments ExtractArguments(string input)
        {
            string arguments = ExtractArgumentPart(input);
            object[][] extractedSets = new object[_extractors.Length][];

            for (int i = 0; i < _extractors.Length; i++)
            {
                ITokenExtractor extractor = _extractors[i];
                extractedSets[i] = extractor.ExtractTokens(arguments);
            }

            return new Arguments(extractedSets);
        }

        private static string ExtractArgumentPart(string fullCommand)
        {

            if (!string.IsNullOrEmpty(fullCommand))
            {
                int spaceIndex = fullCommand.IndexOfAny(CharExtensions.WhitespaceChars);
                if (spaceIndex == -1)
                    return null;
                else
                    return fullCommand.Substring(spaceIndex).Trim ();
            }

            return null;

        }
    }
}
