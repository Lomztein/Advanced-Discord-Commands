using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public class CommandMetadata
    {
        public SocketUserMessage message;
        public CommandRoot root;
        public int depth = 0;

        public CommandMetadata (SocketUserMessage _message, CommandRoot _root) {
            message = _message;
            root = _root;
        }

        public static implicit operator SocketUserMessage (CommandMetadata data) {
            return data.message;
        }

        public static implicit operator CommandRoot (CommandMetadata data) {
            return data.root;
        }
    }
}
