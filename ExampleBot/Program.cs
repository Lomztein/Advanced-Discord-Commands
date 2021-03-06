﻿using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.ExampleCommands;
using System.Collections.Generic;
using System.IO;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using Lomztein.AdvDiscordCommands.Framework.Execution;
using Lomztein.AdvDiscordCommands.Extensions;

namespace Lomztein.AdvDiscordCommands.ExampleBot {
    class Program {

        public static BotClient client;

        static void Main(string [ ] args) {
            Console.WriteLine ("Booting application..");
            client = new BotClient (File.ReadAllText (AppContext.BaseDirectory + "/data/token.txt").Trim ()); // Very simple token loading, because who's gonna write a config system for a bo   t with less than 100 lines?
            client.Initialize().GetAwaiter ().GetResult ();
        }
    }

    public class BotClient {

        public DiscordSocketClient client;
        private string token;

        public CommandRoot commandRoot;

        public BotClient(string _token) {
            client = new DiscordSocketClient();
            token = _token;
        }

        public async Task Initialize () {
            Console.WriteLine ($"Initializing bot using token {token}..");
            try
            {
                Console.WriteLine("Logging in..");
                await client.LoginAsync(TokenType.Bot, token); // Usual Discord.NET bot initialization.
                Console.WriteLine("Starting..");
                await client.StartAsync();
            } catch (Exception exc)
            {
                Console.WriteLine("Everythings not groovy: " + exc.Message + " - " + exc.StackTrace);
            }
            Console.WriteLine("Initializing commands..");
            PopulateCommands (); // Initialize command root.
            client.MessageReceived += MessageRecievedEvent;
            Console.WriteLine ("Everythings groovy!");

            await Task.Delay(-1);
        }

        private void PopulateCommands() {

            commandRoot = new CommandRoot (
                new List<ICommand> {
                    new HelpCommand (),
                    new FlowCommandSet (),
                    new MathCommandSet (),
                    new VariableCommandSet (),
                    new CallstackCommand (),
                    new PrintCommand (),
                    new DiscordCommandSet (),
                }
            );

            commandRoot.InitializeCommands ();
        }

        private async Task MessageRecievedEvent(SocketMessage arg) {
            var result = await commandRoot.EnterCommand (arg.Content, arg, arg.GetGuild ().Id); // CommandRoot.EnterCommand takes in the full string and takes over from there.
            if (result.GetMessage () != null || result.Value as Embed != null) {
                await arg.Channel.SendMessageAsync (result == null ? "" : result.GetMessage () ?? "", false, result?.Value as Embed); // Command help in case of a set be in the message, and if a single command be as the results value as an Embed, because Embeds are cool.
            }
        }
    }
}
