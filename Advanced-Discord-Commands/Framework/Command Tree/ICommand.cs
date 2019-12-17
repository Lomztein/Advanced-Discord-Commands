using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Framework.Categories;

namespace Lomztein.AdvDiscordCommands.Framework.Interfaces {

    public interface ICommand : ICommandChild, INamed {

        string[] Aliases { get; set; }
        string Flatname { get; set; }

        ICategory Category { get; set; }

        void Initialize();

        string GetCommand(ulong? owner);

        Task<Result> TryExecute(CommandMetadata data, Arguments arguments);

        bool IsCommand(string name);

        CommandOverload[] GetOverloads();

        bool AvailableInDM { get; set; }
        bool AvailableOnServer { get; set; }
        bool CommandEnabled { get; set; }

        List<GuildPermission> RequiredPermissions { get; set; }

        Embed GetDocumentationEmbed(CommandMetadata metadata);

        string AllowExecution(CommandMetadata metadata);

    }
}