using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
            /*
            string tempMsg = msg.ToLower();
            if (tempMsg.Contains(Context.Client.CurrentUser.Mention) && tempMsg.Contains("gay") || tempMsg.Contains("g@y") && !tempMsg.Contains("not"))
            {
                await ReplyAsync("no u");
                return;
            }
            */
            await ReplyAsync(msg);
        }
    }
}
