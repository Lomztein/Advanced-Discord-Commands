using Lomztein.AdvDiscordCommands.Framework;
using System;
using Xunit;

namespace Lomztein.AdvDiscordCommands.Tests
{
    public class CommandRootTest {

        [Theory]
        [InlineData ("!command arg1, arg2, arg3", new string[] { "arg1", "arg2", "arg3" })]
        [InlineData ("!command arg1 arg2, arg3", new string[] { "arg1 arg2", "arg3" })]
        [InlineData ("!command arg1, (arg2, arg3)", new string[] { "arg1", "(arg2, arg3)" })]
        [InlineData ("!command arg1, (!arg2, arg3)", new string[] { "arg1", "(!arg2, arg3)" })]
        [InlineData ("!command (arg1, arg2, arg3)", new string[] { "(arg1, arg2, arg3)" })]
        [InlineData ("!command (!arg1 arg2, arg3)", new string[] { "(!arg1 arg2, arg3)" })]
        [InlineData ("!command arg1, [arg2, arg3]", new string[] { "arg1", "[arg2, arg3]" })]
        [InlineData ("!command arg1, [!arg2, arg3]", new string[] { "arg1", "[!arg2, arg3]" })]
        [InlineData ("!command [arg1, arg2, arg3]", new string[] { "[arg1, arg2, arg3]" })]
        [InlineData ("!command [!arg1, arg2, arg3]", new string[] { "[!arg1, arg2, arg3]" })]
        public void SplitArgsTests(string input, string[] expected) {
            Executor executor = new Executor ();
            string[] result = executor.ParseArguments (input);

            bool allTrue = true;
            for (int i = 0; i < result.Length; i++)
                if (result[i] != expected[i])
                    allTrue = false;

            Assert.True (allTrue);
        }

        [Theory]
        [InlineData ("!ban Agesome1", new string[] { "Agesome1" })]
        [InlineData ("!ban    Agesome1", new string[] { "Agesome1" })]
        [InlineData ("!ban Agesome1, 2 days", new string[] { "Agesome1", "2 days" })]
        [InlineData ("!ban Agesome1, \n\t2 weeks, \n   \t\t 2 days", new string[] { "Agesome1", "2 weeks", "2 days" })]
        [InlineData ("!math 2,, 2, 2", new string[] { "2", "", "2", "2" })]
        [InlineData ("!math 2   ,, ,  ,   ,   2", new string[] { "2", "", "", "", "", "2" })]
        public void ConstructArgumentsTests (string input, string[] outArguments) {
            Executor executor = new Executor ();
            object[] arguments = executor.ParseArguments (input);

            bool allTrue = true;
            for (int i = 0; i < arguments.Length; i++)
                if ((string)arguments [ i ] != outArguments [ i ])
                    allTrue = false;

            Assert.True (allTrue);
        }

        [Theory]
        [InlineData ("What the fuck is a sonic\nHe is quite fat", new string[] { "What the fuck is a sonic\nHe is quite fat"} )]
        [InlineData ("What the fuck is a sonic;\nHe is quite fat", new string[] { "What the fuck is a sonic", "He is quite fat"} )]
        [InlineData ("[What the fuck is a sonic;\nHe is quite fat]", new string[] { "[What the fuck is a sonic;\nHe is quite fat]"} )]
        [InlineData ("(What the fuck is a sonic;\nHe is quite fat)", new string[] { "(What the fuck is a sonic", "He is quite fat)"} )]
        public void SplitMultilineTests (string input, string[] expectedOutput) {
            Executor executor = new Executor ();
            string[] output = executor.ParseMultiline (input);

            bool allTrue = true;
            for (int i = 0; i < output.Length; i++)
                if (output [ i ] != expectedOutput[ i ])
                    allTrue = false;

            Assert.True (allTrue);
        }
    }
}
