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

    public abstract class CommandSet : CommandBase, ICommandSet, ICommandParent {

        protected List<ICommand> _commandsInSet = new List<ICommand>();
        protected ICommand _defaultCommand; // TODO: Consider alternative implementations of default commands, since this is its own command with its own will, so to speak. Perhaps it would be better to add Overloads to command sets instead?

        public CommandSet() {
            Name = "commandset";
            Description = "DEFAULT_COMMAND_SET";
            Category = StandardCategories.Miscilaneous;
        }

        public override void Initialize() {
            InitCommands ();
        }

        public void InitCommands() {
            foreach (ICommand c in _commandsInSet) {
                c.CommandParent = this;
                InitializeCommand (c);
            }
            if (_defaultCommand != null)
            {
                _defaultCommand.CommandParent = this;
                InitializeCommand(_defaultCommand);
            }
        }

        // The parsing is currently very reliant on how the rest of the code works, I'd like to change that somehow.
        public override async Task<Result> TryExecute (CommandMetadata data, params object[] arguments) {

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

                command = data.Searcher.GetTrigger (data.Message.GetGuild ()?.Id) + command;

                ExecutionData execution = data.Root.CreateExecution (command, data, newArgs.ToArray (), _commandsInSet);
                return execution.Executable == false ? await ExecuteDefault (arguments, data) : await data.Executor.Execute(execution);
            } else {
                return await ExecuteDefault(arguments, data);
            }
        }

        private async Task<Result> ExecuteDefault (object[] args, CommandMetadata data)
        {
            ExecutionData defaultExecution = new ExecutionData(_defaultCommand, args, data);
            if (defaultExecution.Executable)
            {
                return await data.Executor.Execute(defaultExecution);
            }
            else
            {
                return new Result(GetDocumentationEmbed(data), string.Empty);
            }
        }

        public override string GetCommand(ulong? owner) {
            return this.GetPrefix (owner) + Name + " (set)";
        }

        public void AddCommands(params ICommand [ ] procCommands) {
            _commandsInSet.AddRange (procCommands);

            foreach (ICommand c in procCommands) {
                InitializeCommand (c); // Make sure the commands metadata is set to fit the rest of the set.
            }
        }

        private void InitializeCommand (ICommand c) {
            c.Initialize ();
        }

        public void RemoveCommands(params ICommand[] commands) {
            foreach (ICommand cmd in commands) {
                _commandsInSet.Remove (cmd);
            }
        }

        public List<ICommand> GetCommands() {
            return _commandsInSet;
        }

        public string GetChildPrefix(ulong? owner) => this.GetPrefix (owner) + Name + " ";

        public override Embed GetDocumentationEmbed(CommandMetadata metadata) => this.ListCommands (metadata);
        
        public override CommandOverload[] GetOverloads()
        {
            throw new NotImplementedException();
        }
    }
}
