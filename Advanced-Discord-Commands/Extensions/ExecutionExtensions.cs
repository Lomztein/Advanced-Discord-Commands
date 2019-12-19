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

        public async static Task<Result> EnterCommand (this ICommandRoot root, string fullCommand, IMessage message) => await root.EnterCommand(fullCommand, message, message.GetGuild ().Id);

        public static ExecutionData CreateExecution(this ICommandRoot root, string fullCommand, CommandMetadata metadata) {
            return CreateExecution (root, fullCommand, metadata, root.GetCommands ());
        }

        public static ExecutionData CreateExecution(this ICommandRoot root, string fullCommand, CommandMetadata metadata,List<ICommand> commandList) {
            Arguments arguments = metadata.Extractor.ExtractArguments (fullCommand);
            return CreateExecution (root, fullCommand, metadata, arguments, commandList);
        }

        public static ExecutionData CreateExecution(this ICommandRoot root, string fullCommand, CommandMetadata metadata, Arguments arguments, List<ICommand> commandList) {
            ICommand cmd = metadata.Searcher.Search (fullCommand, commandList, metadata.Owner);
            return new ExecutionData(cmd, arguments, metadata);
        }
    }
}
