using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
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

        public override async Task<Result> TryExecute (CommandMetadata data, params object[] arguments) {
            // Standard command format is !command arg1;arg2;arg3
            // Commandset format is !command secondaryCommand arg1;arg2;arg3
            // Would it be possible to have commandSets within commandSets?
            if (arguments.Length != 0) {

                string combinedArgs = "";
                for (int i = 0; i < arguments.Length; i++) {
                    combinedArgs += arguments [ i ];
                    if (i != arguments.Length - 1)
                        combinedArgs += CommandRoot.argSeperator;
                }

                combinedArgs = ParentRoot.Trigger + combinedArgs;
                var result = await CommandRoot.FindAndExecuteCommand (data, combinedArgs, commandsInSet);
                return result?.result;
            } else {
                return new Result (this, "");
            }
        }

        public override string GetCommand() {
            return this.GetPrefix () + Name + " (set)";
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

        public string GetChildPrefix() => this.GetPrefix () + Name + " ";
    }
}
