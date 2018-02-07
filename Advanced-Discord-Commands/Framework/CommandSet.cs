using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.Framework {

    public abstract class CommandSet : Command, ICommandSet {

        protected List<Command> commandsInSet = new List<Command>();

        public CommandSet() {
            command = "commandset";
            shortHelp = "DEFAULT_COMMAND_SET";
            catagory = Category.Set;
        }

        public override void Initialize() {
            base.Initialize ();
            InitCommands ();
        }

        public void InitCommands() {
            foreach (Command c in commandsInSet) {
                FeedRecursiveData (c);
            }
        }

        public override async Task<Result> TryExecute (CommandMetadata data, int depth, params object[] arguments) {
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

                string cmd = "";

                List<object> newArguments = CommandRoot.ConstructArguments (combinedArgs, out cmd);
                return (await CommandRoot.FindAndExecuteCommand (data, cmd, newArguments, commandsInSet, depth)).result;
            } else {
                return new Result (this, "");
            }
        }

        public override string GetCommand() {
            return helpPrefix + command + " (set)";
        }

        public override string GetHelp(SocketMessage e) {
            // Display all commands within command.
            string help = "";
            help += ("Commands in the **" + command + "** command set:\n```");
            foreach (Command c in commandsInSet) {
                if (c.AllowExecution (e) == "") {
                    help += c.Format () + "\n";
                }
            }
            if (help == "Commands in the **" + command + "** command set:\n```") { // Ew
                return "This set contains no available commands.";
            } else {
                return help + "```";
            }
        }

        public void AddCommands(params Command [ ] procCommands) {
            commandsInSet.AddRange (procCommands);

            foreach (Command c in procCommands) {
                FeedRecursiveData (c); // Make sure the commands metadata is set to fit the rest of the set.
            }
        }

        private void FeedRecursiveData (Command c) {
            c.helpPrefix = helpPrefix + command + " ";
            if (c.requiredPermissions.Count == 0)
                c.requiredPermissions = requiredPermissions;

            c.Initialize ();
        }

        public void RemoveCommands(params Command[] commands) {
            foreach (Command cmd in commands) {
                commandsInSet.Remove (cmd);
            }
        }

        public List<Command> GetCommands() {
            return commandsInSet;
        }
    }
}
