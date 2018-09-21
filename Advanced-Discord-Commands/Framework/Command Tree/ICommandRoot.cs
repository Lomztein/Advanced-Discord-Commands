using Discord;
using Lomztein.AdvDiscordCommands.Framework.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework
{
    // The command root is the root of all commands, obviously. 
    // It consists of four individual components that each
    // perform a task required to execute commands. 
    // This is in accordance to the Single Responsibility Princible of the SOLID principles.
    public interface ICommandRoot : ICommandSet, ICommandParent
    {
        ISplitter Splitter { get; set; } // Takes in the input and splits it into multiple lines, allowing for multi-line commands.
        IExtractor Extractor { get; set; } // Takes the individual command lines and extract the arguments from them
        ISearcher Searcher { get; set; } // Searches through their given list of commands based on the given command.
        IExecutor Executor { get; set; } // Once everything is set up, this creates and executes a command execution.
    }
}
