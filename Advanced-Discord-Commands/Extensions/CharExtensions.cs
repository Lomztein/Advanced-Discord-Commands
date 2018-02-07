using System;
using System.Collections.Generic;
using System.Text;
using Lomztein.AdvDiscordCommands.Framework;

namespace Lomztein.AdvDiscordCommands.Extensions
{
    public static class CharExtensions
    {
        public static bool IsCommandTrigger (this char character) {
            return character == Command.commandTrigger;
        }
    }
}
