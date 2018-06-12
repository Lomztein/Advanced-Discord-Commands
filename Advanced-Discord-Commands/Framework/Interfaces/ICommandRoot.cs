using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Interfaces
{
    public interface ICommandRoot : ICommandParent
    {
        char Trigger { get; set; }
        char HiddenTrigger { get; set; }
    }
}
