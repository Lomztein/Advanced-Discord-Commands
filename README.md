# Advanced Discord Commands Framework

You know what would be cool and utterly useless in 99% of cases? If you could use commands and their results as arguments in other commands. You know what could be even cooler and even more useless in 50% of those cases? Multiline programs constructed entirely of Discord commands. You know what would be kinda useful actually? A robust and easy-to-use Discord command framework, with support for multiple overloads of commands, practically allowing multiple different commands under the same name.

**This framework has all of that and even more!**

This framework is is based on the [Discord.NET API Wrapper by RougeException](https://github.com/RogueException/Discord.Net), but with a bit of elbow-grease and the will of Zues you might be able to port it to a different wrapper.

## Feature Overview

### Basic functionality is perfectly simple!

* It's exactly the same as the standard frameworks for the avarage user, you have a trigger prefix and a command name. The command is executed, and the user is happy!

* While some, including the standard framework, has aliasses for commands, we have the opposite! One command can have any number of different overloads, which are selected based on which kinds of input are given. Aliasses could easily be implemented as well, if the need arises.

* Mentions are automatically converted into the mentionable entities they represent, so they can by default be used in commands that take in something like a user or a text channel.

Executing a simple command can look something like this: `!kick @Agesome1`, which would assumingly kick someone by that name, whoever that is. :thinking:

### Halfway advanced stuff is a little more complicated, but not *too* bad.

* Commands can be used as arguments to other commands, by wrapping them in paranthesies. This can be done in as many layers as you so desire!

This can look something like this: `!kick (!randomuser)` which, if we assume that `!randomuser` returns an entirely random user, would kick the unlucky user, much like a stupidly unfun game of Russian Roulette.

If we assume that `!randomuser` has an alternative overload which can take in a list of users, and we have another command `!invoice` which takes in a name, and returns a list of users in the named channel, then we can type in `!kick (!randomuser (!invoice General))` to completely ruin an intense teamfight!

* Additionally, there are Commmand Sets! They just contain and catagories a bunch of commands under a single prefix, like `!voice` or `!games` or something. Even cooler is that we can nest these sets infinitely to have a stupidly large amount of commands without flooding the command list.

In the example commands, we have a set named `!math`, and in that the command `!add`, which sums numbers. Now we can just use `!math add 2 2` to relearn what was lost last time you played World of Warcraft! As of writing, the `!math` set contains 25 commands.

### Then there's the advanced stuff. This is.. a bit convoluted.

* Setting and getting variables in three different scopes, done using the `!var` example command set. This is in local, personal and global scopes! Local are bound to the single command, personal is bound to the user, and global is bound to the server!

* A few different operators to modify how the interpreter handles some input. There is the one you've already seen, (), and then there is [], and even {}!

() Tells the interpreter that what is inside should be executed, in case that it's a commands. If it's not a command, then it'll just be parsed through.

[] Tells the interpreter to force it through as a string of text. This is intended for use with commands which take in other commands, like `!math graph`, a command which graphs the numeric result of the given command. It can also be used just to force something trough as text regardless, something it shares with ().

{} Tells the interpreter that what is inside is a variable, and it is synonymous with using the getter functions of the `!var` command set. It first searches for a local variable, if nothing is found it tries personal, and global if nothing is found there again.

* Advanced flow commands, as well as multiline command "programs". At this point we're fairly sure the entire thing is actually turing complete, but don't quote that, because it *might just be wrong.*

A few commands exists in the example commands that can be used for simple advanced stuff, like for loops and if statements. These currently only support single-line commands, but that is something that we'd like to change, but isn't exactly a priority, since that can be managed with `!flow goto`, explained in the next paragraph.

Commands can be entered in multiple lines at a time, seperated with the well known `;`, this allows multiple commands to be run in direct sequence. This sequence can be modified using `flow goto`, which sets the program counter to the given number, effectively setting the sequence back to that line number.

## The code side of things.

### Interacting with the framework.

To interact with the framework, you're going to need to create a `CommandRoot` object, which contains all commands and command sets at the root. To enter a command, just pump the text message into the method `EnterCommand (string message)`, and from there the framework will take over. `EnterCommand` then returns the result, containing the object and message from the last command in the message, or, if a questionmark was given as a single argument, a string of commands in a set if the command was a set, and an embed as the value if the command was a basic command.

For example, a snippet of code from the [Example Bot](ExampleBot/Program.cs):

```cs
private async Task MessageRecievedEvent(SocketMessage arg) {
    var result = await commandRoot.EnterCommand (arg as SocketUserMessage);
    await arg.Channel.SendMessageAsync (result?.message, false, result?.value as Embed);
}
```

### Creating commands is *simple-ish*.

* Creating a new command is as simple as creating a class inheriting from the abstract `Command` class, and filling that up with whatever data is wanted!

* The code that executes for an overload is written as a method named Execute, which must return Task<Result> and which first argument must be of type `CommandMetadata`. Any other arguments are free to be whatever you want, and the framework will automatically convert the given arguments to the types, if possible.

* Multiple overloads can exist in one command, and the framework will select and execute the first one that fits!

* Not strictly required, but it's recommended that you add overload descriptions using `AddOverload (Type returnType, string description)` in the constructor. These define the return type and individual overload description for self-documenting porpourses.

* Returning Task<Result> can be done using either the helper method TaskResult if the command is syncronous, or directly returning a `Result` if the command is asyncronous. Result is a simple datacarrier, which contains a returned object, and a returned message to be send to Discord chat once the command has been executed.

**For instance, how about this example command with three different overloads, which returns random numbers?**

```cs
public class Random : Command {
    public Random() {
        command = "random";
        shortHelp = "Get random numbers.";

        AddOverload (typeof (double), "Returns random number between 0 and 1.");
        AddOverload (typeof (double), "Returns random number between 0 and given number.");
        AddOverload (typeof (double), "Returns random number between the given numbers.");
    }

    public Task<Result> Execute(CommandMetadata e) {
        System.Random random = new System.Random ();
        return TaskResult (random.NextDouble (), "");
    }

    public Task<Result> Execute(CommandMetadata e, double max) {
        System.Random random = new System.Random ();
        return TaskResult (random.NextDouble () * max, "");
    }

    public Task<Result> Execute(CommandMetadata e, double min, double max) {
        System.Random random = new System.Random ();
        return TaskResult (random.NextDouble () * (max + min) - min, "");
    }
}
```

Don't actually kick Agesome1, he's pretty cool dude, if a bit of an aquired taste.
