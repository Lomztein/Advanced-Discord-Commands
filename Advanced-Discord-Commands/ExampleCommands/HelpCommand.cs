using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.ExampleCommands {

    public class HelpCommand : Command {

        public HelpCommand() {
            command = "help";
            shortHelp = "Show command list.";
            catagory = Category.Utility;

            availableInDM = true;
        }

        [Overload (typeof (ICommandSet), "Reveals a full list of all commands.")]
        public Task<Result> Execute(CommandMetadata data) {
            Result result = Execute (data, data.root.commands.ToArray ()).Result;
            result.value = data.root;
            return Task.FromResult (result);
        }

        [Overload (typeof (string), "Reveals a list of commands in a given command array.")]
        public Task<Result> Execute(CommandMetadata data, params Command [ ] commands) {
            // I mean, it works, right?
            string result = Command.ListCommands (data, commands.First ().command, commands);
            Task<Result> r = TaskResult (result, result);
            return r;
        }

        [Overload (typeof (string), "Reveals a list of commands in a given command set.")]
        public Task<Result> Execute(CommandMetadata e, CommandSet set) {
            return TaskResult (CommandSet.ListCommands (e, set), CommandSet.ListCommands (e, set));
        }
    }
}
