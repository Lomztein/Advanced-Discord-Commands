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

        public async Task<Arguments> ParseChainElements(CommandMetadata data, Arguments arguments) {
            List<List<object>> converted = new List<List<object>> ();

            // TODO: Experiment with looping over the arguments untill all parsers return null.
            foreach (object[] set in arguments)
            {
                List<object> setConverted = new List<object>();
                foreach (object obj in set)
                {
                    object result = obj;
                    foreach (ITokenParser parser in Parsers)
                    {
                        ParseResult parseResult = await parser.TryParse(obj, data);
                        if (parseResult.Success)
                            result = parseResult.Result;
                    }

                    setConverted.Add(result);
                }
                converted.Add(setConverted);
            }

            return new Arguments (converted);
        }

        public async Task<Result> Execute(ExecutionData execution) {
            execution.SetArguments (await ParseChainElements (execution.Metadata, execution.Arguments));
            var result = await execution.Execute ();
            return result;
        }
    }
}
