using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Tests.Fakes
{
    public class FakeUserMessage : IUserMessage {

        public MessageType Type => MessageType.Default;
        public MessageSource Source => MessageSource.User;

        public bool IsTTS => false;
        public bool IsPinned => false;

        public string Content { get; set; }

        public DateTimeOffset Timestamp => throw new NotImplementedException ();

        public DateTimeOffset? EditedTimestamp => throw new NotImplementedException ();

        public IMessageChannel Channel => new FakeChannel ();

        public IUser Author => throw new NotImplementedException ();

        public IReadOnlyCollection<IAttachment> Attachments => throw new NotImplementedException ();

        public IReadOnlyCollection<IEmbed> Embeds => throw new NotImplementedException ();

        public IReadOnlyCollection<ITag> Tags => throw new NotImplementedException ();

        public IReadOnlyCollection<ulong> MentionedChannelIds => throw new NotImplementedException ();

        public IReadOnlyCollection<ulong> MentionedRoleIds => throw new NotImplementedException ();

        public IReadOnlyCollection<ulong> MentionedUserIds => throw new NotImplementedException ();

        public DateTimeOffset CreatedAt => throw new NotImplementedException();

        public ulong Id => 0;

        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => throw new NotImplementedException ();

        public MessageActivity Activity => throw new NotImplementedException();

        public MessageApplication Application => throw new NotImplementedException();

        public Task DeleteAsync(RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task PinAsync(RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task UnpinAsync(RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task AddReactionAsync(IEmote emote, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task RemoveAllReactionsAsync(RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit = 100, ulong? afterUserId = null, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public string Resolve(TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name, TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name) {
            throw new NotImplementedException ();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public FakeUserMessage (string content) {
            Content = content;
        }
    }
}
