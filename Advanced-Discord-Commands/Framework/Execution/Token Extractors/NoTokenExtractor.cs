﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenExtractors
{
    public class NoTokenExtractor : ITokenExtractor
    {
        public object[] ExtractTokens(string input)
        {
            return string.IsNullOrWhiteSpace (input) ? new object[0] : new[] { input };
        }
    }
}
