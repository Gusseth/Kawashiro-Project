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
        [Command("Reload")]
        [RequireOwner]
        public async Task Reload()
        {
            await ReplyAsync(LineManager.GetLine("BotReload"));
            Nitori.ReloadConfig();
        }
    }
}
