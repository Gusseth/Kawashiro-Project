using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class WeedCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Weed")]
        public async Task Weed([Remainder] string filename)
        {
            await Context.Channel.SendFileAsync(Path.Combine("data\\media\\" + filename));
        }
    }
}
