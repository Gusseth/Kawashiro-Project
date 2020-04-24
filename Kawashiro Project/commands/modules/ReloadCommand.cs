using Discord.Commands;
using Kawashiro_Project.data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class ReloadCommand : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Reloads configs
        /// </summary>
        /// <returns></returns>
        [Command("Reload")]
        [Summary("Reloads config.json and lines.json")]
        [RequireOwner]
        public async Task Reload()
        {
            await ReplyAsync(ResponseManager.GetLine("BotReload"));
            Nitori.ReloadConfig();
        }
    }
}
