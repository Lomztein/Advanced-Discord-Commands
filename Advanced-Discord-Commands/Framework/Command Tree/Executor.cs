using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public class Executor : IExecutor
    {
        public Func<ulong, char> Trigger { get; set; } = (x => '!');
        public Func<ulong, char> HiddenTrigger { get; set; } = (x => '/');

        public const char defaultTrigger = '!';
        public const char defaultHiddenTrigger = '/';

        public const char argSeperator = ',';
        public const char lineSeperator = ';';
        public const char commandHelp = '?';

        public const char forcedStringStart = '[';
        public const char forcedStringEnd = ']';

        public const char nestedCommandStart = '(';
        public const char nestedCommandEnd = ')';

        public const char getVariableStart = '{';
        public const char getVariableEnd = '}';

        public const char arrayIndexStart = '[';
        public const char arrayIndexEnd = ']';


        public int maxCommandLength = short.MaxValue;

        public ICommand FindCommand(string fullCommand, List<ICommand> commandList, ulong? owner) {

            string commandName = fullCommand.ExtractCommandName (owner, this);

            foreach (ICommand cmd in commandList) {
                if (cmd.IsCommand (commandName))
                    return cmd;
            }

            return null;

        }

        public char GetTrigger(ulong? owner) {
            if (!owner.HasValue)
                return defaultTrigger;
            return GetTrigger (owner.Value);
        }

        public char GetHiddenTrigger(ulong? owner) {
            if (!owner.HasValue)
                return defaultHiddenTrigger;
            return GetTrigger (owner.Value);
        }

        public char GetTrigger(ulong entityId) => Trigger (entityId);
        public char GetHiddenTrigger(ulong entityId) => HiddenTrigger (entityId);

        public async Task<object[]> ParseChainElements(CommandMetadata data, object[] arguments) {
            List<object> converted = new List<object> ();

            foreach (object obj in arguments) {
                dynamic result = obj;
                string stringObj = obj.ToString ();

                if (stringObj.Length > 0) {

                    if (stringObj[0] == nestedCommandStart && stringObj.Length > 2) { // If it is hard-defined arguments, required for nested commands.
                        stringObj = stringObj.Substring (1, stringObj.Length - 2);
                        if (stringObj[0].IsCommandTrigger (data.Owner, this)) {

                            object[] chainArguments = ParseArguments (stringObj).Cast<object> ().ToArray ();
                            Execution chainExecution = this.CreateExecution (data, stringObj, chainArguments, data.Root.GetCommands ());
                            Result chainExecutionResult = await Execute (chainExecution);

                            if (chainExecutionResult != null)
                                result = chainExecutionResult.Value;

                        } else {
                            result = stringObj;
                        }
                    } else if (stringObj[0] == getVariableStart) { // Synonymous for !var getl.
                        int endIndex = stringObj.IndexOf (getVariableEnd);
                        if (endIndex != -1) {
                            string varName = stringObj.Substring (1, endIndex - 1);

                            result = CommandVariables.Get (data.Message.Id, varName);
                            if (result == null) // If no local variable, fallback to personal.
                                result = CommandVariables.Get (data.Message.Author.Id, varName);
                            if (result == null) { // If no personal variable, fallback to server.
                                SocketGuild guild = data.Message.GetGuild ();
                                if (guild != null)
                                    result = CommandVariables.Get (guild.Id, varName);
                            }

                            if (stringObj.Length > endIndex + 1 && stringObj[endIndex + 1] == arrayIndexStart) {
                                int squareEnd = stringObj.LastIndexOf (arrayIndexEnd);
                                if (squareEnd != -1) {

                                    string number = stringObj.Substring (endIndex + 2, squareEnd - (endIndex + 2));
                                    if (int.TryParse (number, out int index)) {
                                        result = result[index];
                                    }
                                }
                            }
                        }
                    } else if (stringObj[0] == '<') { // If it's a mention of a Discord object.
                        IMentionable mentionable = stringObj.ExtractMentionable (data.Message.GetGuild ());
                        result = mentionable;
                    } else if (stringObj[0] == forcedStringStart) {
                        result = stringObj.Substring (1, stringObj.Length - 2);
                    }
                }

                converted.Add (result);
            }

            return converted.ToArray ();
        }

        public string[] ParseArguments(string fullCommand) {
            string toSplit = fullCommand.ExtractArgumentPart ();

            if (string.IsNullOrEmpty (toSplit))
                return new string[0];

            List<string> arguments = new List<string> ();
            string arg;

            int quatationBalance = 0;

            int balance = 0;
            int lastCut = 0;

            for (int i = 0; i < toSplit.Length; i++) {
                char cur = toSplit[i];

                if (cur == forcedStringStart)
                    quatationBalance++;
                if (cur == forcedStringEnd)
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
                arguments[i] = arguments[i].Trim (CharExtensions.WhitespaceChars);
            }

            return arguments.ToArray ();
        }

        // Don't think about it too much.
        public string[] ParseMultiline(string input) {

            List<string> result = new List<string> ();

            string arg;
            int quatationBalance = 0;
            int lastCut = 0;

            for (int i = 0; i < input.Length; i++) {
                char cur = input[i];

                if (cur == forcedStringStart)
                    quatationBalance++;
                if (cur == forcedStringEnd)
                    quatationBalance--;

                if (cur == lineSeperator && quatationBalance == 0) {
                    arg = input.Substring (lastCut, i - lastCut);
                    result.Add (arg);
                    lastCut = i + 1;
                }
            }

            if (input.Length > 0) {
                arg = input.Substring (lastCut);
                if (arg.Length > 0)
                    result.Add (arg);
            }

            for (int i = 0; i < result.Count; i++) {
                result[i] = result[i].Trim (CharExtensions.WhitespaceChars);
            }

            return result.ToArray ();
        }

        public async Task<Result> Execute(Execution execution) {
            execution.SetArguments (await ParseChainElements (execution.Metadata, execution.Arguments));
            var result = await execution.TryExecute ();
            return result;
        }
    }
}
