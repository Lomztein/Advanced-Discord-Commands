using Discord;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Autodocumentation {

    public static class CommandListAutodocumentation {

        public static Embed ListCommands (this ICommandSet set, CommandMetadata data) {
            return ListCommands (data, set);
        }

        public static Embed ListCommands(CommandMetadata data, params ICommandSet[] sets) {
            List<ICommand> combined = new List<ICommand> ();
            foreach (ICommandSet set in sets)
                combined.AddRange (set.GetCommands ());
            return ListCommands (data, sets.First ().Name, sets.First ().Description, combined.ToArray ());
        }

        public static Embed ListCommands(CommandMetadata data, string name, string description, params ICommand[] commands) {
            // Display all commands within command.
            EmbedBuilder builder = new EmbedBuilder ();

            builder.WithTitle ("Commands within the " + name + " set of commands.");
            builder.WithDescription (description);
            builder.WithCurrentTimestamp ();

            List<ICommand> withoutDublicates = new List<ICommand> ();
            foreach (ICommand cmd in commands) {
                if (!withoutDublicates.Exists (x => x.Name == cmd.Name))
                    withoutDublicates.Add (cmd);
            }

            var categories = withoutDublicates.Where (x => x.AllowExecution (data) == "").GroupBy (x => x.Category);

            foreach (var category in categories) {

                string fieldCommands = "```";
                foreach (var item in category) {
                    fieldCommands += StringExtensions.UniformStrings (item.GetCommand (data.Owner), item.Description) + "\n";
                }
                fieldCommands += "```\u200b";

                builder.AddField ("**" + category.Key.Name + "** - " + category.Key.Description, fieldCommands);

            }

            return builder.Build ();

        }
    }
}
