using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Categories
{
    public class Category : ICategory {

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public override bool Equals(object obj) {
            if (obj is ICategory category) {
                return (category.Name == Name && category.Description == Description);
            }
            return false;
        }

        public override int GetHashCode() {
            return 0;
        }

        public Category (string _name, string _description) {
            Name = _name;
            Description = _description;
        }

    }
}
