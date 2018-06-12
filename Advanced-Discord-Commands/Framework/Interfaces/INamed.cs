using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Interfaces
{
    public interface INamed
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}
