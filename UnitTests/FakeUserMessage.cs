using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Tests.Fakes
{
    public class FakeUserMessage : IMessage {

        public MessageType Type => MessageType.Default;
        public MessageSource Source => MessageSource.User;

        public bool IsTTS => false;
        public bool IsPinned => false;

        public string Content { get; set; }

        public DateTimeOffset Timestamp => throw new NotImplementedException ();

        public DateTimeOffset? EditedTimestamp => throw new NotImplementedException ();

        public IMessageChannel Channel => throw new NotImplementedException ();

        public IUser Author => throw new NotImplementedException ();

        public IReadOnlyCollection<IAttachment> Attachments => throw new NotImplementedException ();

        public IReadOnlyCollection<IEmbed> Embeds => throw new NotImplementedException ();

        public IReadOnlyCollection<ITag> Tags => throw new NotImplementedException ();

        public IReadOnlyCollection<ulong> MentionedChannelIds => throw new NotImplementedException ();

        public IReadOnlyCollection<ulong> MentionedRoleIds => throw new NotImplementedException ();

        public IReadOnlyCollection<ulong> MentionedUserIds => throw new NotImplementedException ();

        public DateTimeOffset CreatedAt => throw new NotImplementedException();

        public ulong Id => 0;

        public Task DeleteAsync(RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public FakeUserMessage (string content) {
            Content = content;
        }
    }
}
