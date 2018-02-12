using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.ExampleCommands
{
    public class MiscCommandSet : CommandSet
    {
        public MiscCommandSet() {
            command = "misc";
            shortHelp = "Miscellaneous advanced commands.";
            catagory = Category.Advanced;

            commandsInSet = new List<Command> {
                new Log (),
            };
        }

        public class Log : Command {
            public Log() {
                command = "log";
                shortHelp = "Log something.";
                requiredPermissions.Add (Discord.GuildPermission.Administrator);
            }

            [Overload (typeof (void), "Logs something to the bot logs.")]
            public Task<Result> Execute(CommandMetadata e, string text) {
                Logging.Log (Logging.LogType.BOT, text);
                return TaskResult (null, "Log has been logged to the log.");
            }

            [Overload (typeof (void), "Logs something to the bot logs of a specific type of log.")]
            public Task<Result> Execute(CommandMetadata e, int type, string text) {
                Logging.LogType logType = (Logging.LogType)type;
                Logging.Log (logType, text);
                return TaskResult (null, "Log has been logged to the log as log type " + logType.ToString () + ".");
            }
        }
    }
}
