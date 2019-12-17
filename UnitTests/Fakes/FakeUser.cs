using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Tests.Fakes
{
    public class FakeUser : IUser
    {
        public string AvatarId => throw new NotImplementedException();

        public string Discriminator => "0000";

        public ushort DiscriminatorValue => 0;

        public bool IsBot => false;

        public bool IsWebhook => false;

        public string Username => "Test User";

        public DateTimeOffset CreatedAt => new DateTimeOffset (DateTime.Now);

        public ulong Id => 0;

        public string Mention => throw new NotImplementedException();

        public IActivity Activity => throw new NotImplementedException();

        public UserStatus Status => new UserStatus ();

        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
        {
            throw new NotImplementedException();
        }

        public string GetDefaultAvatarUrl()
        {
            throw new NotImplementedException();
        }

        public Task<IDMChannel> GetOrCreateDMChannelAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
