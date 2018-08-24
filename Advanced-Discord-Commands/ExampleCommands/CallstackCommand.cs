using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.ExampleCommands {

    public class CallstackCommand : Command {

        public CallstackCommand() {
            Name = "callstack";
            Category = StandardCategories.Advanced;
            Description = "View command callstack.";
        }

        [Overload (typeof (string), "View the latest executed callstack.")]
        public Task<Result> Execute(CommandMetadata e) {
            return Execute (e, Callstack.callstacks [ 0 ].chainID);
        }

        [Overload (typeof (string), "View the callstack for the command given by the command message ID")]
        public Task<Result> Execute(CommandMetadata e, ulong id) {
            Callstack callstack = Callstack.callstacks.Find (x => x.chainID == id);
            if (callstack != null) {
                string message = "```";
                foreach (Callstack.Item item in callstack.items) {
                    message += item.ToString (e.Owner) + "\n";
                }
                message += "```";
                return TaskResult (message, message);
            }
            return TaskResult (null, "Error - No callstack found for given ID.");
        }
    }
}