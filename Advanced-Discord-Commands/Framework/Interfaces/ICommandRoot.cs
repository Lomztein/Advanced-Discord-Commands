using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework.Interfaces
{
    public interface ICommandRoot : ICommandSet, ICommandParent
    {
        Task<Result> EnterCommand(string input, IUserMessage userMessage);
    }
}
