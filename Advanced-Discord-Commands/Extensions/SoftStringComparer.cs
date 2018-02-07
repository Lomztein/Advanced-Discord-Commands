using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Extensions
{
    class SoftStringComparer : IEqualityComparer<string> {
        public bool Equals(string x, string y) {
            if (x == null || y == null)
                return false;

            if (x.Length > y.Length) {
                return x.StartsWith (y);
            } else if (x.Length < y.Length) {
                return y.StartsWith (x);
            } else {
                return x == y;
            }
        }

        public int GetHashCode(string obj) {
            throw new NotImplementedException ();
        }
    }
}
