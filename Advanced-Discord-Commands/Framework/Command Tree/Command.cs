using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Reflection;

using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using Lomztein.AdvDiscordCommands.Autodocumentation;

namespace Lomztein.AdvDiscordCommands.Framework {

    public abstract class Command : ICommand, ICommandChild {

        private const int ListEmbedDescriptionMaxWidth = 28;

        public string Name { get; set; }

        /// <summary>
        /// Keep in mind that Discord embeds are quite short, so there is only limited space for description characters before it wraps to the next line. Approximately 30 characters.
        /// </summary>
        public string Description { get; set; }
        public string[] Aliases { get; set; } = new string[0];

        public ICategory Category { get; set; }
        public ICommandParent CommandParent { get; set; }
        public ICommandRoot ParentRoot { get; internal set; }

        public bool AvailableInDM { get; set; } = false;
        public bool AvailableOnServer { get; set; } = true;
        public bool CommandEnabled { get; set; } = true;

        public List<GuildPermission> RequiredPermissions { get; set; } = new List<GuildPermission>();

        public class FindMethodResult {

            public CommandOverload overload;
            public List<object> convertedArguments;

            public FindMethodResult(CommandOverload _overload, List<object> _parameters) {
                overload = _overload; convertedArguments = _parameters;
            }
        }

        public FindMethodResult FindMethod(params object [ ] arguments) {
            // All end-command code is written as "Execute" functions, in order for the reflection to easily find it. Alternatively use attributes.

            CommandOverload [ ] overloads = GetOverloads ();
            dynamic argumentList = new List<object> ();

            foreach (CommandOverload overload in overloads) {

                argumentList = new List<object> ();
                var parameters = overload.Parameters;

                bool anyParams = parameters.Any (x => x.HasAttribute (typeof (ParamArrayAttribute)));
                bool isMethod = parameters.Length == arguments.Length || (anyParams && arguments.Length >= parameters.Length); // Have to off-by-one since all commands gets the CommandMetadata parsed through.

                if (isMethod == true) {
                    // Go through all parameters except the first one.
                    for (int i = 0; i < parameters.Length; i++) {
                        try {
                            object arg = arguments [ i ];

                            // Is the parameter given the params attribute? If so then pack all the remaining arguments into an array.
                            if (parameters[ i ].HasAttribute (typeof (ParamArrayAttribute)) && !arguments [ i ].GetType ().IsArray) {
                                Type elementType = parameters[ i ].type.GetElementType ();

                                // Since lists are easier to work with, but not quite straightforward to create dynamically, do this.
                                dynamic dyn = Activator.CreateInstance (typeof (List<>).MakeGenericType (elementType));
                                for (int j = i; j < arguments.Length; j++) {
                                    TryAddToParams (ref dyn, arguments [ j ], elementType);
                                }

                                arg = dyn.ToArray ();
                            }

                            TryAddToParams (ref argumentList, arg, parameters[ i ].type);
                        } catch {
                            isMethod = false;
                            argumentList.Clear ();
                            break;
                        }
                    }
                }

                if (isMethod) {
                    return new FindMethodResult (overload, argumentList);
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
                    object obj = Convert.ChangeType (toConvert, type);
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
            string executionError = AllowExecution (data);
            string executionPrefix = "Failed to execute command " + Name;
            if (executionError == "") {

                FindMethodResult result = FindMethod (arguments);
                if (result != null) {
                    try {

                        result.convertedArguments.Insert (0, data);

                        Result task = await result.overload.ExecuteOverload (result.convertedArguments.ToArray ());
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

        public virtual string AllowExecution (CommandMetadata metadata) {

            if (metadata.Message.Id == 0) // If it is a fake message, then just continue.
                return "";

            string errors = string.Empty;

            if (CommandEnabled == false)
                errors += "\n\tNot enabled on this server.";

            if (!AvailableInDM && metadata.Message.Channel as SocketDMChannel != null) {
                errors += "\n\tNot available in DM channels.";
            }

            if (!(metadata.Message.Author as SocketGuildUser).HasAllPermissios (RequiredPermissions)) {
                errors += "\n\tUser does not have permission.";
            }

            if (!AvailableOnServer && metadata.Message.Channel as SocketGuildChannel != null) {
                errors += "\n\tNot avaiable on server.";
            }

            return errors;
        }

        public virtual void Initialize () {
            CheckDescriptionLength ();
        }

        public virtual string GetCommand(ulong? owner) {
            return this.GetPrefix (owner) + Name;
        }

        private static string GetParenthesesArgs(string input) {
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

        public string Format (ulong? owner, string connector = " | ", int minSpaces = 25) {
            return StringExtensions.UniformStrings (GetCommand (owner), Description, " | ");
        }

        public class Overload {

            public string returnType;
            public string description;

            public Overload(Type _returnType, string _description) {
                returnType = _returnType.Name;
                description = _description;
            }
        }

        public CommandOverload[] GetOverloads () {

            var overloads = GetType ().GetMethods ().Where (x => x.IsDefined (typeof (OverloadAttribute))).ToArray ();
            List<CommandOverload> result = new List<CommandOverload> ();

            foreach (MethodInfo info in overloads) {

                var parameters = new List<CommandOverload.Parameter> ();
                ParameterInfo[] paramInfos = info.GetParameters ();

                for (int i = 1; i < paramInfos.Length; i++) {

                    ParameterInfo paramInfo = paramInfos[i];
                    var parameter = new CommandOverload.Parameter (paramInfo.Name, paramInfo.ParameterType, paramInfo.GetCustomAttributes ().ToArray ());
                    parameters.Add (parameter);

                }

                OverloadAttribute overloadAttribute = info.GetCustomAttribute<OverloadAttribute> ();
                ExampleAttribute exampleAttribute = info.GetCustomAttribute<ExampleAttribute> ();

                result.Add (new CommandOverload (
                    overloadAttribute.ReturnType,
                    parameters.ToArray (),
                    overloadAttribute.Description,
                    new CommandOverload.ExampleInfo (exampleAttribute),
                    new Func<object[], Task<Result>> (x => info.Invoke (this, x) as Task<Result>)
                    ));
            }

            return result.ToArray ();
        }

        public override string ToString() => Name;

        private void CheckDescriptionLength () {
            if (Description.Length > ListEmbedDescriptionMaxWidth)
                Console.WriteLine ("Description for command " + Name + " is longer than recommended, consider shortening it to less than " + ListEmbedDescriptionMaxWidth);
        }

        public bool IsCommand(string name) {
            if (name == Name)
                return true;

            foreach (string alias in Aliases)
                if (alias == name)
                    return true;

            return false;
        }

        public virtual Embed GetDocumentationEmbed(CommandMetadata metadata) => this.GetHelpEmbed (metadata.Message);
    }
}
