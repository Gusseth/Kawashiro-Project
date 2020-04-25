using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class PollCommand : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Creates a 20-second poll using reactions.
        /// </summary>
        /// <param name="title">Title of the poll.</param>
        /// <param name="prompt">The question the poll surrounds.</param>
        /// <param name="options">The options for this poll.</param>
        /// <returns></returns>
        [Command("Poll")]
        [Summary("Creates a 20-second poll.")]
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
            await Task.Delay(20 * 1000);


            message = await Context.Channel.GetMessageAsync(message.Id) as IUserMessage;    // Download message again to get the updated reacts
            
            int[] score = new int[options.Length];
            int winningVote = 0;

            count = 0;
            foreach (IEmote emote in emotes)
            {
                int votes = message.Reactions[emote].ReactionCount - 1; // -1 because of the bot also counting itself.
                score[count] = votes;
                count++;
            }

            winningVote = score.Max();

            EmbedBuilder resultEmbed = new EmbedBuilder()
                .WithTitle($"Results for {title}")
                .WithAuthor(author => author.WithName($"{pollOwner.Username}'s poll:").WithIconUrl(pollOwner.GetAvatarUrl()))
                .WithCurrentTimestamp()
                .WithFooter(footer => footer.WithText("A bot tailored for /r/weather").WithIconUrl(bot.GetAvatarUrl()));
            if (score.All(x => x == 0))
            {
                resultEmbed.WithColor(13576232)
                    .WithDescription("❌ No one voted!");
            }
            else if (score.Count(x => x == winningVote) > 1)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    if (score[i] == winningVote)
                    {
                        IEmote emote = emotes[i];
                        resultEmbed.AddField($"{emote.Name} - {options[i]}", 
                            $"{winningVote} votes.",
                            true);
                    }
                }

                resultEmbed.WithColor(13576232)
                    .WithDescription("❌ A tie in the vote occurred!");
            }
            else
            {
                int index = Array.IndexOf(score, winningVote);
                IEmote emote = emotes[index];
                resultEmbed.WithColor(6798951)
                    .WithDescription("✅ Winner winner, prison dinner:")
                    .AddField($"{emote.Name} - {options[index]}", $"{winningVote} votes.");
            }

            // Failed attempt to make a unique vote guard below
            /*
            Dictionary<IEmote, List<IUser>> userList = new Dictionary<IEmote, List<IUser>>();

            foreach (IEmote emote in emotes)
            {
                userList.Add(emote, new List<IUser>(await message.GetReactionUsersAsync(emote, count + 1).FlattenAsync()));
                userList[emote].Remove(Context.Client.CurrentUser);
            }

            List<IUser> duplicateUsers = userList.Values.Cast<IEnumerable<IUser>>() // List of users that voted more than once
                .Aggregate((accumulator, list) => accumulator.Concat(list)) // Concatenates every list into one
                .GroupBy(x => Context.Guild.GetUser(x.Id) as IUser)         // Groups elements based on if they're the same element
                .Where(g => g.Count() > 1)  // Filters groups that only appear once (distinct elements)
                .Select(g => g.Key)         // ??? what the fuck is this
                .ToList();                  // Converts to list

            List<int> scores = new List<int>();
            int winner = 0;
            foreach (List<IUser> users in userList.Values)
            {
                foreach (IUser loser in duplicateUsers)
                {
                    users.Remove(loser);
                }
                scores.Add(users.Count);
            }
            scores.ForEach(x => winner = Math.Max(winner, x));

            foreach (IUser loser in duplicateUsers) {
                foreach (List<IUser> users in userList.Values)
                {
                    users.Remove(loser);
                }
            }
            */
            await ReplyAsync(embed: resultEmbed.Build());
            //await message.Reac
        }
    }
}
