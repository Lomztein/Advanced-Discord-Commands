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

        private static readonly Category BooleanCategory = new Category ("Boolean", "Commands focused on boolean operations, essentially asking true or false to whatever is given to them.");
        private static readonly Category ControlCategory = new Category ("Control", "Commands used to control the flow of command chains, as well as looping over a single command multiple times.");

        public FlowCommandSet() {
            Name = "flow";
            Description = "Commands controlling flow.";
            Category = StandardCategories.Advanced;

            commandsInSet = new List<ICommand> {
                new IsNull (), new If (), new Not (), new And (), new Or (), new For (), new Foreach (), new Goto (), new Wait (),
            };
        }

        public class IsNull : Command {
            public IsNull() {
                Name = "isnull";
                Description = "Does object exist?.";
                Category = BooleanCategory;
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
                Category = BooleanCategory;
            }

            [Overload (typeof (object), "Runs the given command if input boolean is true.")]
            public async Task<Result> Execute(CommandMetadata e, bool boolean, string command) {
                if (boolean) {
                    ExecutionData execution = e.Root.CreateExecution (command, e, e.Root.GetCommands ());
                    return await e.Executor.Execute (execution);
                }
                return new Result (null, "T'was false.");
            }

            [Overload (typeof (object), "Runs the first command if input boolean is true, otherwise the second.")]
            public async Task<Result> Execute(CommandMetadata e, bool boolean, string command1, string command2) {
                if (boolean) {
                    ExecutionData execution = e.Root.CreateExecution (command1, e, e.Root.GetCommands ());
                    return await e.Executor.Execute (execution);
                }
                else {
                    ExecutionData execution = e.Root.CreateExecution (command2, e, e.Root.GetCommands ());
                    return await e.Executor.Execute (execution);
                }
            }
        }

        public class Not : Command {
            public Not() {
                Name = "not";
                Description = "Inverses booleans.";
                Category = BooleanCategory;
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
                Category = BooleanCategory;
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
                Category = BooleanCategory;
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
                Description = "Run a command a set times.";
                Category = ControlCategory;
            }

            [Overload (typeof (void), "Loop given command the given amount of times with the iteration variable name \"for\".")]
            public async Task<Result> Execute(CommandMetadata data, int amount, string command) {
                return await Execute (data, "for", amount, command);
            }

            [Overload (typeof (void), "Loop given command the given amount of times with a custom iteration variable name.")]
            public async Task<Result> Execute(CommandMetadata data, string varName, int amount, string command) {
                if (command.Length > 1 && command[0].IsCommandTrigger (data.Message.GetGuild ()?.Id, data.Searcher)) {
                    for (int i = 0; i < amount; i++) {
                        CommandVariables.Set (data.Message.Id, varName, i, true);

                        ExecutionData execution = data.Root.CreateExecution (command, data, data.Root.GetCommands ());
                        await data.Executor.Execute (execution);
                    }
                }
                return new Result (null, "");
            }
        }

        public class Foreach : Command {
            public Foreach() {
                Name = "foreach";
                Description = "Iterate over an array.";
                Category = ControlCategory;
            }

            [Overload (typeof (void), "Loop given command for each item in the given array, with the a custom item variable name.")]
            public async Task<Result> Execute(CommandMetadata data, string varName, string command, params object[] array) {

                foreach (object obj in array) {
                    CommandVariables.Set (data.Message.Id, varName, obj, true);

                    ExecutionData execution = data.Root.CreateExecution (command, data, data.Root.GetCommands ());
                    await data.Executor.Execute (execution);
                }
                return new Result (null, "");
            }
        }

        public class Wait : Command {
            public Wait() {
                Name = "wait";
                Description = "Halts command for a while.";
                Category = ControlCategory;
            }

            [Overload (typeof (void), "Wait for the given amount of seconds.")]
            [Example ("", "\"Waited 10 seconds!\"", "10")]
            public async Task<Result> Execute(CommandMetadata data, double seconds) {
                await Task.Delay ((int)Math.Round (seconds * 1000));
                return new Result (null, "Waited " + seconds + " seconds!");
            }

            [Overload (typeof (object), "Wait for the given amount of seconds, then execute the given command.")]
            [Example ("\"Exammple Text\"", "\"Exammple Text\"", "10", "[!print Example Text]")]
            public async Task<Result> Execute (CommandMetadata data, double seconds, string command) {
                await Task.Delay ((int)Math.Round (seconds * 1000));

                ExecutionData execution = data.Root.CreateExecution (command, data, data.Root.GetCommands ());
                return await data.Executor.Execute (execution);
            }
        }

        public class Goto : Command {
            public Goto() {
                Name = "goto";
                Description = "Move line count.";
                Category = ControlCategory;
            }

            [Overload (typeof (void), "Move sequence execution to a specific command line.")]
            public Task<Result> Execute(CommandMetadata data, uint line) {
                data.SetProgramCounter (line);
                return TaskResult (null, "");
            }
        }
    }
}