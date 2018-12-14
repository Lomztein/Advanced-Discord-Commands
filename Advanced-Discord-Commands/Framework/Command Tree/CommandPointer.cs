﻿using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework {

    class CommandPointer : ICommand
    {
        public ICommand AtPointer { get; private set; }

        public ICategory Category { get => AtPointer.Category; set => AtPointer.Category = value; }

        public bool AvailableInDM { get => AtPointer.AvailableInDM; set => AtPointer.AvailableInDM = value; }
        public bool AvailableOnServer { get => AtPointer.AvailableOnServer; set => AtPointer.AvailableOnServer = value; }
        public bool CommandEnabled { get => AtPointer.CommandEnabled; set => AtPointer.CommandEnabled = value; }

        public List<GuildPermission> RequiredPermissions { get => AtPointer.RequiredPermissions; set => AtPointer.RequiredPermissions = value; }

        public ICommandParent CommandParent { get => AtPointer.CommandParent; set => AtPointer.CommandParent = value; }

        public string Name { get; set; }
        public string Description { get => AtPointer.Description; set => AtPointer.Description = value; }
        public string[] Aliases { get => AtPointer.Aliases; set => AtPointer.Aliases = value; }
        public string Flatname { get => AtPointer.Flatname; set => AtPointer.Flatname = value; }

        public CommandPointer (Command atPointer, string flatname) {
            AtPointer = atPointer;
            Name = flatname;
        }

        public string AllowExecution(CommandMetadata metadata) {
            return AtPointer.AllowExecution (metadata);
        }

        public string GetCommand(ulong? owner) {
            return AtPointer.GetCommand (owner);
        }

        public Embed GetDocumentationEmbed(CommandMetadata metadata) {
            return AtPointer.GetDocumentationEmbed (metadata);
        }

        public CommandOverload[] GetOverloads() {
            return AtPointer.GetOverloads ();
        }

        public void Initialize() { } // Since these point at an already initialized command, it wouldn't make sense to initialize them again.

        public bool IsCommand(string name) {
            return AtPointer.IsCommand (name);
        }

        public Task<Result> TryExecute(CommandMetadata data, params object[] arguments) {
            return AtPointer.TryExecute (data, arguments);
        }
    }

}
