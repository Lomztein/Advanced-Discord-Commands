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
    public static class ExecutorExtensions
    {
        public async static Task<Result> EnterCommand(this IExecutor executor, string fullCommand, IMessage message, ICommandRoot root, ulong? owner, List<ICommand> commandList) {
            CommandMetadata metadata = new CommandMetadata (message, root, executor, owner);

            string[] multiline = executor.ParseMultiline (fullCommand);
            Result result = null;

            while (metadata.ProgramCounter < multiline.Length) {
                int counter = (int)metadata.ProgramCounter;
                string line = multiline[counter];

                object[] arguments = executor.ParseArguments (line).Cast<object> ().ToArray ();

                try {
                    Execution execution = CreateExecution (executor, metadata, line, commandList);
                    result = await executor.Execute (execution);
                } catch (Exception exception) {
                    return new Result (exception);
                }

                metadata.ChangeProgramCounter (1);
            }

            return result;
        }

        public async static Task<Result> Execute(this IExecutor executor, CommandMetadata metadata, string fullCommand, List<ICommand> commandList) {
            string[] args = executor.ParseArguments (fullCommand);
            return await Execute (executor, metadata, fullCommand, args, commandList);
        }

        public static async Task<Result> Execute(this IExecutor executor, CommandMetadata metadata, string fullCommand, object[] arguments, List<ICommand> commandList) {
            Execution execution = executor.CreateExecution (metadata, fullCommand, arguments, commandList);
            return await executor.Execute (execution);
        }

        public static Execution CreateExecution(this IExecutor executor, CommandMetadata metadata, string fullCommand, List<ICommand> commandList) {
            object[] arguments = executor.ParseArguments (fullCommand);
            return CreateExecution (executor, metadata, fullCommand, arguments, commandList);
        }

        public static Execution CreateExecution(this IExecutor executor, CommandMetadata metadata, string fullCommand, object[] arguments, List<ICommand> commandList) {
            ICommand cmd = executor.FindCommand (fullCommand, commandList, metadata.Owner);
            return new Execution (cmd, arguments, metadata);
        }
    }
}
