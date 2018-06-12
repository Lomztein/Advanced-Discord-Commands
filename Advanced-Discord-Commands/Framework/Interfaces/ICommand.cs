using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Framework.Categories;

namespace Lomztein.AdvDiscordCommands.Framework.Interfaces {
    public interface ICommand : INamed {

        string[] Aliases { get; set; }

        ICategory Category { get; set; }

        void Initialize();

        string GetCommand();

        Embed GetHelpEmbed(IMessage e, bool advanced);
        string Format(string connector = " | ", int minSpaces = 25);

        Task<Command.Result> TryExecute(CommandMetadata data, params object[] arguments);
        string AllowExecution(IMessage e);
    }
}