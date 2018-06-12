using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Interfaces
{
    public interface ICommandSet : INamed
    {
        List<ICommand> GetCommands ();

        void AddCommands(params ICommand[] newCommands);

        void RemoveCommands(params ICommand[] commands);
    }
}
