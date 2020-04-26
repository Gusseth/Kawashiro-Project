using Discord;
using Discord.Commands;
using Discord.Rest;
using Kawashiro_Project.data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class AboutCommand : ModuleBase<SocketCommandContext>
	{
		private static Embed embedSingleton; // This will be the only instance as you cannot define commands at the time of writing.

		/// <summary>
		/// Displays an embed that shows a description about the bot, the project, and a list of commands
		/// </summary>
		/// <returns></returns>
        [Command("About", true)]
		[Alias("Help")]
        public async Task About()
        {
			string avatarURL = Context.Client.CurrentUser.GetAvatarUrl();

			embedSingleton ??= new EmbedBuilder()
			.WithTitle("**The Kawashiro Project** - A Multipurpose Bot")
			.WithDescription(string.Format(ResponseManager.GetLine("About"), "Nitori Kawashiro", Nitori.version))
			.WithUrl("https://github.com/Gusseth/Kawashiro-Project/tree/master/Kawashiro%20Project")
			.WithColor(6798951)
			.WithCurrentTimestamp()
			.WithFooter(footer => {
				footer
					.WithText("A bot tailored for /r/weather")
					.WithIconUrl(avatarURL);
			})
			.WithThumbnailUrl("https://github.com/Gusseth/Kawashiro-Project/blob/master/Kawashiro%20Project/Resources/icon512.png?raw=true")
			.WithAuthor(author => {
				author
					.WithName("The Kawashiro Project")
					.WithUrl("https://github.com/Gusseth/Kawashiro-Project/tree/master/Kawashiro%20Project")
					.WithIconUrl(avatarURL);
			})
			.AddField("The Commands", string.Format("The following commands are available as of **{0}**:", Nitori.version))
			.AddField("🗑️delete/del", "<int> or <messageID>\n**Even older messages.**", true)
			.AddField("📊poll", "<title> <prompt> <\"options\">\nCreates a 20\nsecond poll.", true)
			.AddField("🎚️autoclear/ac", "<channel>\nMention a channel to toggle auto-clear.", true)
			.AddField("📣echo", "<any length args>\nBot parrots what you say.", true)
			.AddField("🌿weed", "<filename> or dir\nUploads local files!", true)
			.AddField("🏓ping", "No args.\nReturns a 'Pong!'", true)
			.AddField("🤑flexmoney", "<natural>\nFlex cash on the bot.", true)
			.Build();

			await Context.Channel.SendMessageAsync(null, embed: embedSingleton).ConfigureAwait(false);

		}

    }
}
