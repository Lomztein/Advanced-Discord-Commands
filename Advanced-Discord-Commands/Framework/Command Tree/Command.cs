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

    public abstract class Command : CommandBase, ICommandChild {

        private const int ListEmbedDescriptionMaxWidth = 28;

        private class FindMethodResult {

            public enum Fit { None, StringToString, Converted, PartiallyConverted, Perfect }
            public readonly Fit Fits;
            public readonly CommandOverload Overload;
            public readonly List<object> Arguments;

            public FindMethodResult(CommandOverload _overload, List<object> _parameters, Fit fit) {
                Overload = _overload;
                Arguments = _parameters;
                Fits = fit;
            }

            public override string ToString() => $"{Overload}, Arguments: {Arguments.Count}, Fit: {Fits}";
        }

        private FindMethodResult FindMethod(Arguments arguments) {
            CommandOverload [ ] overloads = GetOverloads ();
            List<FindMethodResult> availableMethods = new List<FindMethodResult>();

            foreach (object[] set in arguments)
            {
                foreach (CommandOverload overload in overloads)
                {
                    FindMethodResult result = FitMethod(overload, set);
                    if (result != null)
                    {
                        availableMethods.Add(result);
                    }
                }

                availableMethods.Sort(Comparer<FindMethodResult>.Create((x, y) => (int)y.Fits - (int)x.Fits));
                if (availableMethods.Any ())
                {
                    return availableMethods.First();
                }
            }
            return null;
        }

        private FindMethodResult FitMethod (CommandOverload overload, object[] arguments)
        {
            var parameters = overload.Parameters;
            bool anyParams = parameters.Any (x => x.HasAttribute (typeof (ParamArrayAttribute)));
            bool isMethod = parameters.Length == arguments.Length || (anyParams && arguments.Length >= parameters.Length); // Have to off-by-one since all commands gets the CommandMetadata parsed through.

            dynamic argumentList = new List<object>();
            List<ConvertSuccess> converted = new List<ConvertSuccess>();
            
            if (isMethod == true)
            {
                // Go through all parameters except the first one.
                for (int i = 0; i < parameters.Length; i++)
                {
                    try
                    {
                        object arg = arguments[i];

                        // Is the parameter given the params attribute? If so then pack all the remaining arguments into an array.
                        if (parameters[i].HasAttribute(typeof(ParamArrayAttribute)) && !arguments[i].GetType().IsArray)
                        {
                            Type elementType = parameters[i].type.GetElementType();

                            // Since lists are easier to work with, but not quite straightforward to create dynamically, do this.
                            dynamic dyn = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                            for (int j = i; j < arguments.Length; j++)
                            {
                                TryAddToParams(ref dyn, arguments[j], elementType);
                            }

                            arg = dyn.ToArray();
                        }

                        converted.Add (TryAddToParams(ref argumentList, arg, parameters[i].type));
                    }
                    catch
                    {
                        isMethod = false;
                        argumentList.Clear();
                        break;
                    }
                }
            }

            if (isMethod)
            {
                return new FindMethodResult(overload, argumentList, SelectFit (converted));
            }
            return null;
        }

        private enum ConvertSuccess { Converted, StringToString, NoConvert }
        /// <summary>
        /// Attempts to convert the given arg to the given type, and adds it to a list of parameters.
        /// </summary>
        /// <param name="paramList"></param>
        /// <param name="arg"></param>
        /// <param name="type"></param>
        /// <returns>True if value was converted, false if value wasn't converted</returns>
        private ConvertSuccess TryAddToParams(ref dynamic paramList, object arg, Type type) {
            Type inType = arg.GetType();
            
            dynamic result = TryConvert (arg, type);
            paramList.Add (result);

            if (inType == typeof(string))
            {
                return result.GetType() == arg.GetType() ? ConvertSuccess.StringToString : ConvertSuccess.NoConvert;
            }
            else
            {
                return result.GetType() != arg.GetType() ? ConvertSuccess.Converted : ConvertSuccess.NoConvert;
            }
        }

        private FindMethodResult.Fit SelectFit(IEnumerable<ConvertSuccess> converted)
        {
            if (converted.All(x => x == ConvertSuccess.NoConvert))
            {
                return FindMethodResult.Fit.Perfect;
            }
            if (converted.All(x => x == ConvertSuccess.Converted))
            {
                return FindMethodResult.Fit.Converted;
            }
            if (converted.All(x => x == ConvertSuccess.StringToString))
            {
                return FindMethodResult.Fit.StringToString;
            }
            return FindMethodResult.Fit.PartiallyConverted;
        }

        private object TryConvert(object toConvert, Type type) {
            if (toConvert == null)
            {
                return null;
            }

            if (type.IsInstanceOfType(toConvert))
            {
                return toConvert;
            }
            try
            {
                object obj = Convert.ChangeType(toConvert, type);
                return obj;
            }
            catch
            {
                return null;
            }
        }

        public override async Task<Result> TryExecute(CommandMetadata data, Arguments arguments) {
            string executionError = AllowExecution (data);
            string executionPrefix = $"Failed to execute command '{GetCommand ((data.Author as IUser)?.Id)}':";
            if (executionError == "") {

                FindMethodResult result = FindMethod (arguments);
                if (result != null) {
                    try {
                        result.Arguments.Insert (0, data);
                        Result task = await result.Overload.ExecuteOverload (result.Arguments.ToArray ());
                        return task;

                    } catch (TargetInvocationException exception) {
                        throw exception.InnerException;
                    }
                } else {
                    return new Result (GetDocumentationEmbed (data), $"{executionPrefix}\n\tNo suitable command variant found. This may help you:", true);
                }
            } else {
                return new Result (null, $"{executionPrefix} {executionError}", true);
            }
        }

        public override void Initialize () {
            CheckDescriptionLength ();
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

        public override CommandOverload[] GetOverloads () {

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

        public override Embed GetDocumentationEmbed(CommandMetadata metadata) => this.GetHelpEmbed (metadata);
    }
}
