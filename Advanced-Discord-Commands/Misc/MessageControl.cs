using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Misc
{

    public static class MessageControl
    {
        public async static Task<IUserMessage> SendMessage (IUser user, string text) {
            return await user.SendMessageAsync (text);
        }
    }
}
