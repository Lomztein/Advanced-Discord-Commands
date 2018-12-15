using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public abstract class CommandBase : ICommand
    {
        public string[] Aliases { get; set; } = new string[0];
        public string Flatname { get; set; }
        public ICategory Category { get; set; } = StandardCategories.Uncategorised;

        public bool CommandEnabled { get; set; } = true;
        public bool AvailableInDM { get; set; } = false;
        public bool AvailableOnServer { get; set; } = true;

        public List<GuildPermission> RequiredPermissions { get; set; } = new List<GuildPermission>();
        public ICommandParent CommandParent { get; set; }

        public string Name { get; set; } = "unnamed";
        public string Description { get; set; } = "undesced";

        public virtual string AllowExecution(CommandMetadata metadata)
        {
            if (metadata.Message.Id == 0) // If it is a fake message, then just continue.
                return "";

            string errors = string.Empty;

            if (CommandEnabled == false)
                errors += "\n\tNot enabled on this server.";

            if (!AvailableInDM && metadata.Message.Channel as SocketDMChannel != null)
            {
                errors += "\n\tNot available in DM channels.";
            }

            if (!(metadata.Message.Author as SocketGuildUser).HasAllPermissios(RequiredPermissions))
            {
                errors += "\n\tUser does not have permission.";
            }

            if (!AvailableOnServer && metadata.Message.Channel as SocketGuildChannel != null)
            {
                errors += "\n\tNot avaiable on server.";
            }

            return errors;
        }

        public virtual string GetCommand(ulong? owner)
        {
            return this.GetPrefix (owner) + Name;
        }

        public abstract Embed GetDocumentationEmbed(CommandMetadata metadata);
        public abstract CommandOverload[] GetOverloads();
        public abstract void Initialize();

        public virtual bool IsCommand(string name)
        {
            return (Name == name || Aliases.Contains(name)) ||
                (!string.IsNullOrEmpty (Flatname) && Flatname == name);
        }

        public abstract Task<Result> TryExecute(CommandMetadata data, params object[] arguments);
    }
}
