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
        /// <summary>
        /// Responds with "PingResponse" from lines.json
        /// </summary>
        /// <returns></returns>
        [Command("Ping", true)]
        [Summary("Pong!")]
        public async Task Ping()
        {
            await ReplyAsync(LineManager.GetLine("PingResponse"));
        }
    }
}
