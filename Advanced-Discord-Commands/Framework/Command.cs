using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Reflection;

using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Misc;

namespace Lomztein.AdvDiscordCommands.Framework {

    public abstract class Command {

        public static char commandTrigger = '!';

        public enum Category {
            None, Utility, Fun, Set, Advanced, Admin 
        }

        public const int CHARS_TO_HELP = 4;

        public string command = null;
        public string shortHelp = null;
        public string helpPrefix = commandTrigger.ToString ();
        public Category catagory = Category.None;

        public bool availableInDM = false;
        public bool availableOnServer = true;
        public bool commandEnabled = true;

        public List<GuildPermission> requiredPermissions = new List<GuildPermission>();

        public static List<Callstack> callstacks = new List<Callstack> ();
        public static int maxCallstacks = 64;

        public class FindMethodResult {
            public MethodInfo method;
            public List<object> parameters;

            public FindMethodResult(MethodInfo _method, List<object> _parameters) {
                method = _method; parameters = _parameters;
            }
        }

        public async Task<List<object>> ConvertChainCommandsToObjects(CommandMetadata data, List<object> input) {
            List<object> converted = new List<object> ();

            foreach (object obj in input) {
                dynamic result = obj;
                string stringObj = obj.ToString ();

                if (stringObj.Length > 0) {

                    if (stringObj [ 0 ] == '(' && stringObj.Length > 2) { // If it is hard-defined arguments, required for nested commands.
                        stringObj = stringObj.Substring (1, stringObj.Length - 2);
                        if (stringObj [ 0 ] == '!') {
                            var foundCommandResult = await CommandRoot.FindAndExecuteCommand (data, stringObj, data.root.commands);

                            if (foundCommandResult.result != null) {
                                result = foundCommandResult.result.value;
                            }
                        } else {
                            result = stringObj;
                        }
                    } else if (stringObj [ 0 ] == '{') { // Synonymous for !var getl.
                        int endIndex = stringObj.IndexOf ('}');
                        if (endIndex != -1) {
                            string varName = stringObj.Substring (1, endIndex - 1);

                            result = CommandVariables.Get (data.message.Id, varName);
                            if (result == null) // If no local variable, fallback to personal.
                                result = CommandVariables.Get (data.message.Author.Id, varName);
                            if (result == null) { // If no personal variable, fallback to server.
                                SocketGuild guild = data.message.GetGuild ();
                                if (guild != null)
                                    result = CommandVariables.Get (guild.Id, varName);
                            }

                            if (stringObj.Length > endIndex + 1 && stringObj[endIndex + 1] == '[') {
                                int squareEnd = stringObj.LastIndexOf (']');
                                if (squareEnd != -1) {

                                    string number = stringObj.Substring (endIndex + 2, squareEnd - (endIndex + 2));
                                    if (int.TryParse (number, out int index)) {
                                        if (result.GetType ().IsArray) {
                                            result = result [ index ];
                                        }
                                    }
                                }
                            }
                        }
                    } else if (stringObj [ 0 ] == '<') { // If it's a mention of a Discord object.
                        IMentionable mentionable = stringObj.ExtractMentionable (data.message.GetGuild ());
                        result = mentionable;
                    }else if (stringObj[0] == '[') {
                        result = stringObj.Substring (1, stringObj.Length - 2);
                    }
                }

                converted.Add (result);
            }

            return converted;
        }

        public FindMethodResult FindMethod(params object [ ] arguments) {
            // All end-command code is written as "Execute" functions, in order for the reflection to easily find it. Alternatively use attributes.
            MethodInfo [ ] infos = GetType ().GetMethods ().Where (x => x.Name == "Execute").ToArray ();
            dynamic parameterList = new List<object> ();

            foreach (MethodInfo inf in infos) {
                ParameterInfo [ ] paramInfo = inf.GetParameters ();

                bool anyParams = paramInfo.Any (x => x.IsDefined (typeof (ParamArrayAttribute)));
                bool isMethod = paramInfo.Length - 1 == arguments.Length || (anyParams && arguments.Length >= paramInfo.Length); // Have to off-by-one since all commands gets the SocketUserMessage parsed through.

                if (isMethod == true) {
                    // Go through all parameters except the first one.
                    for (int i = 1; i < paramInfo.Length; i++) {
                        try {
                            int argIndex = i - 1;
                            object arg = arguments [ argIndex ];

                            // Is the parameter given the params attribute? If so then pack all the remaining arguments into an array.
                            if (paramInfo [ i ].IsDefined (typeof (ParamArrayAttribute)) && !arguments [ argIndex ].GetType ().IsArray) {
                                Type elementType = paramInfo [ i ].ParameterType.GetElementType ();

                                // Since lists are easier to work with, but not quite straightforward to create dynamically, do this.
                                dynamic dyn = Activator.CreateInstance (typeof (List<>).MakeGenericType (elementType));
                                for (int j = argIndex; j < arguments.Length; j++) {
                                    TryAddToParams (ref dyn, arguments [ j ], elementType);
                                }

                                arg = dyn.ToArray ();
                            }

                            TryAddToParams (ref parameterList, arg, paramInfo [ i ].ParameterType);
                        } catch {
                            isMethod = false;
                            parameterList.Clear ();
                            break;
                        }
                    }
                }

                if (isMethod) {
                    return new FindMethodResult (inf, parameterList);
                }
            }

            return null;
        }

        private void TryAddToParams(ref dynamic paramList, object arg, Type type) {
            dynamic result = TryConvert (arg, type);
            paramList.Add (result);
        }

        private object TryConvert(object toConvert, Type type) {
            try {
                if (toConvert != null) {
                    dynamic obj = Convert.ChangeType (toConvert, type);
                    return obj;
                } else {
                    throw new Exception ();
                }
            } catch {
                if (type.IsInstanceOfType (toConvert) || toConvert == null)
                    return toConvert;
                else
                    throw new Exception ();
            }
        }

        public virtual async Task<Result> TryExecute(CommandMetadata data, params object[] arguments) {
            string executionError = AllowExecution (data);
            string executionPrefix = "Failed to execute command " + command;
            if (executionError == "") {

                /*if (arguments.Length == 1) {
                    string stringArg = arguments [ 0 ].ToString ();
                    if (stringArg [ 0 ] == '(')
                        arguments = CommandRoot.SplitArgs (GetParenthesesArgs (stringArg)).ToArray ();
                }*/

                arguments = (await ConvertChainCommandsToObjects (data, arguments.ToList ())).ToArray ();
                FindMethodResult result = FindMethod (arguments);
                if (result != null) {
                    try {

                        result.parameters.Insert (0, data);
                        Result task = await (result.method.Invoke (this, result.parameters.ToArray ()) as Task<Result>);
                        AddToCallstack (data.message.Id, new Callstack.Item (this, result.parameters.GetRange(1, result.parameters.Count - 1).Select (x => x.ToString ()).ToList (), task.message, task.value));
                        return task;

                    } catch (Exception exc) {
                        Logging.Log (exc);
                    }
                } else {
                    return new Result (null, $"{executionPrefix}: \n\tNo suitable command overload found.");
                }
            } else {
                return new Result (null, $"{executionPrefix}: Failed to execute: {executionError}");
            }
            return null;
        }

        public Task<Result> TaskResult(object value, string message) {
            return Task.FromResult (new Result (value, message));
        }

        public virtual string AllowExecution (SocketMessage e) {

            string errors = string.Empty;

            if (commandEnabled == false)
                errors += "\n\tNot enabled on this server.";

            if (!availableInDM && e.Channel as SocketDMChannel != null) {
                errors += "\n\tNot available in DM channels.";
            }

            if (!(e.Author as SocketGuildUser).HasAllPermissios (requiredPermissions)) {
                errors += "\n\tUser does not have permission.";
            }

            if (!availableOnServer && e.Channel as SocketGuildChannel != null) {
                errors += "\n\tNot avaiable on server.";
            }

            return errors;
        }

        public virtual void Initialize () {
        }

        public static void AddToCallstack(ulong chainID, Callstack.Item item) {
            Callstack curStack = callstacks.Find (x => x.chainID == chainID);
            if (curStack == null) {
                curStack = new Callstack (chainID);
                callstacks.Insert (0, curStack);
            }

            curStack.items.Add (item);
            if (callstacks.Count > maxCallstacks)
                callstacks.RemoveAt (maxCallstacks);
        }

        public virtual string GetHelpText(SocketMessage e) => "";

        public Embed GetHelpEmbed(SocketMessage e, bool advanced) {
            EmbedBuilder builder = new EmbedBuilder ();
            if (!commandEnabled) {
                builder.WithTitle ("Not enabled on this server.");
                return builder.Build ();
            }

            builder.WithAuthor ("Bot Command Help") // lolwhat.
                .WithTitle ($"Command \"{helpPrefix}{command}\"")
                .WithDescription (shortHelp);

            // This is quite similar to GetArgs and GetHelp together, and the other functions are obsolete due to this.
            MethodInfo [ ] methods = GetType ().GetMethods ().Where (x => x.Name == "Execute").ToArray ();
            for (int i = 0; i < methods.Length; i++) {

                OverloadAttribute overload = methods [ i ].GetCustomAttribute<OverloadAttribute> ();
                if (overload == null) {
                    builder.AddField ("Undefined overload", "Missing overload attribute for Execute method.");
                } else {
                    MethodInfo info = methods [ i ];

                    var parameters = GetDescriptiveOverloadParameters (info);
                    string olText = advanced ? $"{parameters.returnType} => " : helpPrefix + command;

                    olText += " (";
                    for (int j = 1; j < parameters.types.Length; j++) { // Remember to ignore first parameter, it being the SocketUserMessage.
                        Type type = parameters.types[j];
                        string name = parameters.names [ j ];

                        olText += advanced ? type.Name + " " + name : name;

                        if (j != parameters.types.Length - 1)
                            olText += ", ";
                    }
                    olText += ")";

                    builder.AddField (olText, overload.Description);
                }
            }

            string footer = string.Empty;
            if (availableInDM && !availableOnServer)
                footer += " - ONLY IN DM";
            if (availableInDM && availableOnServer)
                footer += " - AVAILABLE IN DM";

            if (requiredPermissions.Count > 0) {
                footer += " - REQUIRES PERMISSIONS: ";
                for (int i = 0; i < requiredPermissions.Count; i++) {
                    footer += requiredPermissions [ i ].ToString ().ToUpper ();

                    if (i != requiredPermissions.Count - 1)
                        footer += ", ";
                }
            }

            builder.WithFooter (footer);
            return builder.Build ();
        }

        public virtual string GetShortHelp () {
            return shortHelp;
        }

        /// <summary>
        /// This is supposed to be used with the autodocumentation functions, NOT with any actual functionalitety, since it doesn't return any parameter metadata, only type and name.
        /// </summary>
        /// <param name="overloadIndex"></param>
        /// <returns></returns>
        public virtual (Type [ ] types, string [ ] names, string returnType) GetDescriptiveOverloadParameters(MethodInfo info) {

            List<Type> paramTypes = new List<Type> ();
            List<string> paramNames = new List<string> ();

            foreach (var param in info.GetParameters ()) {
                paramTypes.Add (param.ParameterType);
                paramNames.Add (param.Name);
            }

            return (paramTypes.ToArray (), paramNames.ToArray (), info.GetCustomAttribute<OverloadAttribute>().ReturnType.Name);
        }

        public virtual string GetCommand() {
            return helpPrefix + command;
        }

        public virtual string GetOnlyName() {
            return command; // Wrapper functions ftw
        }

        public Command CloneCommand() {
            return this.MemberwiseClone () as Command;
        }

        public static string GetParenthesesArgs(string input) {
            int startIndex = 0, endIndex = input.Length;
            int balance = 0;

            for (int i = 0; i < input.Length; i++) {
                if (input [ i ] == '(') {
                    if (balance == 0)
                        startIndex = i;
                    balance++;
                }
                if (input [ i ] == ')') {
                    balance--;

                    if (balance == 0) {
                        endIndex = i;
                        break;
                    }
                }
            }
            return input.Substring (startIndex + 1, endIndex - startIndex - 1);
        }

        public string Format (string connector = " | ", int minSpaces = 25) {
            return StringExtensions.UniformStrings (GetCommand (), GetShortHelp (), " | ");
        }

        public static bool TryIsolateWrappedCommand(string input, out string cmd, out List<object> args) {
            cmd = "";
            args = new List<object> ();

            if (input.Length > 1 && input [ 1 ].IsCommandTrigger ()) {
                args = CommandRoot.ConstructArguments (GetParenthesesArgs (input), out cmd);
                cmd = cmd.Substring (1);
                return true;
            }
            return false;
        }

        public class Overload {
            public string returnType;
            public string description;

            public Overload(Type _returnType, string _description) {
                returnType = _returnType.Name;
                description = _description;
            }
        }

        public class Result {
            public object value;
            public string message;

            public Result(object _value, string _message) {
                value = _value;
                message = _message;
            }
        }

        public override string ToString() => helpPrefix + command;

        [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = false)]

        protected class OverloadAttribute : Attribute {
            public Type ReturnType;
            public string Description;

            public OverloadAttribute (Type _returnType, string _description) {
                ReturnType = _returnType;
                Description = _description;
            }
        }
    }
}
