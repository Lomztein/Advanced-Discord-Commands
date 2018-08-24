using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.Extensions
{
    public static class CharExtensions
    {
        public static readonly char[] WhitespaceChars = { '\n', '\t', ' ' };

        public static bool IsWhitespace (this char character) {
            return WhitespaceChars.Contains (character);
        }

        public static bool IsCommandTrigger (this char character, ulong? owner, IExecutor executor, out bool isHidden) {
            isHidden = false;

            if (character == executor.GetTrigger (owner))
                return true;

            if (character == executor.GetTrigger (owner)) {
                isHidden = true;
                return true;
            }

            return false;
        }

        public static bool IsCommandTrigger (this char character, ulong? owner, IExecutor executor) {
            return character.IsCommandTrigger (owner, executor, out bool isHidden);
        }
    }
}
