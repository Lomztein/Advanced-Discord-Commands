using Lomztein.AdvDiscordCommands.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Lomztein.AdvDiscordCommands.Framework {

    public static class CommandVariables {

        public static Dictionary<ulong, Dictionary<string, object>> variables = new Dictionary<ulong, Dictionary<string, object>> ();
        public static string [ ] reservedNames = new string [ ] {
            "arg", "for", "x",
        };

        public static object Get(ulong ID, string name) {
            if (variables.ContainsKey (ID)) {
                if (variables [ ID ].ContainsKey (name)) {
                    return variables [ ID ] [ name ];
                }
            }
            return null;
        }

        public static async Task<object> AsyncGet(ulong ID, string name, int maxWaitSecs = 10) {
            if (variables.ContainsKey (ID)) {
                int index = 0;
                while (!variables [ ID ].ContainsKey (name)) {
                    await Task.Delay (1000);
                    if (index > maxWaitSecs)
                        break;
                    index++;
                }
                if (variables [ ID ].ContainsKey (name))
                    return variables [ ID ] [ name ];
            }
            return null;
        }

        public static void Set(ulong ID, string name, object obj, bool allowReserved = false) {
            SoftStringComparer softie = new SoftStringComparer ();
            if (!allowReserved && reservedNames.Any (x => softie.Equals (x, name))) {
                throw new ArgumentException ($"Cannot use name {name}, as it is reserved.");
            }

            if (!variables.ContainsKey (ID)) {
                variables.Add (ID, new Dictionary<string, object> ());
            }

            if (variables [ ID ].ContainsKey (name)) {
                variables [ ID ] [ name ] = obj;
            } else {
                variables [ ID ].Add (name, obj);
            }
        }

        public static bool Delete(ulong ID, string name) {
            if (variables.ContainsKey (ID)) {
                if (variables [ ID ].ContainsKey (name)) {
                    // Make sure eventual disposeable variables are properly disposed.
                    IDisposable disposable = variables [ ID ] [ name ] as IDisposable;
                    disposable?.Dispose ();

                    variables [ ID ].Remove (name);
                    return true;
                }
            }
            return false;
        }

        public static bool Clear(ulong ID) {
            if (variables.ContainsKey (ID)) {
                for (int i = 0; i < variables [ ID ].Count; i++) {
                    var element = variables [ ID ].ElementAt (0);
                    Delete (ID, element.Key);
                }
                return true;
            }
            return false;
        }
    }
}