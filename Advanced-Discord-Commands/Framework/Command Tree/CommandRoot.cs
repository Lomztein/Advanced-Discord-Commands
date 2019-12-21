using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Execution;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework {

    public class CommandRoot : ICommandRoot {

        public ISplitter Splitter { get; set; }
        public IExtractor Extractor { get; set; }
        public ISearcher Searcher { get; set; }
        public IExecutor Executor { get; set; }

        public List<ICommand> Commands { get; private set; } = new List<ICommand> ();
        private List<CommandPointer> _shortcuts = new List<CommandPointer>();

        public string Name { get => "Command Root"; set => throw new NotImplementedException (); }
        public string Description { get => "This is the root of all commands, wherefrom an expansive tree of commands form."; set => throw new NotImplementedException (); }

        public CommandRoot (List<ICommand> commands) {
            Commands = commands;
            Splitter = new DefaultSplitter ();
            Extractor = new DefaultExtractor ();
            Searcher = new DefaultSearcher (x => DefaultSearcher.DEFAULT_TRIGGER, x => DefaultSearcher.DEFAULT_TRIGGER_HIDDEN);
            Executor = new DefaultExecutor ();
        }

        public CommandRoot (List<ICommand> _commands, Func<ulong, char> trigger, Func<ulong, char> hiddenTrigger) : this (_commands) {
            Searcher = new DefaultSearcher (trigger, hiddenTrigger);
        }

        public CommandRoot (List<ICommand> commands, Func<ulong, char> trigger, Func<ulong, char> hiddenTrigger, ISplitter splitter, IExtractor extractor, ISearcher searcher, IExecutor executor) : this (commands, trigger, hiddenTrigger) {
            Splitter = splitter;
            Extractor = extractor;
            Searcher = searcher;
            Executor = executor;
        }

        public async Task<Result> EnterCommand(string fullCommand, IMessage message, ulong? owner)
        {
            CommandMetadata metadata = new CommandMetadata(message, this, owner, Splitter, Extractor, Searcher, Executor);

            string[] multiline = Splitter.SplitMultiline(fullCommand);
            Result result = null;

            try
            {
                while (metadata.ProgramCounter < multiline.Length)
                {
                    int counter = (int)metadata.ProgramCounter;
                    string line = multiline[counter];

                    ICommand command = Searcher.Search(line, GetCommands(), owner);
                    if (command == null)
                    {
                        command = Searcher.Search(line, _shortcuts.Cast<ICommand>().ToList (), owner);
                    }

                    Arguments arguments = Extractor.ExtractArguments(line);

                    ExecutionData execution = new ExecutionData(command, arguments, metadata, arguments.IsDocumentationRequest ());

                    if (execution.Executable)
                        result = await Executor.Execute(execution);

                    metadata.ChangeProgramCounter(1);
                }
            }
            catch (Exception exception)
            {
                metadata.AbortProgram();
                result = new Result(exception);
            }

            CommandVariables.Clear(metadata.Message.Id);
            return result;
        }

        private void RecursiveFlatten ()
        {
            _shortcuts = this.GetAllRecursive().Where (x => !string.IsNullOrEmpty (x.Shortcut)).Select(x => new CommandPointer(x, x.Shortcut, x.ShortcutAliases)).ToList ();
        }

        /// <summary>
        /// This must be run after the initial commands has been added to the root.
        /// </summary>
        public void InitializeCommands() {
            foreach (ICommand cmd in Commands) {
                cmd.CommandParent = this;
                cmd.Initialize ();
            }
            RecursiveFlatten();
        }

        public List<ICommand> GetCommands() {
            return Commands;
        }

        public void AddCommands(params ICommand [ ] newCommands) {
            Commands.AddRange (newCommands);
            foreach (ICommand cmd in newCommands)
                cmd.Initialize ();
        }

        public void RemoveCommands(params ICommand [ ] toRemove) {
            foreach (ICommand cmd in toRemove) {
                Commands.Remove (cmd);
            }
        }

        public string GetChildPrefix(ulong? owner) => Searcher.GetTrigger (owner).ToString ();

    }
}
