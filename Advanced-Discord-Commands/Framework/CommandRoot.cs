using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Extensions;
using System.Linq;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public class CommandRoot {

        public List<Command> commands = new List<Command> ();

        /// <summary>
        /// This is the entrance method for the command system, call this. You can have multiple roots, for instance for different servers, and it should work just fine, since commands get their information from the SocketMessage object.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<Command.Result> EnterCommand (SocketUserMessage e) {

            string message = e.Content;
            if (message [ 0 ].IsCommandTrigger ()) {

                FoundCommandResult finalResult = await FindAndExecuteCommand (new CommandMetadata (e, this), message, commands, 0);
                CommandVariables.Clear (e.Id);

                return finalResult.result;
            }

            return null;
        }

        public static async Task<FoundCommandResult> FindAndExecuteCommand(CommandMetadata metadata, string fullCommand, List<Command> commandList, int depth) {
            string cmd = "";
            List<object> arguments = ConstructArguments (fullCommand.Substring (1), out cmd);

            Console.WriteLine (fullCommand + ", " + cmd);

            return await FindAndExecuteCommand (metadata, cmd, arguments, commandList, depth);
        }

        public static async Task<FoundCommandResult> FindAndExecuteCommand(CommandMetadata metadata, string commandName, List<object> arguments, List<Command> commandList, int depth) {
            for (int i = 0; i < commandList.Count; i++) {

                if (commandList[ i ].command == commandName) {
                    if (arguments.Count > 0 && arguments [ 0 ].ToString () == "?") {

                        Command command = commandList [ i ];
                        if (command is CommandSet) {
                            return new FoundCommandResult (new Command.Result (null, command.GetHelp (metadata)), command);
                        } else {
                            return new FoundCommandResult (new Command.Result (command.GetHelpEmbed (metadata, true), ""), command);
                        }

                    } else {
                        FoundCommandResult result = new FoundCommandResult (await commandList [ i ].TryExecute (metadata, depth, arguments.ToArray ()), commandList [ i ]);
                        if (result != null) {
                            return result;
                        }
                    }
                }
            }

            return null;
        }

        public class FoundCommandResult {
            public Command.Result result;
            public Command command;

            public FoundCommandResult(Command.Result _result, Command _command) {
                result = _result;
                command = _command;
            }
        }

        public static List<object> ConstructArguments(string fullCommand, out string command) {
            string toSplit = fullCommand.Substring (fullCommand.IndexOf (' ') + 1);
            List<object> arguments = new List<object> ();
            command = "";

            if (fullCommand.LastIndexOf (' ') != -1) {
                command = fullCommand.Substring (0, fullCommand.Substring (1).IndexOf (' ') + 1);
                arguments.AddRange (SplitArgs (toSplit));
            } else {
                command = fullCommand;
            }

            return arguments;
        }

        public static string [ ] SplitArgs(string toSplit) {
            List<string> arguments = new List<string> ();
            string arg;
            int balance = 0;
            int lastCut = 0;

            for (int i = 0; i < toSplit.Length; i++) {
                char cur = toSplit [ i ];

                switch (toSplit [ i ]) {
                    case ';':
                        if (balance == 0) {
                            arg = toSplit.Substring (lastCut, i - lastCut);
                            arguments.Add (arg);
                            lastCut = i + 1;
                        }
                        break;

                    case '(':
                        balance++;
                        break;

                    case ')':
                        balance--;
                        break;
                }
            }

            if (toSplit.Length > 0) {
                arguments.Add (toSplit.Substring (lastCut));
            }

            for (int i = 0; i < arguments.Count; i++) {
                arguments [ i ] = arguments [ i ].Trim (' ');
            }

            return arguments.ToArray ();
        }
    }
}
