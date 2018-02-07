using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Extensions
{
    public static class StringExtensions
    {
        public static IMentionable ExtractMentionable(this string mention, SocketGuild guild) {
            int start = mention.IndexOf ('<');
            int end = mention.IndexOf ('>');

            string extracted = mention.Substring (start, end - start);

            try {
                switch (extracted) {

                    case var ext when ext.Substring (0, 3) == "<@&": // Role mentions
                        ulong roleID = ulong.Parse (extracted.Substring (3));
                        return guild.GetRole (roleID);

                    case var ext when ext.Substring (0, 3) == "<@!": // Nickname mentions
                        return guild.GetUser (ulong.Parse (ext.Substring (3)));

                    case var ext when ext.Substring (0, 2) == "<@": // Username mentions
                        return guild.GetUser (ulong.Parse (ext.Substring (2)));

                    case var ext when ext.Substring (0, 2) == "<#": // Channel mentions
                        return guild.GetTextChannel (ulong.Parse (ext.Substring (2)));

                }
            } catch (Exception e) {
                Logging.Log (e);
                return null;
            }

            return null;
        }
    }
}
