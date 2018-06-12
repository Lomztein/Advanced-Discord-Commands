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
        public static T GetCommand<T> (this ICommandSet commandSet) where T : ICommand {
            ICommand found = commandSet.GetCommands ().Find (x => x.GetType () == typeof (T));
            if (found != null)
                return (T)found;
            return default(T);
        }

        public static T SearchCommandRecursively<T> (this ICommandSet commandSet) where T : ICommand {
            T result = commandSet.GetCommand<T> ();
            if (result == null) {
                List<ICommandSet> internalSets = commandSet.GetCommands ().Where (x => x is ICommandSet).Cast<ICommandSet> ().ToList ();
                foreach (ICommandSet set in internalSets) {
                    result = set.GetCommand<T> ();
                    if (result != null)
                        return result;
                }
            }

            return default(T);
        }

        public static string GetPrefix (this ICommandChild commandChild) {
            return commandChild.CommandParent.GetChildPrefix ();
        }

        public static bool IsCommandTrigger (this ICommandRoot root, char character, out bool isHidden) {
            isHidden = false;

            if (character == root.Trigger) {
                isHidden = false;
                return true;
            }

            if (character == root.HiddenTrigger) {
                isHidden = true;
                return true;
            }

            return false;
        }
    }
}
