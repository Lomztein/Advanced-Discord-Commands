using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework {

    public class Callstack {

        public static List<Callstack> callstacks = new List<Callstack> ();
        public const int maxCallstacks = 64;

        public ulong chainID;
        public List<Item> items;

        public Callstack(ulong _chainID) {
            chainID = _chainID;
            items = new List<Item> ();
        }

        public static void AddToCallstack(ulong chainID, Item item) {
            Callstack curStack = callstacks.Find (x => x.chainID == chainID);
            if (curStack == null) {
                curStack = new Callstack (chainID);
                callstacks.Insert (0, curStack);
            }

            curStack.items.Add (item);
            if (callstacks.Count > maxCallstacks)
                callstacks.RemoveAt (maxCallstacks);
        }

        // TODO: Disconnect Callstacks from Command.cs, and move it to Executor instead.
        public class Item {

            public ICommand Command { get; private set; }
            public object[] Arguments { get; private set; }
            public int Depth { get; private set; }
            public string Message { get; private set; }
            public object ReturnObj { get; private set; }

            public Item(ICommand _command, object[] _arguments, int _depth, string _message, object _returnObj) {
                Command = _command;
                Arguments = _arguments;
                Depth = _depth;
                Message = _message;
                ReturnObj = _returnObj;
            }

            public string ToString (ulong? owner) {

                string arguments = " ";
                for (int i = 0; i < Arguments.Length; i++) {
                    arguments += Arguments[i] + (i == Arguments.Length - 1 ? "" : Executor.argSeperator + " ");
                }

                string called = Depth + " - " + Command.GetCommand (owner) + arguments;
                return StringExtensions.UniformStrings (called, ReturnObj == null ? "null" : ReturnObj.ToString (), " -> ", 50);

            }
        }
    }
}
