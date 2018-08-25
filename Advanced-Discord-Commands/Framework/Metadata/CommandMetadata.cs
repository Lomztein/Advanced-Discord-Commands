using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public class CommandMetadata {

        public IMessage Message { get; private set; }
        public ICommandRoot Root { get; private set; }
        public IExecutor Executor { get; private set; }
        public ulong? Owner { get; private set; }
        public int Depth { get; private set; }
        public uint ProgramCounter { get; private set; }

        public CommandMetadata (IMessage _message, ICommandRoot _root, IExecutor _executor, ulong? _owner) {
            Message = _message;
            Root = _root;
            this.Executor = _executor;
            Owner = _owner;
        }

        public void ChangeDepth(int change) => Depth += change;
        public void ChangeProgramCounter(uint change) => ProgramCounter += change;
        public void SetProgramCounter(uint position) => ProgramCounter = position;
        public void AbortProgram() => SetProgramCounter (uint.MaxValue);

    }
}
