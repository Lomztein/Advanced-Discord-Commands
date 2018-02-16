﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.Framework {

    public abstract class CommandSet : Command, ICommandSet {

        public List<Command> commandsInSet = new List<Command>();

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

                combinedArgs = commandTrigger + combinedArgs;
                var result = await CommandRoot.FindAndExecuteCommand (data, combinedArgs, commandsInSet);
                return result?.result;
            } else {
                return new Result (this, "");
            }
        }

        public override string GetCommand() {
            return helpPrefix + command + " (set)";
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
