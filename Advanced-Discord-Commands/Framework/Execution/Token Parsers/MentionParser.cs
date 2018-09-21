using Discord;
using Lomztein.AdvDiscordCommands.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenParsers
{
    public class MentionParser : SorroundedArgumentParser {

        public override char Start => '<';
        public override char End => '>';

        public override Task<ParseResult> TryParseInsidesAsync(string prefix, string insides, string suffix, CommandMetadata metadata) {
            IMentionable mentionable = $"<{insides}>".ExtractMentionable (metadata.Message.GetGuild ());
            return ParseResult.CreateTask (mentionable);
        }
    }
}
