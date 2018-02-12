using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Extensions
{
    public static class CommandExtensions
    {
        public static T GetCommand<T> (this ICommandSet commandSet) where T : Command {
            Command found = commandSet.GetCommands ().Find (x => x.GetType () == typeof (T));
            if (found != null)
                return (T)found;
            return null;
        }

        public static T SearchCommandRecursively<T> (this ICommandSet commandSet) where T : Command {
            T result = commandSet.GetCommand<T> ();
            if (result == null) {
                List<ICommandSet> internalSets = commandSet.GetCommands ().Where (x => x is ICommandSet).Cast<ICommandSet> ().ToList ();
                foreach (ICommandSet set in internalSets) {
                    result = set.GetCommand<T> ();
                    if (result != null)
                        return result;
                }
            }

            return null;
        }
    }
}
