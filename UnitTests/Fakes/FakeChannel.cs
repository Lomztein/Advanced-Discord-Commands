﻿using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Tests.Fakes
{
    public class FakeChannel : IMessageChannel {

        public string Name => throw new NotImplementedException ();

        public DateTimeOffset CreatedAt => throw new NotImplementedException ();

        public ulong Id => throw new NotImplementedException ();

        public IDisposable EnterTypingState(RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task<IMessage> GetMessageAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task<IUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task<IUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, RequestOptions options = null) {
            throw new NotImplementedException ();
        }

        public Task<IUserMessage> SendMessageAsync(string text, bool isTTS = false, Embed embed = null, RequestOptions options = null) {
            return null;
        }

        public Task TriggerTypingAsync(RequestOptions options = null) {
            throw new NotImplementedException ();
        }
    }
}
