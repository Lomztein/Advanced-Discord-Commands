using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.ExampleCommands {
    public class FlowCommandSet : CommandSet {
        public FlowCommandSet() {
            Name = "flow";
            Description = "Commands controlling chain flow.";
            Category = StandardCategories.Advanced;

            commandsInSet = new List<ICommand> {
                new IsNull (), new If (), new Not (), new And (), new Or (), new For (), new Foreach (), new Goto (), new Wait (),
            };
        }

        public class IsNull : Command {
            public IsNull() {
                Name = "isnull";
                Description = "Converts objects to booleans.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (bool), "Returns true if the given object isn't null, false otherwise.")]
            public Task<Result> Execute(CommandMetadata e, object obj) {
                return TaskResult (obj != null, "Object = null: " + (obj != null));
            }
        }

        public class If : Command {
            public If() {
                Name = "if";
                Description = "Control command flow.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (object), "Runs the given command if input boolean is true.")]
            public async Task<Result> Execute(CommandMetadata e, bool boolean, string command) {
                if (boolean)
                    return (await CommandRoot.FindAndExecuteCommand (e, command, e.root.commands)).result;
                return new Result (null, "T'was false.");
            }

            [Overload (typeof (object), "Runs the first command if input boolean is true, otherwise the second.")]
            public async Task<Result> Execute(CommandMetadata e, bool boolean, string command1, string command2) {
                if (boolean)
                    return (await CommandRoot.FindAndExecuteCommand (e, command1, e.root.commands)).result;
                else
                    return (await CommandRoot.FindAndExecuteCommand (e, command2, e.root.commands)).result;
            }
        }

        public class Not : Command {
            public Not() {
                Name = "not";
                Description = "Inverses booleans.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (bool), "Inverses a single boolean object.")]
            public Task<Result> Execute(CommandMetadata e, bool boolean) {
                return TaskResult (!boolean, $"Not {boolean} = {!boolean}");
            }

            [Overload (typeof (bool), "Inverses a list of boolean object indivdually.")]
            public Task<Result> Execute(CommandMetadata e, bool[] booleans) {
                for (int i = 0; i < booleans.Length; i++) {
                    booleans[i] = !booleans[i];
                }
                return TaskResult (booleans, "");
            }
        }

        public class And : Command {
            public And() {
                Name = "and";
                Description = "Logic gate AND.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (bool), "Compares two given booleans.")]
            public Task<Result> Execute(CommandMetadata e, bool bool1, bool bool2) {
                return TaskResult (bool1 && bool2, $"{bool1} AND {bool2} = {bool1 && bool2}");
            }

            [Overload (typeof (bool), "Compares an entire array of booleans.")]
            public Task<Result> Execute(CommandMetadata e, params bool[] booleans) {
                return TaskResult (booleans.All (x => x), "");
            }
        }

        public class Or : Command {
            public Or() {
                Name = "or";
                Description = "Logic gate OR.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (bool), "Compares two given booleans.")]
            public Task<Result> Execute(CommandMetadata e, bool bool1, bool bool2) {
                return TaskResult (bool1 || bool2, $"{bool1} OR {bool2} = {bool1 || bool2}");
            }

            [Overload (typeof (bool), "Compares an entire array of booleans.")]
            public Task<Result> Execute(CommandMetadata e, params bool[] booleans) {
                return TaskResult (booleans.Any (x => x), "");
            }
        }

        public class For : Command {
            public For() {
                Name = "for";
                Description = "Loop given command a set times.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (void), "Loop given command the given amount of times with the iteration variable name \"for\".")]
            public async Task<Result> Execute(CommandMetadata data, int amount, string command) {
                return await Execute (data, "for", amount, command);
            }

            [Overload (typeof (void), "Loop given command the given amount of times with a custom iteration variable name.")]
            public async Task<Result> Execute(CommandMetadata data, string varName, int amount, string command) {
                if (command.Length > 1 && command[0].IsCommandTrigger (ParentRoot)) {
                    for (int i = 0; i < amount; i++) {
                        CommandVariables.Set (data.message.Id, varName, i, true);
                        await CommandRoot.FindAndExecuteCommand (data, command, data.root.commands);
                    }
                }
                return new Result (null, "");
            }
        }

        public class Foreach : Command {
            public Foreach() {
                Name = "foreach";
                Description = "Loop a given command for each item in an array.";
                Category = StandardCategories.Advanced;

            }

            [Overload (typeof (void), "Loop given command for each item in the given array, with the a custom item variable name.")]
            public async Task<Result> Execute(CommandMetadata data, string varName, string command, params object[] array) {

                foreach (object obj in array) {
                    CommandVariables.Set (data.message.Id, varName, obj, true);
                    await CommandRoot.FindAndExecuteCommand (data, command, data.root.commands);
                }
                return new Result (null, "");
            }
        }

        public class Wait : Command {
            public Wait() {
                Name = "wait";
                Description = "Halts command for a while.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (void), "Wait for the given amount of secondos.")]
            public async Task<Result> Execute(CommandMetadata data, double seconds) {
                await Task.Delay ((int)Math.Round (seconds * 1000));
                return new Result (null, "Waited " + seconds + " seconds!");
            }

            [Overload (typeof (object), "Wait the given amount of seconds, then return the given command.")]
            public async Task<Result> Execute(CommandMetadata data, double seconds, string command) {
                await Task.Delay ((int)Math.Round (seconds * 1000));

                if (command.Length > 1 && command[1].IsCommandTrigger (ParentRoot)) {

                    var res = await CommandRoot.FindAndExecuteCommand (data, command, data.root.commands);
                    return new Result (res.result, res.result.message);
                }
                return new Result (command, "Waoted " + seconds + " seconds!");
            }

            [Overload (typeof (object), "Wait the given amount of seconds, then return the given object.")]
            public async Task<Result> Execute(CommandMetadata e, double seconds, dynamic obj) {
                await Task.Delay ((int)Math.Round (seconds * 1000));
                return new Result (obj, obj.ToString ());
            }
        }

        public class Goto : Command {
            public Goto() {
                Name = "goto";
                Description = "Move line count.";
                Category = StandardCategories.Advanced;
            }

            [Overload (typeof (void), "Move sequence execution to a specific command line.")]
            public Task<Result> Execute(CommandMetadata data, uint line) {
                data.root.SetProgramCounter (data, line);
                return TaskResult (null, "");
            }
        }
    }
}