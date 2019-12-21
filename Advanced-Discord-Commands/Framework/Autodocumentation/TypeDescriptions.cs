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
            { typeof (Embed), "Embed" },
            { typeof (IUser), "User." },
            { typeof (IUserMessage), "Message" },
            { typeof (IMessage), "Message" },
            { typeof (IMentionable), "Mentionable" },
            { typeof (IDeletable), "Deletable" },
            { typeof (ICommand), "Command" },
            { typeof (ICommandSet), "Command Set" }
        };

        private static readonly Dictionary<Type, string> _descriptions = new Dictionary<Type, string>
        {
            { typeof (string), "This is a piece of text, such as what you are reading here." },
            { typeof (bool), "This is a true-or-false value, that takes in either true or false." },
            { typeof (short), "This is an integer number, which is a number that with no decimal points. This variant cannot represent very large numbers." },
            { typeof (int), "This is an integer number, which is a number that with no decimal points." },
            { typeof (long), "This is an integer number, which is a number that with no decimal points. This specific variant can represent very large numbers." },
            { typeof (float), "This is a decimal number, which means that it is a number with a decimal point." },
            { typeof (double), "This is a decimal number, which means that it is a number with a decimal point. This specific variant is highly accurate." },
            { typeof (object), "This can represent litteraly anything." },
            { typeof (SocketGuildUser), "This represents a [Discord.WebSocket.SocketUser] of a specific server. A mention of the user can be used here." },
            { typeof (SocketUser), "This represents a specific user. A mention can also be used if you are on a server." },
            { typeof (SocketRole), "This represents to a role on a server. A mention can be used if the role is mentionable." },
            { typeof (SocketGuildChannel), "This represents a specific channel on a server. A mention can be used." },
            { typeof (SocketGuild), "This represents a particular Discord server." },
            { typeof (Embed), "This represents a Rich Embed, a large box of fancy formatted information, really." },
            { typeof (IUserMessage), "This represents a message on Discord that are specifically sent by users or bots." },
            { typeof (IMessage), "This represents any type of message on Discord, such as those you send to your friends." },
            { typeof (IUser), "This represents any type of Discord user, be it bot, system or person." },
            { typeof (ICommand), "This represents a single command." },
            { typeof (ICommandSet), "This represents a set of [Lomztein.AdvDiscordCommands.Framework.Interfaces.ICommand]s." },
            { typeof (IMentionable), "This represents anything in Discord that may be mentioned, such as users, channels and roles." },
            { typeof (IDeletable), "This represents anything that may be deleted, such as channels an roles." },
        };

        private static readonly Dictionary<GuildPermission, string> _permissionNames = new Dictionary<GuildPermission, string>
        {
            { GuildPermission.AddReactions, "Add Reactions" },
            { GuildPermission.Administrator, "Administrator" },
            { GuildPermission.AttachFiles, "Attach Files" },
            { GuildPermission.BanMembers, "Ban Members" },
            { GuildPermission.ChangeNickname, "Change Nickname" },
            { GuildPermission.Connect, "Connect" },
            { GuildPermission.CreateInstantInvite, "Create Invite" },
            { GuildPermission.DeafenMembers, "Deafen Members" },
            { GuildPermission.EmbedLinks, "Embed Links" },
            { GuildPermission.KickMembers, "Kick Members" },
            { GuildPermission.ManageChannels, "Manage Channels" },
            { GuildPermission.ManageEmojis, "Manage Emojis" },
            { GuildPermission.ManageGuild, "Manage Server" },
            { GuildPermission.ManageMessages, "Manage Messages" },
            { GuildPermission.ManageNicknames, "Manage Nicknames" },
            { GuildPermission.ManageRoles, "Manage Roles" },
            { GuildPermission.ManageWebhooks, "Manage Webhooks" },
            { GuildPermission.MentionEveryone, "Mention Everyone" },
            { GuildPermission.MoveMembers, "Move Members" },
            { GuildPermission.MuteMembers, "Mute Members" },
            { GuildPermission.PrioritySpeaker, "Priority Speaker" },
            { GuildPermission.ReadMessageHistory, "Read Message History" },
            { GuildPermission.SendMessages, "Send Messages" },
            { GuildPermission.SendTTSMessages, "Send TTS Messages" },
            { GuildPermission.Speak, "Speak" },
            { GuildPermission.UseExternalEmojis, "Use External Emojis" },
            { GuildPermission.UseVAD, "Use Voice Activity" },
            { GuildPermission.ViewAuditLog, "View Audit Log" },
            { GuildPermission.ViewChannel, "View Text Channels & See Voice Channels" },
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

        public static string GetPermissionName(GuildPermission permission) => _permissionNames.ContainsKey (permission) ? _permissionNames[permission] : permission.ToString ();

        public static void Add (Type type, string name, string desc)
        {
            AddName(type, name);
            AddDescription(type, desc);
        }
    }
}
