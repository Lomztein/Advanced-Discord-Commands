using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution
{
    public interface ISplitter
    {
        string[] SplitMultiline(string input);
    }
}
