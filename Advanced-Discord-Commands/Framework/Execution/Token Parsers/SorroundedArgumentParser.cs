using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenParsers {
    public abstract class SorroundedArgumentParser : StringArgumentParser
    {
        public abstract char Start { get; }
        public abstract char End { get; }

        public override Task<ParseResult> TryParseString(string input, CommandMetadata metadata) {

            int startIndex = input.IndexOf (Start);
            int endIndex = input.LastIndexOf (End);

            if (startIndex == -1 || endIndex == -1 || startIndex != 0)
                return ParseResult.CreateTask (null);

            string prefix = input.Substring (0, startIndex);
            string suffix = input.Substring (endIndex + 1);

            string insides = input.Substring (startIndex + 1, endIndex - startIndex - 1);

            return TryParseInsidesAsync (prefix, insides, suffix, metadata);
        }

        public abstract Task<ParseResult> TryParseInsidesAsync(string prefix, string insides, string suffix, CommandMetadata metadata);
    }
}
