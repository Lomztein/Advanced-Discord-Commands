using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework {
    public class CommandRoot : ICommandSet {

        public const char argSeperator = ' ';
        public const char lineSeperator = ';';
        public int maxCommandLength = short.MaxValue;

        public List<Command> commands = new List<Command> ();
        private Dictionary<ulong, int> programCounters = new Dictionary<ulong, int> ();

        public CommandRoot() { }
        
        public CommandRoot (List<Command> _commands) {
            commands = _commands;
        }

        /// <summary>
        /// This is the entrance method for the command system, call this. You can have multiple roots, for instance for different servers, and it should work just fine, since commands get their information from the SocketMessage object.
        /// </summary>
        /// <param name="userMessage"></param>
        /// <returns>Returns the result of the command, if one is found. Otherwise null.</returns>
        public async Task<Command.Result> EnterCommand (SocketUserMessage userMessage) {

            if (string.IsNullOrWhiteSpace (userMessage.Content))
                return null;

            string message = userMessage.Content;
            if (message [ 0 ].IsCommandTrigger ()) {

                string [ ] multiline = message.Split (lineSeperator);
                FoundCommandResult finalResult = null;
                programCounters.Add (userMessage.Id, 0);

                CommandMetadata metadata = new CommandMetadata (userMessage, this);
                while (programCounters[userMessage.Id] < multiline.Length) {
                    int ln = programCounters [ userMessage.Id ];
                    programCounters [ userMessage.Id ]++;

                    string trimmed = multiline[ln].Trim ('\n', '\t', ' '); // Trim off whitespace for consistancy.

                    try {
                        finalResult = await FindAndExecuteCommand (metadata, trimmed, commands);
                    } catch (Exception exception) {
                        finalResult = new FoundCommandResult (new Command.Result (exception, (exception.Message + " - " + exception.StackTrace).Substring (0, 1995)), null);
                        break;
                    }

                }

                CommandVariables.Clear (userMessage.Id);
                return finalResult?.result;
            }

            return null;
        }

        public void SetProgramCounter (ulong messageId, int position) {
            if (!programCounters.ContainsKey (messageId))
                throw new InvalidOperationException ("There is no program with this ID.");

            if (position < 0)
                throw new InvalidOperationException ("Position cannot be below 0");

            programCounters [ messageId ] = position;
        }

        public void EndProgram (ulong messageId) {
            SetProgramCounter (messageId, int.MaxValue);
        }

        /// <summary>
        /// This must be run after the initial commands has been added to the root.
        /// </summary>
        public void InitializeCommands () {
            foreach (Command cmd in commands) {
                cmd.Initialize ();
            }
        }

        public static async Task<FoundCommandResult> FindAndExecuteCommand(CommandMetadata metadata, string fullCommand, List<Command> commandList) {
            string cmd = "";
            List<object> arguments = ConstructArguments (fullCommand.Substring (1), out cmd);

            Console.WriteLine (fullCommand + ", " + cmd);

            return await FindAndExecuteCommand (metadata, cmd, arguments, commandList);
        }

        private static async Task<FoundCommandResult> FindAndExecuteCommand(CommandMetadata metadata, string commandName, List<object> arguments, List<Command> commandList) {
            if (metadata.depth > metadata.root.maxCommandLength)
                throw new OverflowException ("Max command-program depth exceeded.");

            for (int i = 0; i < commandList.Count; i++) {

                if (commandList[ i ].command == commandName) {
                    if (arguments.Count > 0 && arguments [ 0 ].ToString () == "?") {

                        Command command = commandList [ i ];
                        if (command is CommandSet) {
                            CommandSet [ ] totalFromDublicates = commandList.Where (x => x.command == commandName).Cast<CommandSet>().ToArray ();
                            return new FoundCommandResult (new Command.Result (null, Command.ListCommands (metadata, totalFromDublicates)), command);
                        } else {
                            return new FoundCommandResult (new Command.Result (command.GetHelpEmbed (metadata, true), ""), command);
                        }

                    } else {
                        metadata.depth++;
                        FoundCommandResult result = new FoundCommandResult (await commandList [ i ].TryExecute (metadata, arguments.ToArray ()), commandList [ i ]);
                        if (result.result != null) {
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

            int quatationBalance = 0;

            int balance = 0;
            int lastCut = 0;

            for (int i = 0; i < toSplit.Length; i++) {
                char cur = toSplit [ i ];

                if (cur == '[')
                    quatationBalance++;
                if (cur == ']')
                    quatationBalance--;

                if (quatationBalance == 0) {
                    switch (cur) {
                        case argSeperator:
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
            }

            if (toSplit.Length > 0) {
                arguments.Add (toSplit.Substring (lastCut));
            }

            for (int i = 0; i < arguments.Count; i++) {
                arguments [ i ] = arguments [ i ].Trim (' ');
            }

            return arguments.ToArray ();
        }

        public List<Command> GetCommands() {
            return commands;
        }

        public void AddCommands(params Command [ ] newCommands) {
            commands.AddRange (newCommands);
            foreach (Command cmd in newCommands)
                cmd.Initialize ();
        }

        public void RemoveCommands(params Command [ ] toRemove) {
            foreach (Command cmd in toRemove) {
                commands.Remove (cmd);
            }
        }
    }
}
