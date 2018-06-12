using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.ExampleCommands {

    public class HelpCommand : Command {

        public HelpCommand() {
            Name = "help";
            Description = "Show command list.";
            Category = StandardCategories.Utility;

            availableInDM = true;
        }

        [Overload (typeof (ICommandSet), "Reveals a full list of all commands.")]
        public Task<Result> Execute(CommandMetadata data) {
            string result = ListCommands (data, data.root.Name, data.root.commands.ToArray ());
            return TaskResult (data.root, result);
        }

        [Overload (typeof (string), "Reveals a list of commands in a given command array.")]
        public Task<Result> Execute(CommandMetadata data, params Command [ ] commands) {
            // I mean, it works, right?
            string result = ListCommands (data, "given", commands);
            Task<Result> r = TaskResult (result, result);
            return r;
        }

        [Overload (typeof (string), "Reveals a list of commands in a given command set.")]
        public Task<Result> Execute(CommandMetadata e, CommandSet set) {
            return TaskResult (ListCommands (e, set), ListCommands (e, set));
        }
         
        [Overload (typeof (string), "Reveals a list of commands in a given command set interface.")]
        public Task<Result> Execute(CommandMetadata e, ICommandSet set) {
            string result = ListCommands (e, "given", set.GetCommands ().ToArray ());
            return TaskResult (e, result);
        }
    }
}
