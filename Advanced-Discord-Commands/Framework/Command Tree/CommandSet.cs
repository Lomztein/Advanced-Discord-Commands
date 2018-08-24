using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Autodocumentation;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.Framework {

    public abstract class CommandSet : Command, ICommandSet, ICommandParent {

        public List<ICommand> commandsInSet = new List<ICommand>();

        public CommandSet() {
            Name = "commandset";
            Description = "DEFAULT_COMMAND_SET";
            Category = StandardCategories.Miscilaneous;
        }

        public override void Initialize() {
            base.Initialize ();
            InitCommands ();
        }

        public void InitCommands() {
            foreach (Command c in commandsInSet) {
                c.CommandParent = this;
                c.ParentRoot = ParentRoot;
                InitializeCommand (c);
            }
        }

        // The parsing is currently very reliant on how the rest of the code works, I'd like to change that somehow.
        public override async Task<Result> TryExecute (CommandMetadata data, params object[] arguments) {
            // Standard command format is !command arg1;arg2;arg3
            // Commandset format is !command secondaryCommand arg1;arg2;arg3
            // Would it be possible to have commandSets within commandSets?
            if (arguments.Length != 0) {

                string zeroArgString = arguments[0].ToString ();
                int spaceIndex = zeroArgString.IndexOfAny (CharExtensions.WhitespaceChars);

                string command = zeroArgString.Substring (0, spaceIndex != -1 ? spaceIndex : zeroArgString.Length).Trim ();
                List<object> newArgs = new List<object> ();

                if (spaceIndex != -1) {
                    newArgs.Add (zeroArgString.Substring (spaceIndex + 1).Trim (CharExtensions.WhitespaceChars));
                }

                for (int i = 1; i < arguments.Length; i++)
                    newArgs.Add (arguments[i]);
                arguments = newArgs.ToArray ();

                command = data.Executor.GetTrigger (data.Message.GetGuild ()?.Id) + command;

                Execution execution = data.Executor.CreateExecution (data, command, arguments, commandsInSet);
                var result = await data.Executor.Execute (execution);

                return result;
            } else {
                return new Result (this, "");
            }
        }

        public override string GetCommand(ulong? owner) {
            return this.GetPrefix (owner) + Name + " (set)";
        }

        public void AddCommands(params ICommand [ ] procCommands) {
            commandsInSet.AddRange (procCommands);

            foreach (ICommand c in procCommands) {
                InitializeCommand (c); // Make sure the commands metadata is set to fit the rest of the set.
            }
        }

        private void InitializeCommand (ICommand c) {
            c.Initialize ();
        }

        public void RemoveCommands(params ICommand[] commands) {
            foreach (ICommand cmd in commands) {
                commandsInSet.Remove (cmd);
            }
        }

        public List<ICommand> GetCommands() {
            return commandsInSet;
        }

        public string GetChildPrefix(ulong? owner) => this.GetPrefix (owner) + Name + " ";

        public override Embed GetDocumentationEmbed(CommandMetadata metadata) => this.ListCommands (metadata);

    }
}
