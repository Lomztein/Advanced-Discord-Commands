using Lomztein.AdvDiscordCommands.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenParsers {
    public class NestedCommandParser : SorroundedArgumentParser {

        public override char Start => '(';
        public override char End => ')';

        public override async Task<ParseResult> TryParseInsidesAsync(string prefix, string insides, string suffix, CommandMetadata metadata) {
            if (insides.Length > 0 || insides[0].IsCommandTrigger (metadata.Owner, metadata.Searcher)) {

                Arguments chainArguments = metadata.Extractor.ExtractArguments (insides);
                ExecutionData chainExecution = metadata.Root.CreateExecution (insides, metadata, chainArguments, metadata.Root.GetCommands ());
                Result chainExecutionResult = await metadata.Executor.Execute (chainExecution);
                return new ParseResult (chainExecutionResult.Value);

            } else
                return new ParseResult (null);
        }
    }
}
