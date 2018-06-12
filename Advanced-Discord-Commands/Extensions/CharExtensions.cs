using System;
using System.Collections.Generic;
using System.Text;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.Extensions
{
    public static class CharExtensions
    {
        public static bool IsCommandTrigger (this char character, ICommandRoot commandRoot, out bool isHidden) {
            return commandRoot.IsCommandTrigger (character, out isHidden);
        }

        public static bool IsCommandTrigger (this char character, ICommandRoot commandRoot) {
            return commandRoot.IsCommandTrigger (character, out bool isHidden);
        }
    }
}
