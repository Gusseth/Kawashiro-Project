using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Kawashiro_Project.data;
using Kawashiro_Project.data.entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class AutoClearCommand : ModuleBase<SocketCommandContext>
    {
        private readonly GuildManager guildManager = Nitori.GuildManager;
        private SocketGuild guild;
        private string guildName;

        /// <summary>
        /// Toggles the channel for auto-deletion when everyone disconnects from all calls. Uses mentions.
        /// </summary>
        /// <param name="channelName">String that complies with the mention notation for channels</param>
        /// <returns></returns>
        [Command("AutoClear", true)]
        [Alias("AC")]
        [Summary("Toggles the channel for auto-deletion when everyone disconnects from all calls. Uses mentions.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AutoClear(string channelName)
        {
            guild = Context.Guild;
            guildName = guild.Name;
            ulong channelID;

            if (MentionUtils.TryParseChannel(channelName, out channelID))
            {
                SocketTextChannel channel = guild.GetTextChannel(channelID);

                await ToggleChannel(channel);
            }
            else
            {
                await ReplyAsync(string.Format(ResponseManager.GetLine("ClearedChannelsNotExist"), guildName, channelName));
            }
        }

        /// <summary>
        /// Toggles the channel for auto-deletion when everyone disconnects from all calls. Uses channel IDs.
        /// </summary>
        /// <param name="channelID">The ID of the channel</param>
        /// <returns></returns>
        [Command("AutoClear", true)]
        [Alias("AC")]
        [Summary("Toggles the channel for auto-deletion when everyone disconnects from all calls. Uses channel IDs.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AutoClear(ulong channelID)
        {
            guild = Context.Guild;
            guildName = guild.Name;
            SocketTextChannel channel = guild.GetTextChannel(channelID);

            if (channel != null)
            {
                await ToggleChannel(channel);
            }
            else
            {
                await ReplyAsync(string.Format(ResponseManager.GetLine("ClearedChannelsNotExist"), guildName, channelID));
            }
        }

        /// <summary>
        /// Displays the channels that are automatically cleared.
        /// </summary>
        /// <returns></returns>
        [Command("AutoClear", true)]
        [Alias("AC")]
        [Summary("Displays the channels that are automatically cleared.")]
        public async Task AutoClear()
        {
            guild = Context.Guild;
            guildName = guild.Name;
            await DisplayClearedChannels();
        }

        private async Task DisplayClearedChannels()
        {
            string avatarURL = Context.Client.CurrentUser.GetAvatarUrl();
            KappaGuild kGuild;

            if (guildManager.guilds.TryGetValue(guild.Id, out kGuild) && kGuild.clearedChannels.Count > 0)
            {
                EmbedBuilder embed = new EmbedBuilder()
                    .WithTitle($"**{guildName}** - Cleared Channels")
                    .WithDescription(string.Format(ResponseManager.GetLine("ClearedChannelsDisplay"), guildName))
                    .WithAuthor(author =>
                    {
                        author
                            .WithName("The Kawashiro Project")
                            .WithUrl("https://github.com/Gusseth/Kawashiro-Project/tree/master/Kawashiro%20Project")
                            .WithIconUrl(avatarURL);
                    })
                    .WithColor(new Color(0x67BE67))
                    .WithCurrentTimestamp()
                    .WithFooter(footer =>
                    {
                       footer
                            .WithText("A bot tailored for /r/weather")
                            .WithIconUrl(avatarURL);
			        });
                foreach (ulong channelID in kGuild.clearedChannels)
                {
                    SocketTextChannel channel = guild.GetTextChannel(channelID);
                    embed.AddField(channel.Name, $"{channel.Mention} - " + (channel.Topic ?? $"A channel in {guildName}."));
                }

                await ReplyAsync(null, false, embed.Build());
            }
            else
            {
                await ReplyAsync(string.Format(ResponseManager.GetLine("ClearedChannelsEmpty"), guildName));
            }
        }

        /// <summary>
        /// Checks the given channel if it's already autocleared.
        ///     if true, then remove the channel from auto-clear
        ///     if false, then add the channel to auto-clear
        ///     if the guild does not exist, make the guild and put the channel to auto-clear
        /// </summary>
        /// <param name="channel">The text channel to be toggled.</param>
        /// <returns></returns>
        private async Task ToggleChannel(SocketTextChannel channel)
        {
            ulong channelID = channel.Id;
            if (guildManager.guilds.TryGetValue(guild.Id, out KappaGuild kGuild))
            {
                if (kGuild.clearedChannels.Contains(channelID)) // Channel is removed if it is already being cleared
                {
                    kGuild.clearedChannels.Remove(channelID);
                    await ReplyAsync(string.Format(ResponseManager.GetLine("ClearedChannelsRemoved"), guildName, channel.Mention));
                }
                else
                {
                    kGuild.clearedChannels.Add(channelID);
                    await ReplyAsync(string.Format(ResponseManager.GetLine("ClearedChannelsAdded"), guildName, channel.Mention));
                }
            }
            else
            {
                // When the guild doesn't exist in guilds.json
                kGuild = new KappaGuild(new ulong[] { channelID });
                guildManager.guilds.Add(guild.Id, kGuild);
                await ReplyAsync(string.Format(ResponseManager.GetLine("ClearedChannelsAdded"), guildName, channel.Mention));
            }
            await guildManager.ReplaceGuildsJson();
        }
    }
}
