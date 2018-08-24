using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework {

    [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExampleAttribute : Attribute {

        // This is arguably a bit spaghetti, perhaps try to isolate this information elsewhere.
        public CommandOverload.ExampleInfo Info { get; private set; }

        public ExampleAttribute(string _value, string _message, params string[] _arguments) {
            Info = new CommandOverload.ExampleInfo (_value, _message, _arguments);
        }

        public ExampleAttribute(string _value, params string[] _arguments) : this (_value, _value, _arguments) { }
    }
}