using Discord;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework.Execution
{
    public interface IExecutor
    {
        Task<Result> Execute (ExecutionData data);
    }
}
