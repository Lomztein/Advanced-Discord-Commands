using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public static class ExecutionFactory
    {
        public static Execution Create (this ICommandRoot root, CommandMetadata metadata, string fullCommand, List<ICommand> commandList) {
            object[] arguments = root.Extractor.ExtractArguments (fullCommand);
            return CreateExecution (executor, metadata, fullCommand, arguments, commandList);
        }
    }
}
