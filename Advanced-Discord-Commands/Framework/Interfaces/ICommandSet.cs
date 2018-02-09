﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Interfaces
{
    interface ICommandSet
    {
        List<Command> GetCommands();

        void AddCommands(params Command[] newCommands);

        void RemoveCommands(params Command[] commands);
    }
}