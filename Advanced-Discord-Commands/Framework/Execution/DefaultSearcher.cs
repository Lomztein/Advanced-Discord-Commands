using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution {

    public class DefaultSearcher : ISearcher {

        public Func<ulong, char> Trigger { get; set; }
        public Func<ulong, char> HiddenTrigger { get; set; }

        public const char DEFAULT_TRIGGER = '!';
        public const char DEFAULT_TRIGGER_HIDDEN = '/';

        public DefaultSearcher (Func<ulong, char> trigger, Func<ulong, char> hiddenTrigger) {
            Trigger = trigger;
            HiddenTrigger = hiddenTrigger;
        }

        public char GetTrigger(ulong? owner) {
            if (!owner.HasValue)
                return DEFAULT_TRIGGER;
            return Trigger (owner.Value);
        }

        public char GetHiddenTrigger(ulong? owner) {
            if (!owner.HasValue)
                return DEFAULT_TRIGGER_HIDDEN;
            return HiddenTrigger (owner.Value);
        }

        public ICommand Search(string fullCommand, List<ICommand> commandList, ulong? owner) {

            string commandName = fullCommand.ExtractCommandName (owner, this);

            foreach (ICommand cmd in commandList) {
                if (cmd.IsCommand (commandName))
                    return cmd;
            }

            return null;
        }

    }
}