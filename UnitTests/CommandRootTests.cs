using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Execution;
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
            DefaultExtractor extractor = new DefaultExtractor ();
            string[] result = extractor.ExtractArguments (input);

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
            DefaultExtractor extractor = new DefaultExtractor ();
            object[] arguments = extractor.ExtractArguments (input);

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
            DefaultSplitter splitter = new DefaultSplitter ();
            string[] output = splitter.SplitMultiline (input);

            bool allTrue = true;
            for (int i = 0; i < output.Length; i++)
                if (output [ i ] != expectedOutput[ i ])
                    allTrue = false;

            Assert.True (allTrue);
        }
    }
}
