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
        public override async Task<Result> TryExecute(CommandMetadata data, Arguments args)
        {
            string command = string.Empty;
            List<List<object>> newSets = new List<List<object>>();
            int index = 0;

            foreach (object[] arguments in args)
            {
                List<object> newArgs = new List<object>();
                if (arguments.Length != 0)
                {
                    string zeroArgString = arguments[0].ToString();
                    int spaceIndex = zeroArgString.IndexOfAny(CharExtensions.WhitespaceChars);

                    if (spaceIndex != -1)
                    {
                        newArgs.Add(zeroArgString.Substring(spaceIndex).Trim());
                    }

                    for (int i = 1; i < arguments.Length; i++)
                    {
                        newArgs.Add(arguments[i]);
                    }

                    if (index == 0)
                    {
                        command = zeroArgString.Substring(0, spaceIndex != -1 ? spaceIndex : zeroArgString.Length).Trim();
                        command = data.Searcher.GetTrigger(data.Message.GetGuild()?.Id) + command;
                    }
                    index++;
                }
                newSets.Add(newArgs);
            }

            if (args.Any(x => x.Length > 0))
            {
                ExecutionData execution = data.Root.CreateExecution(command, data, new Arguments (newSets), _commandsInSet);
                if (execution.Executable == false)
                {
                    Result defaultResult = await ExecuteDefault (args, data);
                    if (defaultResult.Failed)
                    {
                        return new Result(GetDocumentationEmbed(data), string.Empty);
                    }
                    else
                    {
                        return defaultResult;
                    }
                }
                else
                {
                    return await data.Executor.Execute(execution);
                }
            }
            else
            {
                return new Result(GetDocumentationEmbed(data), string.Empty);
            }
        }

        private async Task<Result> ExecuteDefault(Arguments args, CommandMetadata data)
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
