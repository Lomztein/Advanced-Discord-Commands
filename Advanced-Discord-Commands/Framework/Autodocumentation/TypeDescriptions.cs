using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Autodocumentation
{
    public static class TypeDescriptions
    {
        private static readonly Dictionary<Type, string> _laymansTypeNames = new Dictionary<Type, string>
        {
            { typeof (string), "Text" },
            { typeof (bool), "True/False" },
            { typeof (short), "Integer" },
            { typeof (int), "Integer" },
            { typeof (long), "Integer" },
            { typeof (ulong), "Positive Integer" },
            { typeof (float), "Decimal" },
            { typeof (double), "Decimal" },
            { typeof (SocketGuildUser), "Server User" },
            { typeof (SocketUser), "User" },
            { typeof (SocketRole), "Role" },
            { typeof (SocketGuildChannel), "Channel" },
            { typeof (SocketGuild), "Server" },
            { typeof (IUserMessage), "Message" },
            { typeof (IMessage), "Message" },
            { typeof (IMentionable), "Mentionable" },
            { typeof (IDeletable), "Deletable" },
            { typeof (ICommand), "Command" },
            { typeof (ICommandSet), "Command Set" }
        };

        private static readonly Dictionary<Type, string> _descriptions = new Dictionary<Type, string> {
            { typeof (string), "This is a piece of text, such as what you are reading here." },
            { typeof (bool), "This is a true-or-false value, that takes in either true or false." },
            { typeof (short), "This is an integer number, which is a number that with no decimal points. This variant cannot represent very large numbers." },
            { typeof (int), "This is an integer number, which is a number that with no decimal points." },
            { typeof (long), "This is an integer number, which is a number that with no decimal points. This specific variant can represent very large numbers." },
            { typeof (float), "This is a decimal number, which means that it is a number with a decimal point." },
            { typeof (double), "This is a decimal number, which means that it is a number with a decimal point. This specific variant is highly accurate." },
            { typeof (SocketGuildUser), "This object refers to a [Discord.WebSocket.SocketUser] of a specific server. A mention of the user can be used here." },
            { typeof (SocketUser), "This object refers to a user that is not connected to a server. A mention can also be used if you are on a server." },
            { typeof (SocketRole), "This object refers to a role on a server. A mention can be used if the role is mentionable." },
            { typeof (SocketGuildChannel), "This represents a specific channel on a server. A mention can be used." },
            { typeof (SocketGuild), "This represents a particular Discord server." },
            { typeof (IUserMessage), "This refers to messages on Discord that are specifically sent by users or bots." },
            { typeof (IMessage), "This refers to any type of message on Discord, such as those you send to your friends." },
            { typeof (IUser), "This represents any type of Discord user, be it bot, system or person." },
            { typeof (ICommand), "This represents a single command." },
            { typeof (ICommandSet), "This represents a set of [Lomztein.AdvDiscordCommands.Framework.Interfaces.ICommand]s." }
        };

        public static void AddName (Type type, string name)
        {
            if (_laymansTypeNames.ContainsKey (type))
            {
                _laymansTypeNames.Add(type, name);
            }
        }

        public static string GetName (Type type, bool layman)
        {
            if (layman && _laymansTypeNames.ContainsKey (type))
            {
                return _laymansTypeNames[type];
            }
            return type.Name;
        }

        public static void AddDescription (Type type, string description) {
            if (!_descriptions.ContainsKey (type))
                _descriptions.Add (type, description);
        }

        public static string GetDescription (Type type) {
            if (_descriptions.ContainsKey (type))
                return _descriptions[type];
            else
                return "No description exists for this type.";
        }
    }
}
