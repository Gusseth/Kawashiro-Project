using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class EchoCommand : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Makes the bot reply with the same message
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [Command("Echo")]
        [Summary("Makes the bot reply with the same message.")]
        public async Task Ping([Remainder] string msg)
        {
            await ReplyAsync(msg);
        }
    }
}
