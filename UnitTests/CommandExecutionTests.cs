using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.ExampleCommands;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Execution;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using Lomztein.AdvDiscordCommands.Tests.Fakes;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace Lomztein.AdvDiscordCommands.Tests
{
    public class CommandExecutionTests
    {
        [Theory]
        [InlineData ("!math add 2, 2", 4d)]
        [InlineData ("!math subtract 4,2", 2d)]
        [InlineData ("!math subtract (!math add 4, 4), (!math divide 8, 2)", 4d)]
        [InlineData ("!math multiply (!math floor (!math sin 80)), (!math subtract 10, (!math mod 10, 2))", -10d)]
        [InlineData ("!math multiply    \n  \t (!math \t\t floor (!math sin      80)), (!math subtract 10,  \t\n (!math mod \n\t 10,\n\t     2))", -10d)]
        [InlineData ("!var setl n, 0;\n!flow for 10, [!var setl n, (!math add {n}, 1)];\n!var getl n", 10d)]
        [InlineData ("!math add 2 5 2 6, 6", "2 5 2 66")]
        [InlineData ("!math dieinafire 2, 5", null)]
        [InlineData ("!thisdoesntexist", null)]
        [InlineData ("math add 2, 2", null)]
        [InlineData ("!var setl text, This is Text;\n!math add {text}[5], t", "it")]
        [InlineData ("!flow wait 5, [!print lol]", "lol")]
        public async void TestMathExecution (string input, object expectedResult) {

            CommandRoot testRoot = new CommandRoot (new List<ICommand> () { new MathCommandSet (), new VariableCommandSet (), new FlowCommandSet (), new PrintCommand () });
            testRoot.InitializeCommands ();

            FakeUserMessage message = new FakeUserMessage (input);
            var result = await testRoot.EnterCommand (message.Content, message, null);

            Assert.Equal (expectedResult, result?.Value);

        }

        [Theory]
        [InlineData ("!math add {x}, {y}", typeof (InvalidOperationException))]
        [InlineData ("!var setl test, (!math add 2, 2);\n!math add {test}[0], 2", typeof (RuntimeBinderException))]
        [InlineData ("!var getl x", typeof (InvalidOperationException))]
        [InlineData ("!var setl x, 0", typeof (ArgumentException))]
        public async void TestExceptionHandling (string input, Type expectedException) {

            CommandRoot testRoot = new CommandRoot (new List<ICommand> () { new MathCommandSet (), new VariableCommandSet (), new FlowCommandSet () });
            testRoot.InitializeCommands ();

            FakeUserMessage message = new FakeUserMessage (input);
            var result = await testRoot.EnterCommand (message.Content, message, null);

            Assert.True (expectedException.IsInstanceOfType (result.Exception));
        }
    }
}
