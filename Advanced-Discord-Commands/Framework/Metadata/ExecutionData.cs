using Lomztein.AdvDiscordCommands.Autodocumentation;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Execution;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public class ExecutionData
    {
        private const int MAX_DEPTH = 1024*8;

        public ICommand Command { get; private set; }
        public Arguments Arguments { get; private set; }
        public CommandMetadata Metadata { get; private set; }
        public bool Executable { get => Command != null && Arguments != null && Metadata != null; }
        public bool IsDocumentationRequest { get; }

        public ExecutionData (ICommand _command, Arguments _arguments, CommandMetadata _metadata) {
            Command = _command;
            Arguments = _arguments;
            Metadata = _metadata;
        }

        public ExecutionData(ICommand _command, Arguments _arguments, CommandMetadata _metadata, bool isDocRequest) : this(_command, _arguments, _metadata)
        {
            IsDocumentationRequest = isDocRequest;
        }

        public void SetArguments(Arguments newArguments) => Arguments = newArguments;

        public async Task<Result> Execute()
        {

            if (Metadata.Complexity > MAX_DEPTH)
                throw new DepthExceededException("The maximum command complexity was exceeded, please try to simplify your command program.");

            if (IsDocumentationRequest)
                return new Result(Command.GetDocumentationEmbed(Metadata), "");

            if (Executable)
            {
                Metadata.ChangeComplexity(Arguments.Sum(x => x.Length));

                var commandResult = await Command.TryExecute(Metadata, Arguments);

                if (!(Command is ICommandSet))
                { // Passing through a set doesn't really count as executing a command, so they are excluded.
                    Callstack.AddToCallstack(Metadata.Message.Id, new Callstack.Item(Command, Arguments.Last().Select(x => x.ToString()).ToArray(), Metadata.Complexity, commandResult.Message, commandResult.Value));
                }

                return commandResult;
            }
            else
            {
                return new Result(null, string.Empty, true);
            }
        }
    }

    public class InvalidExecutionException : Exception {

        public InvalidExecutionException(string message) : base (message) { }

    }

    public class DepthExceededException : Exception {

        public DepthExceededException(string message) : base (message) { }

    }
}
