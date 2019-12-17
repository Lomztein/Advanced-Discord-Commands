using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenExtractors
{
    public interface ITokenExtractor
    {
        object[] ExtractTokens (string input);

        
    }
}
