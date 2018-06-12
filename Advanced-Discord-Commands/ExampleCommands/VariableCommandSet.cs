using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.ExampleCommands {

    public class VariableCommandSet : CommandSet {
        public VariableCommandSet() {

            Name = "var";
            Description = "Set relating to variables";
            Category = StandardCategories.Advanced;

            commandsInSet = new List<ICommand> {
                new SetL (), new SetP (), new SetG (),
                new GetL (), new GetP (), new GetG (),
                new DelL (), new DelP (), new DelG (),
                new ArraySet  (),
            };
        }

        public class ArraySet : CommandSet {
            public ArraySet() {
                Name = "array";
                Description = "Set for manipulating arrays.";
                Category = StandardCategories.Advanced;

                commandsInSet = new List<ICommand> {
                    new Create (), new Add (), new Remove (), new Count (), new IndexOf (), new AtIndex (),
                };
            }

            public class Create : Command {
                public Create() {
                    Name = "create";
                    Description = "Create an array.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (object[]), "Create an entirely empty array.")]
                public Task<Result> Execute(CommandMetadata data) {
                    return TaskResult (new dynamic[0], "");
                }

                [Overload (typeof (object[]), "Create an array containing the given objects.")]
                public Task<Result> Execute(CommandMetadata e, params dynamic[] toAdd) {
                    dynamic[] array = new dynamic[toAdd.Length];
                    for (int i = 0; i < toAdd.Length; i++) {
                        array[i] = toAdd[i];
                    }
                    return TaskResult (array, "");
                }
            }

            public class Add : Command {
                public Add() {
                    Name = "add";
                    Description = "Add to an array.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (object[]), "Add the given objects to a given array and returns it.")]
                public Task<Result> Execute(CommandMetadata e, dynamic[] array, params dynamic[] toAdd) {
                    List<dynamic> arr = array.ToList ();
                    foreach (dynamic obj in toAdd) {
                        arr.Add (obj);
                    }
                    return TaskResult (arr.ToArray (), "");
                }
            }

            public class Remove : Command {
                public Remove() {
                    Name = "remove";
                    Description = "Remove from an array.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (object), "Removes the given object from an array.")]
                public Task<Result> Execute(CommandMetadata e, dynamic[] array, params dynamic[] toRemove) {
                    List<dynamic> arr = array.ToList ();
                    foreach (dynamic obj in toRemove) {
                        arr.Remove (obj);
                    }
                    return TaskResult (arr.ToArray (), "");
                }
            }

            public class Count : Command {
                public Count() {
                    Name = "count";
                    Description = "Count elements in an array.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (int), "Returns the amount of objects that are in an array.")]
                public Task<Result> Execute(CommandMetadata e, dynamic[] array) {
                    return TaskResult (array.Length, $"Array count: {array.Length}");
                }
            }


            public class AtIndex : Command {
                public AtIndex() {
                    Name = "atindex";
                    Description = "Get object at index from array.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (object), "Get an object from an array at the given index.")]
                public Task<Result> Execute(CommandMetadata e, int index, params dynamic[] array) {
                    if (array != null && array.Length > 0 && (index < 0 || index >= array.Length)) {
                        return TaskResult (null, "Error - Index null or out of range.");
                    } else {
                        return TaskResult (array[index], array[index].ToString ());
                    }
                }
            }

            public class IndexOf : Command {
                public IndexOf() {
                    Name = "indexof";
                    Description = "Get the index of object array.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (int), "Get the index of a given object within a given array. Returns -1 if not there.")]
                public Task<Result> Execute(CommandMetadata e, object obj, params dynamic[] array) {
                    return TaskResult (array.ToList ().IndexOf (obj), "");
                }
            }
        }

        public class SetL : Command {
            public SetL() {
                Name = "setl";
                Description = "Set local variable.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (void), "Set a variable in the local scope, only accessable from current command chain.")]
            public Task<Result> Execute(CommandMetadata e, string name, object variable) {
                CommandVariables.Set (e.message.Id, name, variable, false);
                return TaskResult (null, $"Succesfully set local variable {name} to {variable}");
            }
        }

        public class SetP : Command {
            public SetP() {
                Name = "setp";
                Description = "Set personal variable.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (void), "Set a variable in the personal scope, only accessable for current user.")]
            public Task<Result> Execute(CommandMetadata e, string name, object variable) {
                CommandVariables.Set (e.message.Author.Id, name, variable, false);
                return TaskResult (null, $"Succesfully set personal variable {name} to {variable}");
            }
        }

        public class SetG : Command {
            public SetG() {
                Name = "setg";
                Description = "Set global variable.";

                requiredPermissions.Add (Discord.GuildPermission.Administrator);
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (void), "Set a variable in the global scope, accessable for the entire Discord server.")]
            public Task<Result> Execute(CommandMetadata e, string name, object variable) {
                CommandVariables.Set (e.message.GetGuild ().Id, name, variable, false);
                return TaskResult (null, $"Succesfully set global variable {name} to {variable}");
            }
        }

        public class GetL : Command {
            public GetL() {
                Name = "getl";
                Description = "Get local variable.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (object), "Get a variable in the local scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                object variable = CommandVariables.Get (e.message.Id, name);
                return TaskResult (variable, variable?.ToString ());
            }
        }

        public class GetP : Command {
            public GetP() {
                Name = "getp";
                Description = "Get personal variable.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (object), "Get a variable in the personal scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                object variable = CommandVariables.Get (e.message.Author.Id, name);
                return TaskResult (variable, variable?.ToString ());
            }
        }

        public class GetG : Command {
            public GetG() {
                Name = "getg";
                Description = "Get global variable.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (object), "Get a variable in the global scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                object variable = CommandVariables.Get (e.message.GetGuild ().Id, name);
                return TaskResult (variable, variable?.ToString ());
            }
        }

        public class DelL : Command {
            public DelL() {
                Name = "dell";
                Description = "Delete local variable.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (bool), "Delete a variable in the local scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                return TaskResult (CommandVariables.Delete (e.message.Id, name), "");
            }
        }

        public class DelP : Command {
            public DelP() {
                Name = "delp";
                Description = "Delete personal variable.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (bool), "Delete a variable in the personal scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                return TaskResult (CommandVariables.Delete (e.message.Author.Id, name), "");
            }
        }

        public class DelG : Command {
            public DelG() {
                Name = "delg";
                Description = "Delete global variable.";

                requiredPermissions.Add (Discord.GuildPermission.Administrator);
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (bool), "Delete a variable in the global scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                return TaskResult (CommandVariables.Delete (e.message.GetGuild ().Id, name), "");
            }
        }
    }
}