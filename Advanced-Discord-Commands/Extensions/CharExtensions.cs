using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using Lomztein.AdvDiscordCommands.Framework.Execution;

namespace Lomztein.AdvDiscordCommands.Extensions
{
    public static class CharExtensions
    {
        public static readonly char[] WhitespaceChars = { '\n', '\t', ' ' };

        public static bool IsWhitespace (this char character) {
            return WhitespaceChars.Contains (character);
        }

        public static bool IsCommandTrigger (this char character, ulong? owner, ISearcher searcher, out bool isHidden) {
            isHidden = false;

            if (character == searcher.GetTrigger (owner))
                return true;

            if (character == searcher.GetHiddenTrigger (owner)) {
                isHidden = true;
                return true;
            }

            return false;
        }

        public static bool IsCommandTrigger (this char character, ulong? owner, ISearcher searcher) {
            return character.IsCommandTrigger (owner, searcher, out _);
        }
    }
}
