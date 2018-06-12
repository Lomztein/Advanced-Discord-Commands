using Lomztein.AdvDiscordCommands.Framework;
using System;
using Xunit;

namespace Lomztein.AdvDiscordCommands.Tests
{
    public class CommandRootTest {

        [Theory]
        [InlineData ("arg1, arg2, arg3", new string [ ] { "arg1", "arg2", "arg3" } )]
        [InlineData ("arg1 arg2, arg3", new string [ ] { "arg1 arg2", "arg3" } )]
        [InlineData ("arg1, (arg2, arg3)", new string [ ] { "arg1", "(arg2, arg3)" } )]
        [InlineData ("arg1, (!arg2, arg3)", new string [ ] { "arg1", "(!arg2, arg3)" } )]
        [InlineData ("(arg1, arg2, arg3)", new string [ ] { "(arg1, arg2, arg3)" } )]
        [InlineData ("(!arg1 arg2, arg3)", new string [ ] { "(!arg1 arg2, arg3)" } )]
        [InlineData ("arg1, [arg2, arg3]", new string [ ] { "arg1", "[arg2, arg3]" })]
        [InlineData ("arg1, [!arg2, arg3]", new string [ ] { "arg1", "[!arg2, arg3]" })]
        [InlineData ("[arg1, arg2, arg3]", new string [ ] { "[arg1, arg2, arg3]" })]
        [InlineData ("[!arg1, arg2, arg3]", new string [ ] { "[!arg1, arg2, arg3]" })]
        public void SplitArgsTests(string input, string[] expected) {
            string [ ] result = CommandRoot.SplitArgs (input);

            bool allTrue = true;
            for (int i = 0; i < result.Length; i++)
                if (result [ i ] != expected [ i ])
                    allTrue = false;

            Assert.True (allTrue);
        }

        [Theory]
        [InlineData ("!ban Agesome1", "!ban", new string [ ] { "Agesome1" })]
        [InlineData ("!ban    Agesome1", "!ban", new string [ ] { "Agesome1" })]
        [InlineData ("!ban Agesome1, 2 days", "!ban", new string [ ] { "Agesome1", "2 days" })]
        public void ConstructArgumentsTests (string input, string outCommand, string[] outArguments) {
            object[] arguments = CommandRoot.ConstructArguments (input, out string command).ToArray ();

            Assert.Equal (outCommand, command);

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
            string[] output = CommandRoot.SplitMultiline (input);

            bool allTrue = true;
            for (int i = 0; i < output.Length; i++)
                if (output [ i ] != expectedOutput[ i ])
                    allTrue = false;

            Assert.True (allTrue);
        }
    }
}
