using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenParsers {
    public abstract class StringArgumentParser : ITokenParser {

        public Task<ParseResult> TryParse(object input, CommandMetadata metadata) {
            return TryParseString (input.ToString (), metadata);
        }

        public abstract Task<ParseResult> TryParseString(string input, CommandMetadata metadata);
    }
}
