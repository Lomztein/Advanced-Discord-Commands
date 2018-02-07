using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.ExampleCommands {

    public class CallstackCommand : Command {

        public CallstackCommand() {
            command = "callstack";
            catagory = Category.Advanced;
            shortHelp = "View command chain callstack.";
            AddOverload (typeof (string), "View the latest executed callstack.");
            AddOverload (typeof (string), "View the callstack for the command given by the command message ID");
        }

        public Task<Result> Execute(CommandMetadata e) {
            return Execute (e, callstacks [ 0 ].chainID);
        }

        public Task<Result> Execute(CommandMetadata e, ulong id) {
            Callstack callstack = callstacks.Find (x => x.chainID == id);
            if (callstack != null) {
                string message = "```";
                foreach (Callstack.Item item in callstack.items) {
                    string arguments = " ";
                    for (int i = 0; i < item.arguments.Count; i++) {
                        arguments += item.arguments [ i ] + (i == item.arguments.Count - 1 ? "" : "; ");
                    }

                    message += StringExtensions.UniformStrings (item.command.helpPrefix + item.command.command + arguments, item.returnObj == null ? "null" : item.returnObj.ToString (), " -> ", 50) + "\n";
                }
                message += "```";
                return TaskResult (message, message);
            }
            return TaskResult (null, "Error - No callstack found for given ID.");
        }
    }
}

        // TODO: Add a better way to send command as arguments without executing them.
            /*public async Task<Result> Execute(SocketUserMessage e, string cmd) {
                string newCmd; // Well thats not confusing.
                List<string> args;

                if (TryIsolateWrappedCommand (cmd, out newCmd, out args)) {
                    Program.FoundCommandResult result = await Program.FindAndExecuteCommand (e, newCmd, args, Program.commands, 0, false);
                    return Execute (e, e.Id).Result;
                }
                return new Result (null, "Error - Input must be a command wrapped in parenthesis.");
            }*/
