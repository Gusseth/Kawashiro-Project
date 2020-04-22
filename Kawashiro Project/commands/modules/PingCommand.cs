using Discord.Commands;
using Kawashiro_Project.data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class PingCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Ping")]
        [Summary("Pong!")]
        public async Task Ping()
        {
            await ReplyAsync(LineManager.GetLine("PingResponse"));
        }
    }
}
