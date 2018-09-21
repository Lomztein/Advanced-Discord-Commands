using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public interface INamed
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}
