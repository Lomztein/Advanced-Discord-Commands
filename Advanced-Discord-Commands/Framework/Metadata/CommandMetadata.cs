using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using Lomztein.AdvDiscordCommands.Framework.Execution;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public class CommandMetadata {

        public IMessage Message { get; private set; }
        public ICommandRoot Root { get; private set; }

        public ulong? Owner { get; private set; }
        public int Complexity { get; private set; }
        public uint ProgramCounter { get; private set; }

        public string Content { get => Message.Content; }
        public IUser Author { get => Message.Author; }
        public ulong AuthorID { get => Message.Author.Id; }
        public ulong ID { get => Message.Id; }

        public ISplitter Splitter { get; private set; }
        public IExtractor Extractor { get; private set; }
        public ISearcher Searcher { get; private set; }
        public IExecutor Executor { get; private set; }

        public readonly List<string> _callstack = new List<string>();

        public CommandMetadata (IMessage _message, ICommandRoot _root, ulong? _owner, ISplitter splitter, IExtractor extractor, ISearcher searcher, IExecutor executor) {
            Message = _message;
            Root = _root;
            Owner = _owner;

            Splitter = splitter;
            Extractor = extractor;
            Searcher = searcher;
            Executor = executor;
        }

        public void ChangeComplexity(int change) => Complexity += change;
        public void ChangeProgramCounter(uint change) => ProgramCounter += change;
        public void SetProgramCounter(uint position) => ProgramCounter = position;
        public void AbortProgram() => SetProgramCounter (uint.MaxValue);
        public void AddToCallstack(string call) => _callstack.Add(call);
        public string[] GetCallstack() => _callstack.ToArray();

    }
}
