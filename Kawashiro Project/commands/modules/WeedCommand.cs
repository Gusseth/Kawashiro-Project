using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Kawashiro_Project.data;

namespace Kawashiro_Project.commands.modules
{
    public class WeedCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Weed")]
        [Summary("Uploads a file from the data\\media folder.")]
        [RequireUserPermission(ChannelPermission.AttachFiles)]
        public async Task Weed([Remainder] string filename = "dir")
        {
            string path = Path.Combine("data\\media\\" + filename);

            if (filename.ToLower() == "dir")
            {
                string filenames = "";
                IEnumerable<string> mediaFolderContents = Directory.EnumerateFiles("data\\media\\");
                ulong numFiles = (ulong)mediaFolderContents.Count();

                if (numFiles == 0)
                {
                    await Nitori.Say(Context.Channel, ResponseManager.GetLine("MediaFolderEmpty"));
                    return;
                }
                foreach (string file in mediaFolderContents)
                {
                    filenames = filenames + file + "\n";
                }
                await Nitori.Say(Context.Channel, ResponseManager.GetLine("MediaFolderDisplay") +
                    ResponseManager.GetLine("MediaFolderCodeblock"), filenames, numFiles);
                return;
            }
            else if (File.Exists(path))
            {
                await Context.Channel.SendFileAsync(Path.Combine("data\\media\\" + filename));
                return;
            }
            await Nitori.Say(Context.Channel, ResponseManager.GetLine("FileNotFound"), filename);
        }
    }
}
