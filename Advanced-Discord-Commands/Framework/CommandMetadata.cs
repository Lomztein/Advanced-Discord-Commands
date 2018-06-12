using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public class CommandMetadata
    {
        public IMessage message;
        public CommandRoot root;
        public int depth = 0;
        public uint programCounter = 0;

        public CommandMetadata (IMessage _message, CommandRoot _root) {
            message = _message;
            root = _root;
        }

        public static implicit operator CommandRoot (CommandMetadata data) {
            return data.root;
        }
    }
}
