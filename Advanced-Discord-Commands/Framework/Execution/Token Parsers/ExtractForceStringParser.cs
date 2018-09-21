using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenParsers {

    public class ExtractForceStringParser : SorroundedArgumentParser {

        public override char Start => '[';
        public override char End => ']';

        public override Task<ParseResult> TryParseInsidesAsync(string prefix, string insides, string suffix, CommandMetadata metadata) {
            return ParseResult.CreateTask (insides);
        }
    }
}
