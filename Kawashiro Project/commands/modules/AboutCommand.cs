﻿using Discord;
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
        [Command("About")]
        public async Task About()
        {
			string avatarURL = Context.Client.CurrentUser.GetAvatarUrl();

			var builder = new EmbedBuilder()
			.WithTitle("**The Kawashiro Project** - A Multipurpose Bot for /r/Weather")
			.WithDescription(string.Format(LineManager.GetLine("About"), "Nitori Kawashiro", Nitori.version))
			.WithUrl("https://github.com/Gusseth/Kawashiro-Project/tree/master/Kawashiro%20Project")
			.WithColor(new Color(0x67BE67))
			.WithCurrentTimestamp()
			.WithFooter(footer => {
				footer
					.WithText("A bot tailored for /r/weather")
					.WithIconUrl(avatarURL);
			})
			.WithThumbnailUrl("https://github.com/Gusseth/Kawashiro-Project/blob/master/Kawashiro%20Project/Resources/icon512.png?raw=true")
			.WithImageUrl("https://raw.githubusercontent.com/Gusseth/Kawashiro-Project/master/Kawashiro%20Project/Resources/logo.png")
			.WithAuthor(author => {
				author
					.WithName("The Kawashiro Project")
					.WithUrl("https://github.com/Gusseth/Kawashiro-Project/tree/master/Kawashiro%20Project")
					.WithIconUrl(avatarURL);
			})
			.AddField("The Commands", string.Format("The following commands are available as of {0}:", Nitori.version))
			.AddField("🗑️delete/del", "<int> or <messageID>\nEven older messages.\n", true)
			.AddField("📣echo", "<any length args>\nBot parrots what you say.", true)
			.AddField("🌿weed", "<filename> or dir\nUploads local files!", true)
			.AddField("🏓ping", "No args.\nReturns a 'Pong!'", true);
			var embed = builder.Build();
			await Context.Channel.SendMessageAsync(
					null,
					embed: embed)
					.ConfigureAwait(false);

		}

    }
}