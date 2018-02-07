using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Extensions
{
    static class UserExtensions
    {
        public static string GetShownName (this IUser user) {
            SocketGuildUser guildUser = user as SocketGuildUser;

            if (guildUser != null) {
                return String.IsNullOrEmpty (guildUser.Nickname) ? user.Username : guildUser.Nickname;
            } else {
                return user.Username;
            }
        }

        public static SocketRole GetColorRole (this SocketGuildUser user) {
            List <SocketRole> roles = user.Roles.ToList ();
            roles.Sort (new RoleComparer ());
            return roles.Find (x => { return x.Color.RawValue != Color.Default.RawValue; });
        }

        public static Color GetCollor (this SocketGuildUser user) {
            return GetColorRole (user).Color;
        }

        public static async Task AsyncSecureAddRole (this SocketGuildUser user, SocketRole role) {
            while (!user.Roles.Contains (role))
                await user.AddRoleAsync (role);
        }

        public static async Task AsyncSecureRemoveRole (this SocketGuildUser user, SocketRole role) {
            while (user.Roles.Contains (role))
                await user.RemoveRoleAsync (role);
        }

        public static bool HasAllPermissios (this SocketGuildUser user, List<GuildPermission> permissions) {
            bool hasAll = true;
            foreach (GuildPermission perm in permissions) {
                List<GuildPermission> userPermissions = user.GuildPermissions.ToList ();
                if (!user.GuildPermissions.ToList ().Contains (perm))
                    hasAll = false;
            }

            return hasAll;
        }
    }
}
