﻿using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Extensions
{
    public static class MessageExtensions
    {
        public static SocketGuild GetGuild (this IMessage message) {
            return (message.Channel as SocketGuildChannel)?.Guild;
        }
    }
}
