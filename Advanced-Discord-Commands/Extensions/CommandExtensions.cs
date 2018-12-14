using Discord;
using Lomztein.AdvDiscordCommands.Autodocumentation;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Extensions
{
    public static class CommandExtensions {
        public static T GetCommand<T>(this ICommandSet commandSet) where T : ICommand {
            ICommand found = commandSet.GetCommands ().Find (x => x.GetType () == typeof (T));
            if (found != null)
                return (T)found;
            return default (T);
        }

        public static T GetCommandRecursive<T>(this ICommandSet commandSet) where T : ICommand {
            T result = commandSet.GetCommand<T> ();
            if (result == null) {
                List<ICommandSet> internalSets = commandSet.GetCommands ().Where (x => x is ICommandSet).Cast<ICommandSet> ().ToList ();
                foreach (ICommandSet set in internalSets) {
                    result = set.GetCommandRecursive<T> ();
                    if (result != null)
                        return result;
                }
            }

            return default (T);
        }

        public static List<ICommand> GetAllRecursive (this ICommandSet commandSet) {
            List<ICommand> found = new List<ICommand> ();
            foreach (ICommand cmd in commandSet.GetCommands ()) {
                found.Add (cmd);
                if (cmd is ICommandSet set) {
                    found.AddRange (GetAllRecursive (set));
                }
            }
            return found;
        }

        public static string GetPrefix(this ICommandChild commandChild, ulong? owner) {
            return commandChild.CommandParent.GetChildPrefix (owner);
        }

        public static Embed GetHelpEmbed (this ICommand command, IMessage message) {
            return CommandAutodocumentation.GetAutodocumentationEmbed (command, message, false);
        }

    }
}
