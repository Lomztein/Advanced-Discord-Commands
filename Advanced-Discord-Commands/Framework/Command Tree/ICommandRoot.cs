using Discord;
using Lomztein.AdvDiscordCommands.Framework.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public interface ICommandRoot : ICommandSet, ICommandParent
    {
        Task<Result> EnterCommand(string fullCommand, IMessage message, ulong? owner);
    }
}
