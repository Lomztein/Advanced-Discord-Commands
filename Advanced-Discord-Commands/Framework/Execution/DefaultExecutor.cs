using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Lomztein.AdvDiscordCommands.Framework.Execution.TokenParsers;

namespace Lomztein.AdvDiscordCommands.Framework.Execution
{
    public class DefaultExecutor : IExecutor
    {
        public Func<ulong, char> Trigger { get; set; } = (x => '!');
        public Func<ulong, char> HiddenTrigger { get; set; } = (x => '/');

        public List<ITokenParser> Parsers { get; private set; } = new List<ITokenParser> {
            new NestedCommandParser (),
            new GetVariableParser (),
            new MentionParser (),
            new ExtractForceStringParser (),
        };

        public async Task<object[]> ParseChainElements(CommandMetadata data, object[] arguments) {
            List<object> converted = new List<object> ();

            foreach (object obj in arguments) {
                dynamic result = obj;
                string stringObj = obj.ToString ();

                foreach (ITokenParser parser in Parsers) {
                    ParseResult parseResult = await parser.TryParse (obj, data);
                    if (parseResult.Success)
                        result = parseResult.Result;
                }

                converted.Add (result);
            }

            return converted.ToArray ();
        }

        public async Task<Result> Execute(ExecutionData execution) {
            execution.SetArguments (await ParseChainElements (execution.Metadata, execution.Arguments));
            var result = await execution.TryExecute ();
            return result;
        }
    }
}
