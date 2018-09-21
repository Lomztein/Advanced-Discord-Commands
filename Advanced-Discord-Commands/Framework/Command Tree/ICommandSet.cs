using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public interface ICommandSet : INamed
    {
        List<ICommand> GetCommands ();

        void AddCommands(params ICommand[] newCommands);

        void RemoveCommands(params ICommand[] commands);
    }
}
