using Lomztein.AdvDiscordCommands.Framework.Execution;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework
{

    public class CommandOverload {

        public Type ReturnType { get; private set; }
        public Parameter[] Parameters { get; private set; }
        public string Description { get; private set; }

        public Func<object[], Task<Result>> ExecuteOverload { get; private set; }

        public ExampleInfo Example { get; set; }

        public CommandOverload(Type returnType, Parameter[] parameters, string description, ExampleInfo exampleInfo, Func<object[], Task<Result>> executeOverload) {
            ReturnType = returnType;
            Parameters = parameters;
            Description = description;
            Example = exampleInfo;
            ExecuteOverload = executeOverload;
        }

        public string GetExample (ICommand cmd, ulong? owner) => Example?.GetExample (cmd, owner);

        public class Parameter {

            public string name;
            public Type type;
            public Attribute[] attributes = new Attribute[0];

            public Parameter(string _name, Type _type, Attribute[] _attributes) {
                name = _name;
                type = _type;
                attributes = _attributes;
            }

            public bool HasAttribute(Type attributeType) => attributes.Any (x => x.GetType () == attributeType);

            public override string ToString() => $"{type.Name} {name}";
        }

        public override string ToString() => $"{ExecuteOverload.Method.Name} ({string.Join(", ", Parameters.Select(x => x.ToString()))}) - {Description}";

        public class ExampleInfo {

            public string[] Arguments { get; private set; }

            public string Value { get; private set; }
            public string Message { get; private set; }

            public bool IsEmpty { get => Arguments == null && Value == null && Message == null; }

            public ExampleInfo (ExampleAttribute source) {
                Arguments = source?.Info.Arguments;
                Value = source?.Info.Value;
                Message = source?.Info.Message;
            }

            public ExampleInfo (string value, string message, params string[] arguments) {
                Value = value;
                Message = message;
                Arguments = arguments;
            }

            public string GetExample(ICommand cmd, ulong? owner) {
                string combinedArgs = "";
                for (int i = 0; i < Arguments.Length; i++) {
                    combinedArgs += Arguments[i];
                    if (i != Arguments.Length - 1)
                        combinedArgs += Framework.Arguments.SEPERATOR + " ";
                }

                return cmd.GetCommand (owner) + " " + combinedArgs;
            }

        }
    }
}
