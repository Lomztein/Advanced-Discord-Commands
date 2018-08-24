using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework.Interfaces
{
    public interface IExecutor
    {
        Func<ulong, char> Trigger { get; set; }
        Func<ulong, char> HiddenTrigger { get; set; }

        char GetTrigger(ulong? id);
        char GetHiddenTrigger(ulong? id);

        string[] ParseArguments(string fullCommand);
        string[] ParseMultiline(string text);

        ICommand FindCommand(string fullCommand, List<ICommand> commandList, ulong? owner);

        Task<Result> Execute(Execution execution);
    }
}
