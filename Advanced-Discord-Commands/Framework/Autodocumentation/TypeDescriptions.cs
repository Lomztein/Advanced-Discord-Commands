using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Autodocumentation
{
    public static class TypeDescriptions
    {
        public static string ArrayDescription { get => "The `[]` characters means that this is an array of values of the type they are attached to, instead of a single value."; }

        private static Dictionary<Type, string> Descriptions { get; set; } = new Dictionary<Type, string> {
            { typeof (string), "This is a piece of text, such as what you are reading here." },
            { typeof (bool), "This is a true-or-false value, that takes in either true or false." },
            { typeof (short), "This is an integer number, which is a number that with no decimal points. This variant cannot count as high as the \"int\" variant." },
            { typeof (int), "This is an integer number, which is a number that with no decimal points." },
            { typeof (long), "This is an integer number, which is a number that with no decimal points. This variant can count much higher that the \"int\" variant." },
            { typeof (float), "This is a floating point number, which means that it is a number with a decimal point." },
            { typeof (double), "This is a floating point number, which means that it is a number with a decimal point. Compared to the \"float\" type, the \"double\" is much more accurate." },
            { typeof (SocketGuildUser), "This object refers to a member of a specific server. A mention of the user can be used here." },
            { typeof (SocketUser), "This object refers to a user that is not connected to a server. A mention can also be used if you are on a server." },
            { typeof (SocketRole), "This object refers to a role on a server. A mention can be used if the role is mentionable." },
            { typeof (SocketGuildChannel), "This object refers to a channel on a server. A mention can be used." },
            { typeof (SocketGuild), "This object refers to a Discord server." },
            { typeof (IUserMessage), "This refers to messages on Discord that are specifically sent by users or bots." },
            { typeof (IMessage), "This refers to any type of message on Discord, such as those you send to your friends." }
        };

        public static void AddDescription (Type type, string description) {
            if (!Descriptions.ContainsKey (type))
                Descriptions.Add (type, description);
        }

        public static string GetDescription (Type type) {
            if (Descriptions.ContainsKey (type))
                return Descriptions[type];
            else
                return "No description exists for this type.";
        }
    }
}
