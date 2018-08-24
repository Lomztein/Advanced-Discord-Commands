using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework {

    [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = false)]

    public class OverloadAttribute : Attribute {
        public Type ReturnType;
        public string Description;

        public OverloadAttribute(Type _returnType, string _description) {
            ReturnType = _returnType;
            Description = _description;
        }
    }
}