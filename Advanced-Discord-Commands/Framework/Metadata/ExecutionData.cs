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
        private const int MAX_DEPTH = int.MaxValue;

        public ICommand Command { get; private set; }
        public object[] Arguments { get; private set; }
        public CommandMetadata Metadata { get; private set; }
        public bool Executable { get => Command != null && Arguments != null && Metadata != null; }

        public const char HELPCHAR = '?';

        public bool IsDocumentationRequest { get => Arguments.Length == 1 && Arguments[0].ToString () == HELPCHAR.ToString (); }

        public ExecutionData (ICommand _command, object[] _arguments, CommandMetadata _metadata) {
            Command = _command;
            Arguments = _arguments;
            Metadata = _metadata;
        }

        public void SetArguments(object[] newArguments) => Arguments = newArguments;

        public async Task<Result> TryExecute() {

            if (!Executable)
                throw new InvalidExecutionException ("Unable to execute. Execution is not executable, either due to a null command, arguments list, or metadata.");

            if (Metadata.Depth > MAX_DEPTH)
                throw new DepthExceededException ("The maximum command depth was exceeded, try to simplify your command program.");

            if (IsDocumentationRequest)
                return new Result (Command.GetDocumentationEmbed (Metadata), "");

            var commandResult = await Command.TryExecute (Metadata, Arguments);

            if (!(Command is ICommandSet)) { // Passing through a set doesn't really count as executing a command, so they are excluded.
                Callstack.AddToCallstack (Metadata.Message.Id, new Callstack.Item (Command, Arguments.Select (x => x.ToString ()).ToArray (), Metadata.Depth, commandResult.Message, commandResult.Value));
                Metadata.ChangeDepth (1);
            }

            return commandResult;
        }
    }

    public class InvalidExecutionException : Exception {

        public InvalidExecutionException(string message) : base (message) { }

    }

    public class DepthExceededException : Exception {

        public DepthExceededException(string message) : base (message) { }

    }
}
