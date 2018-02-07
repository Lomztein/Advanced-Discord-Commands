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

            AddOverload (typeof (ICommandSet), "Reveals a full list of all commands.");
            AddOverload (typeof (string), "Reveals a list of commands in a given command array.");
            AddOverload (typeof (string), "Reveals a list of commands in a given command set.");
        }

        public Task<Result> Execute(CommandMetadata data) {
            Result result = Execute (data, data.root.commands.ToArray ()).Result;
            result.value = data.root;
            return Task.FromResult (result);
        }

        public Task<Result> Execute(CommandMetadata data, params Command [ ] commands) {
            var catagories = commands.Where (x => x.AllowExecution (data) == "").GroupBy (x => x.catagory);
            string result = "```";

            foreach (var catagory in catagories) {
                result += catagory.ElementAt (0).catagory.ToString () + " Commands\n";
                foreach (var item in catagory) {
                    result += item.Format () + "\n";
                }
                result += "\n";
            }
            result += "```";
            bool anyFound = result != "``````";

            // I mean, it works, right?
            Task<Result> r = anyFound ? TaskResult (result, result) : TaskResult ("", "This command set contains no available commands.");
            return r;
        }

        public Task<Result> Execute(SocketUserMessage e, CommandSet set) {
            return TaskResult (set.GetHelp (e), set.GetHelp (e));
        }
    }
}
