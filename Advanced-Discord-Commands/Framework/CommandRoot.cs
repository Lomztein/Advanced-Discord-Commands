using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework {
    public class CommandRoot : ICommandRoot, ICommandSet {

        public char Trigger { get; set; } = '!';
        public char HiddenTrigger { get; set; } = '/';

        public const char argSeperator = ',';
        public const char lineSeperator = ';';
        public const char commandHelp = '?';

        public const char forcedStringStart = '[';
        public const char forcedStringEnd = ']';

        public const char nestedCommandStart = '(';
        public const char nestedCommandEnd = ')';

        public const char getVariableStart = '{';
        public const char getVariableEnd = '}';

        public const char arrayVariableStart = '[';
        public const char arrayVariableEnd = ']';

        public static char [ ] whitespaceChars = { '\n', '\t', ' ' };

        public int maxCommandLength = short.MaxValue;

        public List<ICommand> commands = new List<ICommand> ();

        public string Name { get => "Command Root"; set => throw new NotImplementedException (); }
        public string Description { get => "Commands are entered into this entity, whereafter they are routed around to their destination."; set => throw new NotImplementedException (); }

        public CommandRoot() { }
        
        public CommandRoot (List<ICommand> _commands, char _trigger, char _hiddenTrigger) {
            commands = _commands;
            Trigger = _trigger;
            HiddenTrigger = _hiddenTrigger;
        }

        /// <summary>
        /// This is the entrance method for the command system, call this. You can have multiple roots, for instance for different servers, and it should work just fine, since commands get their information from the SocketMessage object.
        /// </summary>
        /// <param name="userMessage"></param>
        /// <returns>Returns the result of the command, if one is found. Otherwise null.</returns>
        public async Task<Command.Result> EnterCommand (IMessage input) {
            return await EnterCommand (input.Content, input);
        }

        /// <summary>
        /// Same as other entrance method, except that you can send in a seperate message than the one in the input, in case you that's not what you want to do.
        /// </summary>
        /// <param name="commandMessage"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Command.Result> EnterCommand (string commandMessage, IMessage input) {

            if (input.Source != MessageSource.User)
                return null;

            if (string.IsNullOrWhiteSpace (input.Content))
                return null;

            string message = commandMessage.Trim (whitespaceChars);
            if (this.IsCommandTrigger (message[0], out bool isHidden)) {

                string [ ] multiline = SplitMultiline (message);
                FoundCommandResult finalResult = null;

                CommandMetadata metadata = new CommandMetadata (input, this);

                while (metadata.programCounter < multiline.Length) {

                    uint line = metadata.programCounter;
                    metadata.programCounter++;

                    string trimmed = multiline[line].Trim (whitespaceChars); // Trim off whitespace for consistancy.

                    try {
                        finalResult = await FindAndExecuteCommand (metadata, trimmed, commands);
                    } catch (Exception exception) {
                        finalResult = new FoundCommandResult (new Command.Result (exception, "Error - " + exception.Message), null);
                        break;
                    }

                }

                CommandVariables.Clear (input.Id);
                return finalResult?.result;
            }

            return null;
        }

        public void SetProgramCounter (CommandMetadata metadata, uint position) {
            if (metadata == null) // Lol this doesn't even make sense.
                throw new InvalidOperationException ("There is no program with this ID.");

            metadata.programCounter = position;
        }

        public void EndProgram (CommandMetadata metadata) {
            SetProgramCounter (metadata, int.MaxValue);
        }

        /// <summary>
        /// This must be run after the initial commands has been added to the root.
        /// </summary>
        public void InitializeCommands () {
            foreach (Command cmd in commands) {
                cmd.CommandParent = this;
                cmd.ParentRoot = this;
                cmd.Initialize ();
            }
        }

        public static async Task<FoundCommandResult> FindAndExecuteCommand(CommandMetadata metadata, string fullCommand, List<ICommand> commandList) {
            string cmd = "";
            List<object> arguments = ConstructArguments (fullCommand.Trim (whitespaceChars).Substring (1), out cmd);

            return await FindAndExecuteCommand (metadata, cmd, arguments, commandList);
        }

        public static async Task<FoundCommandResult> FindAndExecuteCommand(CommandMetadata metadata, string commandName, List<object> arguments, List<ICommand> commandList) {
            if (metadata.depth > metadata.root.maxCommandLength)
                throw new OverflowException ("Max command-program depth exceeded.");

            commandName = commandName.Trim (whitespaceChars);

            for (int i = 0; i < commandList.Count; i++) {

                if (commandList[ i ].Name == commandName) {
                    if (arguments.Count > 0 && arguments [ 0 ].ToString ()[0] == commandHelp) {

                        ICommand command = commandList [ i ];
                        if (command is CommandSet) {
                            CommandSet [ ] totalFromDublicates = commandList.Where (x => x.Name == commandName).Cast<CommandSet>().ToArray ();
                            return new FoundCommandResult (new Command.Result (null, Command.ListCommands (metadata, totalFromDublicates)), command);
                        } else {
                            return new FoundCommandResult (new Command.Result (command.GetHelpEmbed (metadata.message, true), ""), command);
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
            public ICommand command;

            public FoundCommandResult(Command.Result _result, ICommand _command) {
                result = _result;
                command = _command;
            }
        }

        // Don't think about it too much.
        public static string [ ] SplitMultiline(string input) {

            List<string> result = new List<string> ();

            string arg;
            int quatationBalance = 0;
            int lastCut = 0;

            for (int i = 0; i < input.Length; i++) {
                char cur = input [ i ];

                if (cur == forcedStringStart)
                    quatationBalance++;
                if (cur == forcedStringEnd)
                    quatationBalance--;

                if (cur == lineSeperator && quatationBalance == 0) {
                    arg = input.Substring (lastCut, i - lastCut).Trim (whitespaceChars);
                    result.Add (arg);
                    lastCut = i + 1;
                }
            }

            if (input.Length > 0) {
                arg = input.Substring (lastCut).Trim (whitespaceChars);
                if (arg.Length > 0)
                    result.Add (arg);
            }

            return result.ToArray ();
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

                if (cur == forcedStringStart)
                    quatationBalance++;
                if (cur == forcedStringEnd)
                    quatationBalance--;

                if (quatationBalance == 0) {
                    switch (cur) {
                        case argSeperator:
                            if (balance == 0) {
                                arg = toSplit.Substring (lastCut, i - lastCut).Trim (whitespaceChars);
                                arguments.Add (arg);
                                lastCut = i + 1;
                            }
                            break;

                        case nestedCommandStart:
                            balance++;
                            break;

                        case nestedCommandEnd:
                            balance--;
                            break;
                    }
                }
            }

            if (toSplit.Length > 0) {
                arguments.Add (toSplit.Substring (lastCut));
            }

            for (int i = 0; i < arguments.Count; i++) {
                arguments [ i ] = arguments [ i ].Trim (whitespaceChars);
            }

            return arguments.ToArray ();
        }

        public List<ICommand> GetCommands() {
            return commands;
        }

        public void AddCommands(params ICommand [ ] newCommands) {
            commands.AddRange (newCommands);
            foreach (ICommand cmd in newCommands)
                cmd.Initialize ();
        }

        public void RemoveCommands(params ICommand [ ] toRemove) {
            foreach (ICommand cmd in toRemove) {
                commands.Remove (cmd);
            }
        }

        public string GetChildPrefix() => Trigger.ToString ();
    }
}
