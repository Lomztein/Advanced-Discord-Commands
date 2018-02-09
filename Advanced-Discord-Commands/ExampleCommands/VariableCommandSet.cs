using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Extensions;

namespace Lomztein.AdvDiscordCommands.ExampleCommands {

    public class VariableCommandSet : CommandSet {
        public VariableCommandSet() {

            command = "var";
            shortHelp = "Set relating to variables";
            catagory = Category.Advanced;

            commandsInSet = new List<Command> {
                new SetL (), new SetP (), new SetG (),
                new GetL (), new GetP (), new GetG (),
                new DelL (), new DelP (), new DelG (),
                new ArraySet  (),
            };
        }

        public class ArraySet : CommandSet {
            public ArraySet() {
                command = "array";
                shortHelp = "Set for manipulating arrays.";

                commandsInSet = new List<Command> {
                    new Create (), new Add (), new Remove (), new Count (), new IndexOf (), new AtIndex (),
                };
            }

            public class Create : Command {
                public Create() {
                    command = "create";
                    shortHelp = "Create an array.";

                    AddOverload (typeof (object), "Create an array containing the given objects.");
                }

                public Task<Result> Execute (CommandMetadata data) {
                    return TaskResult (new dynamic [ 0 ], "");
                }

                public Task<Result> Execute(CommandMetadata e, params dynamic [ ] toAdd) {
                    dynamic [ ] array = new dynamic [ toAdd.Length ];
                    for (int i = 0; i < toAdd.Length; i++) {
                        array [ i ] = toAdd [ i ];
                    }
                    return TaskResult (array, "");
                }
            }

            public class Add : Command {
                public Add() {
                    command = "add";
                    shortHelp = "Add to an array.";
                    AddOverload (typeof (object), "Add the given objects to a given array and returns it.");
                }

                public Task<Result> Execute(CommandMetadata e, dynamic [ ] array, params dynamic [ ] toAdd) {
                    List<dynamic> arr = array.ToList ();
                    foreach (dynamic obj in toAdd) {
                        arr.Add (obj);
                    }
                    return TaskResult (arr.ToArray (), "");
                }
            }

            public class Remove : Command {
                public Remove() {
                    command = "remove";
                    shortHelp = "Remove from an array.";

                    AddOverload (typeof (object), "Removes the given object from an array.");
                }

                public Task<Result> Execute(CommandMetadata e, dynamic [ ] array, params dynamic [ ] toRemove) {
                    List<dynamic> arr = array.ToList ();
                    foreach (dynamic obj in toRemove) {
                        arr.Remove (obj);
                    }
                    return TaskResult (arr.ToArray (), "");
                }
            }

            public class Count : Command {
                public Count() {
                    command = "count";
                    shortHelp = "Count elements in an array.";

                    AddOverload (typeof (int), "Returns the amount of objects that are in an array.");
                }

                public Task<Result> Execute(CommandMetadata e, dynamic [ ] array) {
                    return TaskResult (array.Length, $"Array count: {array.Length}");
                }
            }


            public class AtIndex : Command {
                public AtIndex() {
                    command = "atindex";
                    shortHelp = "Get object at index from array.";
                    AddOverload (typeof (object), "Get an object from an array at the given index.");
                    catagory = Category.Advanced;
                }

                public Task<Result> Execute(CommandMetadata e, int index, params dynamic [ ] array) {
                    if (array != null && array.Length > 0 && (index < 0 || index >= array.Length)) {
                        return TaskResult (null, "Error - Index null or out of range.");
                    } else {
                        return TaskResult (array [ index ], array [ index ].ToString ());
                    }
                }
            }

            public class IndexOf : Command {
                public IndexOf() {
                    command = "indexof";
                    shortHelp = "Get the index of object array.";
                    AddOverload (typeof (object), "Get the index of a given object within a given array. Returns -1 if not there.");
                    catagory = Category.Advanced;
                }

                public Task<Result> Execute(CommandMetadata e, object obj, params dynamic [ ] array) {
                    return TaskResult (array.ToList ().IndexOf (obj), "");
                }
            }
        }

        public class SetL : Command {
            public SetL() {
                command = "setl";
                shortHelp = "Set local variable.";

                AddOverload (typeof (object), "Set a variable in the local scope, only accessable from current command chain.");
            }

            public Task<Result> Execute(CommandMetadata e, string name, object variable) {
                try {
                    CommandVariables.Set (e.message.Id, name, variable, false);
                } catch (ArgumentException exc) {
                    return TaskResult (exc.Message, exc.Message);
                }
                return TaskResult (null, "");
            }
        }

        public class SetP : Command {
            public SetP() {
                command = "setp";
                shortHelp = "Set personal variable.";

                AddOverload (typeof (object), "Set a variable in the personal scope, only accessable for current user.");
            }

            public Task<Result> Execute(CommandMetadata e, string name, object variable) {
                try {
                    CommandVariables.Set (e.message.Author.Id, name, variable, false);
                } catch (ArgumentException exc) {
                    return TaskResult (exc.Message, exc.Message);
                }
                return TaskResult (null, "");
            }
        }

        public class SetG : Command {
            public SetG() {
                command = "setg";
                shortHelp = "Set global variable.";

                AddOverload (typeof (object), "Set a variable in the global scope, accessable for the entire Discord server.");
                requiredPermissions.Add (Discord.GuildPermission.Administrator);
            }

            public Task<Result> Execute(CommandMetadata e, string name, object variable) {
                try {
                    CommandVariables.Set (0, name, variable, false);
                } catch (ArgumentException exc) {
                    return TaskResult (exc.Message, exc.Message);
                }
                return TaskResult (null, "");
            }
        }

        public class GetL : Command {
            public GetL() {
                command = "getl";
                shortHelp = "Get local variable.";

                AddOverload (typeof (object), "Get a variable in the local scope.");
            }

            public Task<Result> Execute(CommandMetadata e, string name) {
                object variable = CommandVariables.Get (e.message.Id, name);
                return TaskResult (variable, variable?.ToString ());
            }
        }

        public class GetP : Command {
            public GetP() {
                command = "getp";
                shortHelp = "Get personal variable.";

                AddOverload (typeof (object), "Get a variable in the personal scope.");
            }

            public Task<Result> Execute(CommandMetadata e, string name) {
                object variable = CommandVariables.Get (e.message.Author.Id, name);
                return TaskResult (variable, variable?.ToString ());
            }
        }

        public class GetG : Command {
            public GetG() {
                command = "getg";
                shortHelp = "Get global variable.";

                AddOverload (typeof (object), "Get a variable in the global scope.");
            }

            public Task<Result> Execute(CommandMetadata e, string name) {
                object variable = CommandVariables.Get (e.message.GetGuild ().Id, name);
                return TaskResult (variable, variable?.ToString ());
            }
        }

        public class DelL : Command {
            public DelL() {
                command = "dell";
                shortHelp = "Delete local variable.";
                AddOverload (typeof (bool), "Delete a variable in the local scope.");
            }

            public Task<Result> Execute(CommandMetadata e, string name) {
                return TaskResult (CommandVariables.Delete (e.message.Id, name), "");
            }
        }

        public class DelP : Command {
            public DelP() {
                command = "delp";
                shortHelp = "Delete personal variable.";
                AddOverload (typeof (bool), "Delete a variable in the personal scope.");
            }

            public Task<Result> Execute(CommandMetadata e, string name) {
                return TaskResult (CommandVariables.Delete (e.message.Author.Id, name), "");
            }
        }

        public class DelG : Command {
            public DelG() {
                command = "delg";
                shortHelp = "Delete global variable.";
                AddOverload (typeof (bool), "Delete a variable in the global scope.");

                requiredPermissions.Add (Discord.GuildPermission.Administrator);
            }

            public Task<Result> Execute(CommandMetadata e, string name) {
                return TaskResult (CommandVariables.Delete (0, name), "");
            }
        }
    }
}