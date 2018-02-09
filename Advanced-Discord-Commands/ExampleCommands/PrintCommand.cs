using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lomztein.AdvDiscordCommands.Framework;

namespace Lomztein.AdvDiscordCommands.ExampleCommands
{
    public class PrintCommand : Command {
        public PrintCommand() {
            command = "print";
            shortHelp = "Prints stuff.";
            catagory = Category.Utility;

            AddOverload (typeof (string), "Prints whatever is put into it, regardless of position in program.");
        }

        public Task<Result> Execute(CommandMetadata data, object obj) {
            if (obj != null) {
                data.message.Channel.SendMessageAsync (obj.ToString ());
                return TaskResult (obj.ToString (), "");
            } else {
                data.message.Channel.SendMessageAsync ("null");
                return TaskResult ("null", "");
            }
        }
    }
}
