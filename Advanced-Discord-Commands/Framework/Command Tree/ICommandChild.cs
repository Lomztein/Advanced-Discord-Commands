using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public interface ICommandChild
    {
        ICommandParent CommandParent { get; set; }
    }
}
