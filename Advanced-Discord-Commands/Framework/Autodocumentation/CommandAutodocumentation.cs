using Discord;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Execution;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Autodocumentation
{
    public static class CommandAutodocumentation
    {


        public static Embed GetAutodocumentationEmbed(this ICommand command, CommandMetadata metadata, bool advanced) {

            EmbedBuilder builder = new EmbedBuilder ();
            ulong? owner = metadata.Message.GetGuild()?.Id;

            builder.WithTitle ($"Command \"{command.GetCommand (owner)}\"")
                .WithDescription (command.Description);

            if (command.Aliases.Length > 0 || !string.IsNullOrEmpty(command.Shortcut))
            {
                string aliasText;
                aliasText = $"```{string.Join('\n', command.Aliases.Select(x => $"{command.GetPrefix(owner)}{x}"))}";
                if (!string.IsNullOrEmpty(command.Shortcut))
                {
                    string prefix = metadata.Root.GetChildPrefix(owner);
                    aliasText += $"\n{prefix}{command.Shortcut}\n{string.Join ('\n', command.ShortcutAliases.Select (x => $"{prefix}{x}"))}";
                }
                aliasText += "```\u200b";

                builder.AddField("Aliases", aliasText);
            }

            // This is quite similar to GetArgs and GetHelp together, and the other functions are obsolete due to this.
            CommandOverload[] overloads = command.GetOverloads ();

            TypeDescriptor descriptor = new TypeDescriptor(true); // TODO: Allow individual users to decide on laymanship.
            for (int i = 0; i < overloads.Length; i++) {

                CommandOverload overload = overloads[i];

                string overloadText = "";

                // Add overload description.
                overloadText += $"*{overload.Description}*\n\n";

                // Start codeblock for syntax.
                    StringBuilder ioText = new StringBuilder ();

                // Add arguments.
                if (overload.Parameters.Length > 0)
                {
                    ioText.Append("Arguments: ");
                    ioText.AppendLine(string.Join(", ", overload.Parameters.Select(x => $"{descriptor.GetName(x.type, true)} {x.name}")));
                }

                // If return type is not void, then add return type.
                if (overload.ReturnType != typeof(void))
                {
                    ioText.AppendLine("Returns:   " + descriptor.GetName(overload.ReturnType, true));
                }

                if (ioText.Length > 0)
                {
                    // Add "Syntax".
                    overloadText += $"** -- Usage -- **\n```{ioText.ToString ()}```\n";
                }
                
                // Add example, if there is one.
                if (overload.Example != null && !overload.Example.IsEmpty) {

                    // Add "Example" header.
                    overloadText += "** -- Example Usage -- **";

                    // Add example codeblock.
                    overloadText += "```" + overload.GetExample (command, owner) + "```\n";

                    // Add display value.
                    overloadText += "Displays: " + overload.Example.Message + "\n";

                    // Add return value if there is any.
                    overloadText += overload.Example.Value == null ? "" : "Returns: " + overload.Example.Value;

                    overloadText += "\n";
                }

                // Add seperator.
                overloadText += "\u200b";

                builder.AddField ("Command Variant " + (i + 1).ToString (), overloadText);
            }

            // Laymans terms field.
            string laymansText = "\n" + string.Join ("\n\n\u200b", descriptor.GetDescriptions());

            if (!string.IsNullOrWhiteSpace (laymansText) && laymansText.Length > 0)
            {
                builder.AddField("Type descriptions", laymansText);
            }

            string footer = string.Empty;
            if (command.AvailableInDM && !command.AvailableOnServer)
                footer += " - ONLY IN DM";
            if (command.AvailableInDM && command.AvailableOnServer)
                footer += " - AVAILABLE IN DM";

            if (command.RequiredPermissions.Count > 0) {
                footer += " - REQUIRES PERMISSIONS: ";
                for (int i = 0; i < command.RequiredPermissions.Count; i++) {
                    footer += command.RequiredPermissions[i].ToString ().ToUpper ();

                    if (i != command.RequiredPermissions.Count - 1)
                        footer += ", ";
                }
            }

            builder.WithFooter (footer);
            return builder.Build ();
        }
    }
}
