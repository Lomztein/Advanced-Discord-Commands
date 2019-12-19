using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Categories
{
    public static class StandardCategories
    {
        public static readonly Category Uncategorised = new Category ("Uncategorised", "Uncateroised commands. These shouldn't exist, go scream at the dev.");
        public static readonly Category Utility = new Category ("Utility", "Commands that expand the base Discord functionality, by adding new or improving existing features through commands.");
        public static readonly Category Fun = new Category ("Fun", "Commands that are less useful and more about having silly fun.");
        public static readonly Category Admin = new Category ("Admin", "Commands that are reserved for administrators of servers, most often management and configuration commands.");
        public static readonly Category Miscellaneous = new Category ("Miscellaneous", "Miscellaneous commands that doesn't really fit anywhere and just do their own thing.");
        public static readonly Category Advanced = new Category ("Advanced", "Advanced commands created to use in complicated command chains, but rarely useful outside of command chains.");
    }

}
