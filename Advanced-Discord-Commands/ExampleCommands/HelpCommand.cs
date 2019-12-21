using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Autodocumentation;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using Lomztein.AdvDiscordCommands.Extensions;

namespace Lomztein.AdvDiscordCommands.ExampleCommands {

    public class HelpCommand : Command {

        public HelpCommand() {
            Name = "help";
            Description = "Show command list.";
            Category = StandardCategories.Utility;

            Aliases = new string[] { "clist", "list", "commands" };

            AvailableInDM = true;
        }

        [Overload (typeof (ICommandSet), "Reveals a full list of all commands.")]
        public Task<Result> Execute(CommandMetadata data) {
            return Execute (data, data.Root);
        }

        [Overload (typeof (Embed), "Reveals a list of commands in a given command array.")]
        public Task<Result> Execute(CommandMetadata data, params ICommand[] commands) {
            // I mean, it works, right?
            Embed result = CommandListAutodocumentation.ListCommands (data, "given", "", commands);
            Task<Result> r = TaskResult (result, null);
            return r;
        }

        [Overload (typeof (Embed), "Reveals a list of commands in a given command set interface.")]
        public Task<Result> Execute(CommandMetadata e, ICommandSet set) {
            //Embed result = set.ListCommands (e);
            Embed result = CommandListAutodocumentation.ListCommands (e, "given", "",set.GetCommands ().ToArray ());
            return TaskResult (result, null);
        }

        [Overload (typeof (Embed), "Get help for a given command. Really just appends `?` and throws whatever you give it back in, so it can execute commands if you give them arguments as well. Fixing this 'feature' isn't really possible, at the current time.")]
        public Task<Result> Execute (CommandMetadata e, string command)
        {
            string request;
            if (command.Length > 0 && command[0].IsCommandTrigger (e.Owner, e.Searcher))
            {
                request = command;
            }
            else
            {
                request = e.Root.GetChildPrefix(e.Owner) + command;
            }

            ExecutionData execution = e.Root.CreateExecution (request, e);
            var result = e.Executor.Execute(execution);
            return result;
        }
    }
}
