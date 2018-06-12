using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Categories
{
    public static class StandardCategories
    {
        public static readonly Category Utility = new Category ("Utility", "Commands that expand the base Discord functionality, by adding new or improving existing features through commands.");

        public static readonly Category Fun = new Category ("Fun", "Commands that are less useful and more about having silly fun.");

        public static readonly Category Admin = new Category ("Admin", "Commands that are reserved for administrators of servers, most often management and configuration commands.");

        public static readonly Category Miscilaneous = new Category ("Miscilaneous", "Commands that are uncategorised for various reasons, most likely being that the author has forgotten to assign a category.");

        public static readonly Category Advanced = new Category ("Advanced", "Advanced commands created for use in complicated command chains, but rarely useful outside of command chains.");
    }

}
