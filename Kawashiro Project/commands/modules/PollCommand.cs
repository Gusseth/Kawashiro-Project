using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class PollCommand : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Makes the bot reply with the same message
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [Command("Poll")]
        [Summary("Makes the bot reply with the same message.")]
        public async Task Poll(string title, string prompt, params string[] options)
        {
            await Context.Message.DeleteAsync();
            SocketUser pollOwner = Context.User;
            SocketUser bot = Context.Client.CurrentUser;
            IEmote[] emotes = new IEmote[options.Length];

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(prompt)
                .WithAuthor(author => author.WithName($"{pollOwner.Username} started a poll:").WithIconUrl(pollOwner.GetAvatarUrl()))
                .WithColor(6798951)
                .WithCurrentTimestamp()
                .WithFooter(footer => footer.WithText("A bot tailored for /r/weather").WithIconUrl(bot.GetAvatarUrl()));


            int count = 0;  // Too lazy to make an actual for loop lmao
            foreach (string option in options)
            {
                string emoteString = Nitori.Config.pollEmotes[count];
                IEmote emote;
                if (Emote.TryParse(emoteString, out Emote e)) emote = e;
                else emote = new Emoji(emoteString);

                embed.AddField($"{emote.Name} " + option, $"Emote below to pick '**{option}**'.");

                emotes[count] = emote;
                count++;
            }
            IUserMessage message = await ReplyAsync(embed: embed.Build());
            await message.AddReactionsAsync(emotes);
            await Task.Delay(10 * 1000);

            foreach (IEmote emote in emotes)
            {
                ReactionMetadata meta;
                message.Reactions.TryGetValue(emote, out meta);
                

            }
            //await message.Reac
        }
    }
}
