using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution
{
    public interface IExtractor
    {
        Arguments ExtractArguments (string input);
    }
}
