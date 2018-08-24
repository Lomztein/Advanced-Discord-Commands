using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Autodocumentation;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.ExampleCommands {

    public class HelpCommand : Command {

        public HelpCommand() {
            Name = "help";
            Description = "Show command list.";
            Category = StandardCategories.Utility;

            Aliases = new string[] { "clist" };

            AvailableInDM = true;
        }

        [Overload (typeof (ICommandSet), "Reveals a full list of all commands.")]
        public Task<Result> Execute(CommandMetadata data) {
            return Execute (data, data.Root);
        }

        [Overload (typeof (string), "Reveals a list of commands in a given command array.")]
        public Task<Result> Execute(CommandMetadata data, params Command [ ] commands) {
            // I mean, it works, right?
            Embed result = CommandListAutodocumentation.ListCommands (data, "given", "", commands);
            Task<Result> r = TaskResult (result, null);
            return r;
        }

        [Overload (typeof (string), "Reveals a list of commands in a given command set interface.")]
        public Task<Result> Execute(CommandMetadata e, ICommandSet set) {
            Embed result = set.ListCommands (e);
            return TaskResult (result, null);
        }
    }
}
