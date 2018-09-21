using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenParsers {

    public interface ITokenParser
    {
        Task<ParseResult> TryParse(object input, CommandMetadata metadata);
    }
}
