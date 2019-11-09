using Discord;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Extensions
{
    public static class ExecutionExtensions {

        public async static Task<Result> EnterCommand(this ICommandRoot root, IMessage message) => await EnterCommand (root, message.Content, message);

        public async static Task<Result> EnterCommand (this ICommandRoot root, string fullCommand, IMessage message) => await EnterCommand(root, fullCommand, message, message.GetGuild ().Id);

        public async static Task<Result> EnterCommand(this ICommandRoot root, string fullCommand, IMessage message, ulong? owner) {

            CommandMetadata metadata = new CommandMetadata (message, root, owner);

            string[] multiline = root.Splitter.SplitMultiline (fullCommand);
            Result result = null;
                
            try {
                while (metadata.ProgramCounter < multiline.Length) {
                    int counter = (int)metadata.ProgramCounter;
                    string line = multiline[counter];

                    ICommand command = root.Searcher.Search (line, root.GetCommands (), owner);
                    object[] arguments = root.Extractor.ExtractArguments (line).Cast<object> ().ToArray ();

                    ExecutionData execution = new ExecutionData (command, arguments, metadata);

                    if (execution.Executable)
                        result = await root.Executor.Execute (execution);

                    metadata.ChangeProgramCounter (1);
                }
            } catch (Exception exception) {
                metadata.AbortProgram ();
                result = new Result (exception);
            }

            CommandVariables.Clear (metadata.Message.Id);
            return result;
        }

        public static ExecutionData CreateExecution(this ICommandRoot root, string fullCommand, CommandMetadata metadata) {
            return CreateExecution (root, fullCommand, metadata, root.GetCommands ());
        }

        public static ExecutionData CreateExecution(this ICommandRoot root, string fullCommand, CommandMetadata metadata,List<ICommand> commandList) {
            object[] arguments = root.Extractor.ExtractArguments (fullCommand);
            return CreateExecution (root, fullCommand, metadata, arguments, commandList);
        }

        public static ExecutionData CreateExecution(this ICommandRoot root, string fullCommand, CommandMetadata metadata, object[] arguments, List<ICommand> commandList) {
            ICommand cmd = root.Searcher.Search (fullCommand, commandList, metadata.Owner);
            return new ExecutionData(cmd, arguments, metadata);
        }
    }
}
