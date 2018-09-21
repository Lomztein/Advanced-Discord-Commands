using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public interface ICommandParent
    {
        string GetChildPrefix(ulong? ownerID);
    }
}
