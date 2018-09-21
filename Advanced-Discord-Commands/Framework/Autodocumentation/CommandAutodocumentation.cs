using Discord;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Execution;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Autodocumentation
{
    public static class CommandAutodocumentation
    {

        private static readonly string[] SuperscriptNumbers = new string[] { "⁰", "¹", "²", "³", "⁴", "⁵", "⁶", "⁷", "⁸", "⁹" };
        private static readonly string SuperscriptArrayIndicator = "ᴬ";

        public static Embed GetAutodocumentationEmbed(this ICommand command, IMessage e, bool advanced) {
            EmbedBuilder builder = new EmbedBuilder ();

            builder.WithTitle ($"Command \"{command.GetPrefix (e.GetGuild ().Id)}{command.Name}\"")
                .WithDescription (command.Description);

            // Compile a list of all C# types used, in order to explain them to the layman.
            List<Type> shownTypes = new List<Type> ();
            bool containsArray = false;

            string AddAndGetShownTypeName(Type type) {
                if (shownTypes.Contains (type))
                    return type.Name + SuperscriptNumbers[shownTypes.IndexOf (type) + 1];
                else {
                    if (type.IsArray) {
                        type = type.GetElementType ();
                        containsArray = true;
                        return AddAndGetShownTypeName (type) + "[]" + SuperscriptArrayIndicator;
                    }
                    shownTypes.Add (type);
                }
                return type.Name + SuperscriptNumbers[shownTypes.Count];
            }

            // This is quite similar to GetArgs and GetHelp together, and the other functions are obsolete due to this.
            CommandOverload[] overloads = command.GetOverloads ();

            for (int i = 0; i < overloads.Length; i++) {

                CommandOverload overload = overloads[i];

                string overloadText = "";

                // Add overload description.
                overloadText += "*" + overload.Description + "*\n\n";

                // Add "Syntax".
                overloadText += "** -- Usage -- **";

                // Start codeblock for syntax.
                overloadText += "```";

                // If return type is not void, then add return type.
                if (overload.ReturnType != typeof (void))
                    overloadText += "Returns:   " + AddAndGetShownTypeName (overload.ReturnType) + "\n";

                // Add arguments.
                overloadText += "Arguments: ";
                for (int j = 0; j < overload.Parameters.Length; j++) {
                    CommandOverload.Parameter parameter = overload.Parameters[j];

                    overloadText += AddAndGetShownTypeName (parameter.type) + " " + parameter.name;

                    // Add arg seperator between each argument to seperate.
                    if (j != overload.Parameters.Length - 1)
                        overloadText += DefaultExtractor.argSeperator + " ";
                }
                // End syntax code block
                overloadText += "```";

                if (i != overloads.Length - 1)
                    overloadText += "\n";

                // Add example, if there is one.
                if (!overload.Example.IsEmpty) {

                    // Add "Example" header.
                    overloadText += "** -- Example Usage -- **";

                    // Add example codeblock.
                    overloadText += "```" + overload.GetExample (command, e.GetGuild ()?.Id) + "```\n";

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
            string laymansText = "";
            for (int i = 0; i < shownTypes.Count; i++) {

                Type type = shownTypes[i];
                string indexText = (i + 1).ToString ();

                laymansText += "**" + indexText + " - " + type.Name + "** - *" + TypeDescriptions.GetDescription (type) + "*";
                if (i != shownTypes.Count - 1)
                    laymansText += "\n\n\u200b";
            }

            if (containsArray)
                laymansText += "\n\n\u200b**A - Array** - " + TypeDescriptions.ArrayDescription;

            builder.AddField ("Types in laymans terms", laymansText);

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
