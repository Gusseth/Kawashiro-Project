using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class EchoCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Echo")]
        public async Task Ping([Remainder] string msg)
        {
            await ReplyAsync(msg);
        }
    }
}
