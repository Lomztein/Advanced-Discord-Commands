using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Extensions {

    public class RoleComparer : IComparer<IRole> {

        public int Compare(IRole x, IRole y) {
            return x.Position - y.Position;
        }
    }
}
