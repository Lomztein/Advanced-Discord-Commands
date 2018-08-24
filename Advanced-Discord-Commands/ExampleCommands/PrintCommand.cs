using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Categories;

namespace Lomztein.AdvDiscordCommands.ExampleCommands
{
    public class PrintCommand : Command {
        public PrintCommand() {
            Name = "print";
            Description = "Prints stuff.";
            Category = StandardCategories.Advanced;
        }

        [Overload (typeof (string), "Prints whatever is put into it, regardless of position in program.")]
        public Task<Result> Execute(CommandMetadata data, object obj) {
            if (obj != null) {
                data.Message.Channel.SendMessageAsync (obj.ToString ());
                return TaskResult (obj.ToString (), "");
            } else {
                data.Message.Channel.SendMessageAsync ("null");
                return TaskResult ("null", "");
            }
        }
    }
}
