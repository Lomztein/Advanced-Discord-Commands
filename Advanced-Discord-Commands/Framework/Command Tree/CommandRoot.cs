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

        public List<ICommand> commands = new List<ICommand> ();

        public string Name { get => "Command Root"; set => throw new NotImplementedException (); }
        public string Description { get => "This is the root of all commands, wherefrom an expansive tree of commands form."; set => throw new NotImplementedException (); }

        public CommandRoot (List<ICommand> _commands, Func<ulong, char> trigger, Func<ulong, char> hiddenTrigger) {
            commands = _commands;
            Splitter = new DefaultSplitter ();
            Extractor = new DefaultExtractor ();
            Searcher = new DefaultSearcher (trigger, hiddenTrigger);
            Executor = new DefaultExecutor ();
        }

        public CommandRoot (List<ICommand> commands, ISplitter splitter, IExtractor extractor, ISearcher searcher, IExecutor executor) : this (commands, null, null) {
            Splitter = splitter;
            Extractor = extractor;
            Searcher = searcher;
            Executor = executor;
        }

        /// <summary>
        /// This must be run after the initial commands has been added to the root.
        /// </summary>
        public void InitializeCommands() {
            foreach (Command cmd in commands) {
                cmd.CommandParent = this;
                cmd.ParentRoot = this;
                cmd.Initialize ();
            }
        }

        public List<ICommand> GetCommands() {
            return commands;
        }

        public void AddCommands(params ICommand [ ] newCommands) {
            commands.AddRange (newCommands);
            foreach (ICommand cmd in newCommands)
                cmd.Initialize ();
        }

        public void RemoveCommands(params ICommand [ ] toRemove) {
            foreach (ICommand cmd in toRemove) {
                commands.Remove (cmd);
            }
        }

        public string GetChildPrefix(ulong? owner) => Searcher.GetTrigger (owner).ToString ();

    }
}
