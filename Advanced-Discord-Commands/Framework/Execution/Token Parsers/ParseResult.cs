using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenParsers {
    public class ParseResult
    {
        public bool Success { get => Result != null; }
        public object Result { get; private set; }

        public ParseResult (object result) {
            Result = result;
        }

        public static Task<ParseResult> CreateTask (object result) {
            return Task.FromResult (new ParseResult (result));
        }
    }
}
