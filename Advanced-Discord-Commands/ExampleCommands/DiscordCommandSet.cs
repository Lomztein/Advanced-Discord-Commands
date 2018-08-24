using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;

namespace Lomztein.AdvDiscordCommands.ExampleCommands
{
    public class DiscordCommandSet : CommandSet
    {
        public DiscordCommandSet() {
            Name = "discord";
            Description = "Discord-related commands.";
            Category = StandardCategories.Advanced;

            commandsInSet = new List<ICommand> {
                new UserSet (), new ChannelSet (), new RoleSet (), new ServerSet (),
                new Mention (), new ID (), new Delete (),
            };
        }

        public class UserSet : CommandSet {
            public UserSet() {
                base.Name = "user";
                Description = "User related commands.";
                Category = StandardCategories.Advanced;

                commandsInSet = new List<ICommand> {
                    new Find (), new Random (), new Username (), new Online (), new Kick (), new Nickname (),
                    new AddRole (), new RemoveRole (), new DM (), new Move (), new SetVoice (),
                };
            }

            public class Find : Command {
                public Find() {
                    Name = "find";
                    Description = "Use this to find users.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (SocketGuildUser), "Find user by ID")]
                public async Task<Result> Execute(CommandMetadata e, ulong id) {
                    IUser user = await e.Message.Channel.GetUserAsync (id);
                    return new Result (user, user.GetShownName ());
                }

                [Overload (typeof (SocketGuildUser), "Find user by name.")]
                public Task<Result> Execute(CommandMetadata e, string name) {
                    IUser user = (e.Message.GetGuild ()?.Users.Where (x => x.GetShownName () == name).FirstOrDefault ());
                    return TaskResult (user, user.GetShownName ());
                }

                [Overload (typeof (SocketGuildUser [ ]), "Find users by role.")]
                public Task<Result> Execute(CommandMetadata e, SocketRole role) {
                    return TaskResult (role.Guild.Users.Where (x => x.Roles.Contains (role)).ToArray (), "");
                }
            }

            public class Random : Command {
                public Random() {
                    Name = "random";
                    Description = "Get random user.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (SocketGuildUser), "Get a completely random online user from the server.")]
                public Task<Result> Execute(CommandMetadata e) {
                    System.Random random = new System.Random ();
                    IEnumerable<SocketGuildUser> users = (e.Message.GetGuild ()?.Users.Where (x => x.Status == UserStatus.Online));
                    return TaskResult (users.ElementAt (random.Next (0, users.Count ())), "");
                }

                [Overload (typeof (SocketGuildUser), "Get a random online user who is a member of the given role.")]
                public Task<Result> Execute(CommandMetadata e, SocketRole role) {
                    System.Random random = new System.Random ();
                    IEnumerable<SocketGuildUser> users = role.Guild.Users.Where (x => x.Roles.Contains (role)).Where (x => x.Status == UserStatus.Online);
                    return TaskResult (users.ElementAt (random.Next (0, users.Count ())), "");
                }

                [Overload (typeof (SocketGuildUser), "Get a random user from the given array of users.")]
                public Task<Result> Execute(CommandMetadata e, params IUser [ ] users) {
                    System.Random random = new System.Random ();
                    return TaskResult (users.ElementAt (random.Next (0, users.Count ())), "");
                }
            }

            public class Username : Command {

                public Username() {
                    Name = "name";
                    Description = "Get user name.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (string), "Get the name of a user, nickname if there is one.")]
                public Task<Result> Execute(CommandMetadata e, IUser user) {
                    string name = user.GetShownName ();
                    return TaskResult (name, name);
                }
            }

            public class Online : Command {
                public Online() {
                    Name = "online";
                    Description = "Is user online.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (bool), "Returns true if the given user by name is online, false if not.")]
                public Task<Result> Execute(CommandMetadata e, IUser user) {
                    if (user != null) {
                        return TaskResult (user.Status == UserStatus.Online, "");
                    } else {
                        return TaskResult (false, "User not found.");
                    }
                }
            }

            public class Kick : Command {
                public Kick() {
                    Name = "kick";
                    Description = "Kicks a user if permitted.";
                    Category = StandardCategories.Advanced;
                    RequiredPermissions.Add (GuildPermission.KickMembers);
                }

                [Overload (typeof (bool), "Kicks user for no given reason.")]
                public Task<Result> Execute(CommandMetadata e, SocketGuildUser user) {
                    return Execute (e, user, "");
                }

                [Overload (typeof (bool), "Kicks user with a reason.")]
                public async Task<Result> Execute(CommandMetadata e, SocketGuildUser user, string reason) {
                    try {
                    await user.KickAsync (reason);
                    return new Result (true, $"Succesfully kicked **{user.Username}** from the server.");
                    } catch (Exception exc) {
                        return new Result (exc, $"Failed to kick {user.GetShownName ()} - " + exc.Message);
                    }
                }
            }

            public class Nickname : Command {
                public Nickname() {
                    Name = "nickname";
                    Description = "Set someones nickname.";
                    Category = StandardCategories.Advanced;

                    RequiredPermissions.Add (GuildPermission.ManageNicknames);
                }

                [Overload (typeof (bool), "Set the given users nickname to something new.")]
                public async Task<Result> Execute(CommandMetadata e, SocketGuildUser user, string nickname) {
                    try {
                        await user.ModifyAsync (delegate (GuildUserProperties properties) {
                            properties.Nickname = nickname;
                        });
                        if (nickname == string.Empty) {
                            return new Result (true, $"Succesfully reset **{user.Username}**'s nickname.");
                        }
                        return new Result (true, $"Succesfully set **{user.Username}**'s nickname to **{nickname}**.");
                    } catch (Exception exc) {
                        return new Result (exc, $"Failed to set {user.GetShownName ()}'s nickname - " + exc.Message);
                    }
                }

                [Overload (typeof (bool), "Reset the given users nickname.")]
                public Task<Result> Execute(CommandMetadata e, SocketGuildUser user) {
                    return Execute (e, user, "");
                }
            }

            public class AddRole : Command {

                public AddRole() {
                    Name = "addrole";
                    Description = "Adds roles to someone.";
                    Category = StandardCategories.Advanced;
                    RequiredPermissions.Add (GuildPermission.ManageRoles);
                }

                [Overload (typeof (bool), "Add all given roles to the given person.")]
                public async Task<Result> Execute(CommandMetadata e, SocketGuildUser user, params SocketRole[] roles) {
                    try {
                        foreach (SocketRole role in roles) {
                            await user.AsyncSecureAddRole (role);
                        }
                        return new Result (true, $"Succesfully added **{roles.Length}** roles to **{user.GetShownName ()}**!");
                    } catch (Exception exc) {
                        return new Result (exc, $"Could not add roles - " + exc.Message);
                    }
                }
            }

            public class RemoveRole : Command {

                public RemoveRole() {
                    Name = "removerole";
                    Description = "Removes roles from someone.";
                    Category = StandardCategories.Advanced;

                    RequiredPermissions.Add (GuildPermission.ManageRoles);
                }

                [Overload (typeof (bool), "Add all given roles to the given person.")]
                public async Task<Result> Execute(CommandMetadata e, SocketGuildUser user, params SocketRole [ ] roles) {
                    try {
                        foreach (SocketRole role in roles) {
                            await user.AsyncSecureAddRole (role);
                        }
                        return new Result (true, $"Succesfully added **{roles.Length}** roles to **{user.GetShownName ()}**!");
                    } catch (Exception exc) {
                        return new Result (exc, $"Could not add roles - " + exc);
                    }
                }
            }

            public class DM : Command {
                public DM() {
                    Name = "dm";
                    Description = "DM's a person.";
                    Category = StandardCategories.Advanced;
                    RequiredPermissions.Add (GuildPermission.Administrator);

                    Aliases = new string[] { "pm" };
                }

                [Overload (typeof (IUserMessage), "Sends a DM to the given person with the given text.")]
                [Example ("IUserMessage", "Succesfully sent a DM.", "@Usermention", "Hello Usermention!")]
                public async Task<Result> Execute(CommandMetadata e, SocketGuildUser user, string text) {
                    IUserMessage message = await user.SendMessageAsync (text);
                    return new Result (message, "Succesfully send a DM.");
                }
            }

            public class Move : Command {
                public Move() {
                    Name = "move";
                    Description = "Move to voice channel.";
                    Category = StandardCategories.Advanced;
                    RequiredPermissions.Add (GuildPermission.MoveMembers);
                }

                [Overload (typeof (SocketVoiceChannel), "Moves a user to a different voice channel. Must be in one to begin with.")]
                public async Task<Result> Execute(CommandMetadata e, SocketGuildUser user, SocketVoiceChannel newChannel) {
                    try {
                        if (user.VoiceChannel != null) {
                            SocketVoiceChannel previous = user.VoiceChannel;
                            await user.ModifyAsync (delegate (GuildUserProperties properties) {
                                properties.Channel = newChannel;
                            });
                            return new Result (previous, $"Succesfully moved **{user.GetShownName ()}** to channel **{newChannel.Name}**");
                        }
                        return new Result (null, $"Failed to move **{user.GetShownName ()}**, since he isn't currently in voice.");
                    }catch (Exception exc) {
                        return new Result (exc, $"Failed to move **{user.GetShownName ()}** - " + exc.Message);
                    }
                } 
            }

            public class SetVoice : Command {
                public SetVoice() {
                    Name = "setvoice";
                    Description = "Servermute/deafen someone.";
                    Category = StandardCategories.Advanced;

                    RequiredPermissions.Add (GuildPermission.MoveMembers);
                    RequiredPermissions.Add (GuildPermission.DeafenMembers);
                }

                // Optionables are neat, but they don't mesh particularily well with commands. Could perhaps use reflection.
                [Overload (typeof (bool), "Set mute on the given person!")]
                public Task<Result> Execute(CommandMetadata e, SocketGuildUser user, bool mute) {
                    try {
                        user.ModifyAsync (delegate (GuildUserProperties properties) {
                            properties.Mute = mute;
                        });
                        return TaskResult (true, $"Succesfully changed voice status of **{user.GetShownName ()}**.");
                    } catch (Exception exc) {
                        return TaskResult (exc, $"Failed to change voice status of **{user.GetShownName ()}** - " + exc.Message);
                    }
                }

                [Overload (typeof (bool), "Set mute or deafen on the given person, or both at once!")]
                public Task<Result> Execute(CommandMetadata e, SocketGuildUser user, bool mute, bool deafen) {
                    try {
                        user.ModifyAsync (delegate (GuildUserProperties properties) {
                            properties.Mute = mute;
                            properties.Deaf = deafen;
                        });
                        return TaskResult (true, $"Succesfully changed voice status of **{user.GetShownName ()}**.");
                    } catch (Exception exc) {
                        return TaskResult (exc, $"Failed to change voice status of **{user.GetShownName ()}** - " + exc.Message);
                    }
                }
            }
        }

        public class RoleSet : CommandSet {

            public RoleSet() {
                Name = "role";
                Description = "Role related commands.";
                Category = StandardCategories.Advanced;

                commandsInSet = new List<ICommand> {
                    new Find (), new Members (),
                };
            }

            public class Find : Command {
                public Find() {
                    Name = "find";
                    Description = "Find role.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (SocketRole), "Find role by given ID.")]
                public Task<Result> Execute(CommandMetadata e, ulong id) {
                    return TaskResult (e.Message.GetGuild ()?.GetRole (id), "");
                }

                [Overload (typeof (SocketRole), "Find role by given name.")]
                public Task<Result> Execute(CommandMetadata e, string rolename) {
                    return TaskResult (e.Message.GetGuild ()?.Roles.Where (x => x.Name.ToUpper () == rolename.ToUpper ()).FirstOrDefault (), "");
                }

                [Overload (typeof (SocketRole [ ]), "Get all roles of the given user.")]
                public Task<Result> Execute(CommandMetadata e, SocketGuildUser user) {
                    return TaskResult (user.Roles.ToArray (), "");
                }
            }

            public class Members : Command {
                public Members() {
                    Name = "members";
                    Description = "Get role members.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (SocketGuildUser), "Get all members of the given role.")]
                public Task<Result> Execute(CommandMetadata e, SocketRole role) {
                    return TaskResult (role.Members.ToArray (), "");
                }
            }
        }

        public class ChannelSet : CommandSet {

            public ChannelSet() {
                base.Name = "channel";
                Description = "Channel related commands.";
                Category = StandardCategories.Advanced;

                commandsInSet = new List<ICommand> {
                    new Find (), new Members (), new Channelname (), new Type (), new Create (),
                };
            }

            public class Find : Command {
                public Find() {
                    Name = "find";
                    Description = "Find channel.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (SocketChannel), "Find channel by given ID.")]
                public Task<Result> Execute(CommandMetadata e, ulong id) {
                    return TaskResult (e.Message.GetGuild ()?.GetChannel (id), "");
                }

                [Overload (typeof (SocketChannel), "Find channel by given name.")]
                public Task<Result> Execute(CommandMetadata e, string name) {
                    SoftStringComparer comparer = new SoftStringComparer ();
                    return TaskResult (e.Message.GetGuild ()?.Channels.Where (x => comparer.Equals (x.Name, name)).FirstOrDefault (), "");
                }
            }

            public class Members : Command {
                public Members() {
                    Name = "members";
                    Description = "Get channel members.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (SocketGuildUser [ ]), "Get")]
                public Task<Result> Execute(CommandMetadata e, SocketChannel channel) {
                    return TaskResult (channel.Users.ToArray (), "");
                }
            }

            public class Channelname : Command {
                public Channelname() {
                    Name = "name";
                    Description = "Get channel name.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (string), "Get the name of the given channel")]
                public Task<Result> Execute(CommandMetadata e, SocketGuildChannel channel) {
                    return TaskResult (channel.Name, "");
                }
            }

            public class Type : Command {
                public Type() {
                    Name = "type";
                    Description = "Get channel type.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (SocketGuildChannel), "Get the type of given channel, either \"TEXT\", \"VOICE\", or \"CATEGORY\".")]
                public Task<Result> Execute(CommandMetadata e, SocketGuildChannel channel) {
                    if (channel is SocketVoiceChannel) {
                        return TaskResult (channel as SocketVoiceChannel, "VOICE");
                    } else if (channel is SocketTextChannel) {
                        return TaskResult (channel as SocketTextChannel, "TEXT");
                    }
                    return TaskResult ("CATEGORY", "CATEGORY");
                }
            }

            public class Create : Command {
                public Create() {
                    Name = "create";
                    Description = "Create new text channel.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (ITextChannel), "Create a new text channel with the given name.")]
                public async Task<Result> Execute(CommandMetadata e, string name, string topic) {
                    try {
                        RestTextChannel channel = await (e.Message.Channel as SocketTextChannel)?.Guild.CreateTextChannelAsync (name);
                        await channel.ModifyAsync (delegate (TextChannelProperties properties) {
                            properties.Topic = topic;
                        });
                        return new Result (channel, "Succesfully created a new text channel: " + channel.Mention);
                    } catch (Exception exc) {
                        return new Result (exc, "Failed to create new channel - " + exc.Message);
                    }
                }

                [Overload (typeof (ITextChannel), "Create a new text channel with the given name and topic.")]
                public async Task<Result> Execute(CommandMetadata e, string name) {
                    try {
                        RestTextChannel channel = await (e.Message.Channel as SocketTextChannel)?.Guild.CreateTextChannelAsync (name);
                        return new Result (channel, "Succesfully created a new text channel: " + channel.Mention);
                    } catch (Exception exc) {
                        return new Result (exc, "Failed to create new channel - " + exc.Message);
                    }
                }
            }
        }

        public class ServerSet : CommandSet {

            public ServerSet() {
                Name = "server";
                Description = "Server related commands.";
                Category = StandardCategories.Advanced;

                commandsInSet = new List<ICommand> {
                    new Get (), new Servername (), new Channels (), new Members (), new AFKChannel (),
                };
            }

            public class Get : Command {
                public Get() {
                    Name = "get";
                    Description = "Returns the server object.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (SocketGuild), "Returns the server object.")]
                public Task<Result> Execute(CommandMetadata e) {
                    SocketGuild guild = (e.Message.Channel as SocketTextChannel)?.Guild;
                    return TaskResult (guild, guild.Name);
                }
            }

            public class Servername : Command {

                public Servername() {
                    Name = "name";
                    Description = "Get server name.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (string), "Returns the server name.")]
                public Task<Result> Execute(CommandMetadata e) {
                    SocketGuild guild = (e.Message.Channel as SocketTextChannel)?.Guild;
                    return TaskResult (guild.Name, guild.Name);
                }
            }

            public class Channels : Command {
                public Channels() {
                    Name = "channels";
                    Description = "Get all channels.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (SocketGuildChannel [ ]), "Returns all channels on the server.")]
                public Task<Result> Execute(CommandMetadata e) {
                    SocketGuild guild = (e.Message.Channel as SocketTextChannel)?.Guild;
                    return TaskResult (guild.Channels.ToArray (), "");
                }
            }

            public class Members : Command {
                public Members() {
                    Name = "members";
                    Description = "Get all members.";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (SocketGuildUser [ ]), "Returns all members on the server.")]
                public Task<Result> Execute(CommandMetadata e) {
                    SocketGuild guild = (e.Message.Channel as SocketTextChannel)?.Guild;
                    string members = "";
                    guild?.Users.ToList ().ForEach (x => members += x.GetShownName () + ", ");

                    return TaskResult (guild?.Users.ToArray (), members);
                }
            }

            public class AFKChannel : Command {
                public AFKChannel() {
                    Name = "afkchannel";
                    Description = "Get AFK Channel";
                    Category = StandardCategories.Advanced;
                }

                [Overload (typeof (SocketVoiceChannel), "Get the AFK channel if there is one, returns null otherwise.")]
                public Task<Result> Execute(CommandMetadata e) {
                    SocketGuild guild = e.Message.GetGuild ();
                    return TaskResult (guild?.AFKChannel, guild?.AFKChannel.Name);
                }
            }
        }

        public class Mention : Command {
            public Mention() {
                Name = "mention";
                Description = "Mentions given objects.";
                    Category = StandardCategories.Advanced;
            }

            [Overload (typeof (string), "Mention all given objects.")]
            public Task<Result> Execute(CommandMetadata e, params IMentionable[] mentionables) {
                string total = "";
                foreach (IMentionable mention in mentionables) {
                    total += mention.Mention + "\n";
                }
                return TaskResult (total, total);
            }
        }

        public class ID : Command {
            public ID() {
                Name = "id";
                Description = "Get the ID of given object.";
                    Category = StandardCategories.Advanced;
            }

            [Overload (typeof (ulong), "Return the ID of the given object.")]
            public Task<Result> Execute(CommandMetadata e, IEntity<ulong> obj) {
                return TaskResult (obj.Id, obj.Id.ToString ());
            }
        }

        public class Delete : Command {
            public Delete() {
                Name = "delete";
                Description = "Delete deletable objects.";
                Category = StandardCategories.Advanced;

                RequiredPermissions.Add (GuildPermission.ManageChannels);
                RequiredPermissions.Add (GuildPermission.ManageMessages);
            }

            [Overload (typeof (object), "Delete whatever deletable object is given.")]
            public async Task<Result> Execute(CommandMetadata e, IDeletable deletable) {
                await deletable.DeleteAsync ();
                return new Result (null, "Succesfully deleted the given object.");
            }
        }
    }
}
