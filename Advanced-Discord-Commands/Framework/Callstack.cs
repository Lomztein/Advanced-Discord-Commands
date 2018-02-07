using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public class Callstack {

    public ulong chainID;
    public List<Item> items;

    public Callstack(ulong _chainID) {
        chainID = _chainID;
        items = new List<Item> ();
    }

    public class Item {
        public Command command;
        public List<string> arguments;
        public string message;
        public object returnObj;

        public Item(Command _command, List<string> _arguments, string _message, object _returnObj) {
            command = _command;
            arguments = _arguments;
            message = _message;
            returnObj = _returnObj;
        }
    }
}
}
