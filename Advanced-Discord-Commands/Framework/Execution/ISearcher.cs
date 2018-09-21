using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution
{
    public interface ISearcher
    {
        Func<ulong, char> Trigger { get; set; }
        Func<ulong, char> HiddenTrigger { get; set; }

        char GetTrigger(ulong? id);
        char GetHiddenTrigger(ulong? id);

        ICommand Search (string fullCommand, List<ICommand> commandList, ulong? owner);
    }
}
