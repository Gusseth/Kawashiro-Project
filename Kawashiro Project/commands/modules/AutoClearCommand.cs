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
        private readonly GuildManager guildManager = Nitori.guildManager;
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
        public async Task AutoClear(string channelName = "view")
        {
            guild = Context.Guild;
            guildName = guild.Name;
            ulong channelID;

            if (channelName == "view")
            {
                await DisplayClearedChannels();
                return;
            }
            else if (MentionUtils.TryParseChannel(channelName, out channelID))
            {
                SocketTextChannel channel = guild.GetTextChannel(channelID);

                await ToggleChannel(channel, channelID);
            }
            else
            {
                await ReplyAsync(string.Format(LineManager.GetLine("ClearedChannelsNotExist"), guildName, channelName));
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
                await ToggleChannel(channel, channelID);
            }
            else
            {
                await ReplyAsync(string.Format(LineManager.GetLine("ClearedChannelsNotExist"), guildName, channelID));
            }
        }

        private async Task DisplayClearedChannels()
        {
            string avatarURL = Context.Client.CurrentUser.GetAvatarUrl();
            KappaGuild kGuild;

            if (guildManager.guilds.TryGetValue(guild.Id, out kGuild) && kGuild.clearedChannels.Count > 0)
            {
                EmbedBuilder embed = new EmbedBuilder()
                    .WithTitle($"**{guildName}** - Cleared Channels")
                    .WithDescription(string.Format(LineManager.GetLine("ClearedChannelsDisplay"), guildName))
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
                await ReplyAsync(string.Format(LineManager.GetLine("ClearedChannelsEmpty"), guildName));
            }
        }

        private async Task ToggleChannel(SocketTextChannel channel, ulong channelID)
        {
            KappaGuild kGuild;
            if (guildManager.guilds.TryGetValue(guild.Id, out kGuild))
            {
                if (kGuild.clearedChannels.Contains(channelID)) // Channel is removed if it is already being cleared
                {
                    kGuild.clearedChannels.Remove(channelID);
                    await ReplyAsync(string.Format(LineManager.GetLine("ClearedChannelsRemoved"), guildName, channel.Mention));
                }
                else 
                {
                    kGuild.clearedChannels.Add(channelID);
                    await ReplyAsync(string.Format(LineManager.GetLine("ClearedChannelsAdded"), guildName, channel.Mention));
                }
            }
            else
            {
                // When the guild doesn't exist in guilds.json
                kGuild = new KappaGuild(new ulong[] { channelID });
                guildManager.guilds.Add(guild.Id, kGuild);
                await ReplyAsync(string.Format(LineManager.GetLine("ClearedChannelsAdded"), guildName, channel.Mention));
            }
            await guildManager.ReplaceGuildsJson();
        }
    }
}
