using Discord.Commands;
using Kawashiro_Project.data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class EmbedCommand : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Returns an embed message with the given key or parses the given embed.
        /// </summary>
        /// <param name="key">Key of the deisred embed to display</param>
        /// <param name="json">JSON to be parsed</param>
        /// <returns></returns>
        [Command("Embed")]
        [Summary("Returns an embed message with the given key or parses the given embed")]
        public async Task Embed(string key, [Remainder] string json = "")
        {
            if (key.ToLower() == "parse") await ReplyAsync(null, false, ResponseManager.BuildEmbed(json).Build());
            else await ReplyAsync(null, false, ResponseManager.GetEmbed(key).Build());
        }
    }
}
