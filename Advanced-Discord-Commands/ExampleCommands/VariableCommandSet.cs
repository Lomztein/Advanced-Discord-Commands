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

        public static readonly Category ArrayCategory = new Category ("Array", "Commands revolving around the use of arrays, single variables that contain other variables.");
        public static readonly Category LocalCategory = new Category ("Local", "Commands that are message-specific, where everything is automatically deleted after all commands has finished executing.");
        public static readonly Category PersonalCategory = new Category ("Personal", "Commands that are user-specific, which can be controlled and accessed by only the one declaring them");
        public static readonly Category GlobalCategory = new Category ("Global", "Commands that are server-specific, declared and set by only server moderators, but can be accessed by anyone.");
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

            public static readonly Category ModifyCategory = new Category ("Modify", "Commands used to modify arrays, by adding or removing new elements.");
            public static readonly Category InfomationCategory = new Category ("Information", "Commands used to gather information about the arrays given to them.");

            public ArraySet() {
                Name = "array";
                Description = "Set for manipulating arrays.";
                Category = ArrayCategory;

                commandsInSet = new List<ICommand> {
                    new Create (), new Add (), new Remove (), new Count (), new IndexOf (), new AtIndex (),
                };
            }

            public class Create : Command {
                public Create() {
                    Name = "create";
                    Description = "Create an array.";
                    Category = ModifyCategory;
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
                    Category = ModifyCategory;
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
                    Category = ModifyCategory;
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
                    Category = InfomationCategory;
                }

                [Overload (typeof (int), "Returns the amount of objects that are in an array.")]
                public Task<Result> Execute(CommandMetadata e, dynamic[] array) {
                    return TaskResult (array.Length, $"Array count: {array.Length}");
                }
            }


            public class AtIndex : Command {
                public AtIndex() {
                    Name = "atindex";
                    Description = "Get object at index.";
                    Category = InfomationCategory;
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
                    Description = "Get the index of object.";
                    Category = InfomationCategory;
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
                Category = LocalCategory;
            }

            [Overload (typeof (void), "Set a variable in the local scope, only accessable from current command chain.")]
            public Task<Result> Execute(CommandMetadata e, string name, object variable) {
                CommandVariables.Set (e.Message.Id, name, variable, false);
                return TaskResult (null, $"Succesfully set local variable {name} to {variable}");
            }
        }

        public class SetP : Command {
            public SetP() {
                Name = "setp";
                Description = "Set personal variable.";
                Category = PersonalCategory;
            }

            [Overload (typeof (void), "Set a variable in the personal scope, only accessable for current user.")]
            public Task<Result> Execute(CommandMetadata e, string name, object variable) {
                CommandVariables.Set (e.Message.Author.Id, name, variable, false);
                return TaskResult (null, $"Succesfully set personal variable {name} to {variable}");
            }
        }

        public class SetG : Command {
            public SetG() {
                Name = "setg";
                Description = "Set global variable.";

                RequiredPermissions.Add (Discord.GuildPermission.Administrator);
                Category = GlobalCategory;
            }

            [Overload (typeof (void), "Set a variable in the global scope, accessable for the entire Discord server.")]
            public Task<Result> Execute(CommandMetadata e, string name, object variable) {
                CommandVariables.Set (e.Message.GetGuild ().Id, name, variable, false);
                return TaskResult (null, $"Succesfully set global variable {name} to {variable}");
            }
        }

        public class GetL : Command {
            public GetL() {
                Name = "getl";
                Description = "Get local variable.";
                Category = LocalCategory;
            }

            [Overload (typeof (object), "Get a variable in the local scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                object variable = CommandVariables.Get (e.Message.Id, name);
                return TaskResult (variable, variable?.ToString ());
            }
        }

        public class GetP : Command {
            public GetP() {
                Name = "getp";
                Description = "Get personal variable.";
                Category = PersonalCategory;
            }

            [Overload (typeof (object), "Get a variable in the personal scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                object variable = CommandVariables.Get (e.Message.Author.Id, name);
                return TaskResult (variable, variable?.ToString ());
            }
        }

        public class GetG : Command {
            public GetG() {
                Name = "getg";
                Description = "Get global variable.";
                Category = GlobalCategory;
            }

            [Overload (typeof (object), "Get a variable in the global scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                object variable = CommandVariables.Get (e.Message.GetGuild ().Id, name);
                return TaskResult (variable, variable?.ToString ());
            }
        }

        public class DelL : Command {
            public DelL() {
                Name = "dell";
                Description = "Delete local variable.";
                Category = LocalCategory;
            }

            [Overload (typeof (bool), "Delete a variable in the local scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                return TaskResult (CommandVariables.Delete (e.Message.Id, name), "");
            }
        }

        public class DelP : Command {
            public DelP() {
                Name = "delp";
                Description = "Delete personal variable.";
                Category = PersonalCategory;
            }

            [Overload (typeof (bool), "Delete a variable in the personal scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                return TaskResult (CommandVariables.Delete (e.Message.Author.Id, name), "");
            }
        }

        public class DelG : Command {
            public DelG() {
                Name = "delg";
                Description = "Delete global variable.";

                RequiredPermissions.Add (Discord.GuildPermission.Administrator);
                Category = GlobalCategory;
            }

            [Overload (typeof (bool), "Delete a variable in the global scope.")]
            public Task<Result> Execute(CommandMetadata e, string name) {
                return TaskResult (CommandVariables.Delete (e.Message.GetGuild ().Id, name), "");
            }
        }
    }
}