using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution
{
    public interface IExtractor
    {
        string[] ExtractArguments (string input);
    }
}
