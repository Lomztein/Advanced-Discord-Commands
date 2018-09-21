using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenParsers
    {
    public class GetVariableParser : SorroundedArgumentParser {

        public override char Start => '{';
        public override char End => '}';

        public virtual char IndexerStart { get => '['; }
        public virtual char IndexerEnd { get => ']'; }

        public override Task<ParseResult> TryParseInsidesAsync(string prefix, string insides, string suffix, CommandMetadata metadata) {

            dynamic result = null;

            result = CommandVariables.Get (metadata.Message.Id, insides);
            if (result == null) // If no local variable, fallback to personal.
                result = CommandVariables.Get (metadata.Message.Author.Id, insides);
            if (result == null) { // If no personal variable, fallback to server.
                SocketGuild guild = metadata.Message.GetGuild ();
                if (guild != null)
                    result = CommandVariables.Get (guild.Id, insides);
            }

            suffix.Trim ();

            if (suffix.Length > 0 && suffix[0] == IndexerStart) {
                int squareEnd = suffix.LastIndexOf (IndexerEnd);
                if (squareEnd != -1) {

                    string number = suffix.Substring (1, squareEnd - 1);
                    if (int.TryParse (number, out int index)) {
                        result = result[index];
                    }
                }
            }

            return ParseResult.CreateTask (result);

        }
    }
}
