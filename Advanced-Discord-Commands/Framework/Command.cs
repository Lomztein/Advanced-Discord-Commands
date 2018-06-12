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
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using Lomztein.AdvDiscordCommands.Framework.Categories;

namespace Lomztein.AdvDiscordCommands.Framework {

    public abstract class Command : ICommand, ICommandChild {

        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Aliases { get; set; }

        public ICategory Category { get; set; }
        public ICommandParent CommandParent { get; set; }
        public ICommandRoot ParentRoot { get; internal set; }

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
                                        result = result [ index ];
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
            MethodInfo [ ] infos = GetExecuteMethods ();
            dynamic parameterList = new List<object> ();

            foreach (MethodInfo inf in infos) {
                List<Parameter> paramInfo = GetMethodParameters (inf);

                bool anyParams = paramInfo.Any (x => x.HasAttribute (typeof (ParamArrayAttribute)));
                bool isMethod = paramInfo.Count - 1 == arguments.Length || (anyParams && arguments.Length >= paramInfo.Count); // Have to off-by-one since all commands gets the CommandMetadata parsed through.

                if (isMethod == true) {
                    // Go through all parameters except the first one.
                    for (int i = 1; i < paramInfo.Count; i++) {
                        try {
                            int argIndex = i - 1;
                            object arg = arguments [ argIndex ];

                            // Is the parameter given the params attribute? If so then pack all the remaining arguments into an array.
                            if (paramInfo [ i ].HasAttribute (typeof (ParamArrayAttribute)) && !arguments [ argIndex ].GetType ().IsArray) {
                                Type elementType = paramInfo [ i ].type.GetElementType ();

                                // Since lists are easier to work with, but not quite straightforward to create dynamically, do this.
                                dynamic dyn = Activator.CreateInstance (typeof (List<>).MakeGenericType (elementType));
                                for (int j = argIndex; j < arguments.Length; j++) {
                                    TryAddToParams (ref dyn, arguments [ j ], elementType);
                                }

                                arg = dyn.ToArray ();
                            }

                            TryAddToParams (ref parameterList, arg, paramInfo [ i ].type);
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

        public virtual async Task<Result> TryExecute(CommandMetadata data, params object [ ] arguments) {
            string executionError = AllowExecution (data.message);
            string executionPrefix = "Failed to execute command " + Name;
            if (executionError == "") {

                arguments = (await ConvertChainCommandsToObjects (data, arguments.ToList ())).ToArray ();
                FindMethodResult result = FindMethod (arguments);
                if (result != null) {
                    try {

                        result.parameters.Insert (0, data);
                        Result task = await (result.method.Invoke (this, result.parameters.ToArray ()) as Task<Result>);
                        AddToCallstack (data.message.Id, new Callstack.Item (this, result.parameters.GetRange (1, result.parameters.Count - 1).Select (x => x.ToString ()).ToList (), task.message, task.value));
                        return task;

                    } catch (TargetInvocationException exception) {
                        throw exception.InnerException;
                    }
                } else {
                    return new Result (null, $"{executionPrefix}: \n\tNo suitable command overload found.");
                }
            } else {
                return new Result (null, $"{executionPrefix}: Failed to execute: {executionError}");
            }
        }

        public Task<Result> TaskResult(object value, string message) {
            return Task.FromResult (new Result (value, message));
        }

        public virtual string AllowExecution (IMessage e) {

            if (e.Id == 0) // If it is a fake message, then just continue.
                return "";

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

        private MethodInfo[] GetExecuteMethods () {
            MethodInfo [ ] infos = GetType ().GetMethods ().Where (x => x.IsDefined (typeof (OverloadAttribute))).ToArray ();
            return infos;
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

        public Embed GetHelpEmbed(IMessage e, bool advanced) {
            EmbedBuilder builder = new EmbedBuilder ();
            if (!commandEnabled) {
                builder.WithTitle ("Not enabled on this server.");
                return builder.Build ();
            }

            builder.WithAuthor ("Bot Command Help") // lolwhat.
                .WithTitle ($"Command \"{this.GetPrefix ()}{Name}\"")
                .WithDescription (Description);

            // This is quite similar to GetArgs and GetHelp together, and the other functions are obsolete due to this.
            MethodInfo [ ] methods = GetExecuteMethods ();
            for (int i = 0; i < methods.Length; i++) {

                OverloadAttribute overload = methods [ i ].GetCustomAttribute<OverloadAttribute> ();
                if (overload == null) {
                    builder.AddField ("Undefined overload", "Missing overload attribute for Execute method.");
                } else {
                    MethodInfo info = methods [ i ];

                    var parameters = GetDescriptiveOverloadParameters (info);
                    string olText = advanced ? $"{parameters.returnType} => " : this.GetPrefix () + Name;

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
            return this.GetPrefix () + Name;
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
            return StringExtensions.UniformStrings (GetCommand (), Description, " | ");
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

        public class Parameter {

            public string name;
            public Type type;
            public List<Type> attributes = new List<Type> ();

            public Parameter(string _name, Type _type, params Type[] _attributes) {
                name = _name;
                type = _type;

                foreach (Type attribute in _attributes) {
                    attributes.Add (attribute);
                }
            }

            public bool HasAttribute(Type attributeType) => attributes.Contains (attributeType);

        }

        public virtual List<Parameter> GetMethodParameters (MethodInfo info) {
            List<Parameter> parameters = new List<Parameter> ();

            ParameterInfo[] paramInfos = info.GetParameters ();
            foreach (ParameterInfo paramInfo in paramInfos) {
                Parameter parameter = new Parameter (paramInfo.Name, paramInfo.ParameterType, paramInfo.CustomAttributes.Select (x => x.AttributeType).ToArray ());
                parameters.Add (parameter);
            }

            return parameters;
        }

        public static string ListCommands(CommandMetadata data, params ICommandSet [ ] sets) {
            List<ICommand> combined = new List<ICommand> ();
            foreach (CommandSet set in sets)
                combined.AddRange (set.commandsInSet);
            return ListCommands (data, sets.First ().Name, combined.ToArray ());
        }

        public static string ListCommands(CommandMetadata data, string name, params ICommand[] commands) {
            // Display all commands within command.
            string result = "";

            List<Command> withoutDublicates = new List<Command> ();
            foreach (Command cmd in commands) {
                if (!withoutDublicates.Exists (x => x.Name == cmd.Name))
                    withoutDublicates.Add (cmd);
            }

            result += ("Commands in the **" + name + "** command set:\n```");

            var catagories = withoutDublicates.Where (x => x.AllowExecution (data.message) == "").GroupBy (x => x.Category);

            foreach (var catagory in catagories) {
                result += catagory.ElementAt (0).Category.Name + " Commands\n";
                foreach (var item in catagory) {
                    result += item.Format () + "\n";
                }
                result += "\n";
            }

            if (result == "Commands in the **" + name + "** command set:\n```") { // Ew
                return "This set contains no available commands.";
            }

            return result + "```";
        }

        public override string ToString() => this.GetPrefix () + Name;

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
