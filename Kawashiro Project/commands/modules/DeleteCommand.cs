using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kawashiro_Project.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class DeleteCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Delete")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Delete(int amount)
        {
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
            if ((Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages).IsCompletedSuccessfully)
            {
                await ReplyAsync("gay");
            }
            IUserMessage delMessage = await ReplyAsync($"Deleted {amount} messages.");
            await Task.Delay(1000 * 5);
            await delMessage.DeleteAsync();
        }

        [Command("Delete")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Delete(ulong destinationID, ulong startID = 0)
        {
            IMessage destination = Context.Channel.GetMessageAsync(destinationID).Result;
            // IMessage end = null;
            if (destination == null) 
            { 
                await ReplyAsync("No such message exists."); 
                return; 
            }

            /*
            if (startID != 0) {
                end = Context.Channel.GetMessageAsync(startID).Result;
                if (end == null) await ReplyAsync("The start message does not exist.");
            }

            int deletedMessages = int.MaxValue;

             */
            IEnumerable<IMessage> accept = await Context.Channel.GetMessagesAsync(destinationID, Direction.After, int.MaxValue).FlattenAsync();
            
            /*
            if (end != null)
            {
                IEnumerable<IMessage> reject = await Context.Channel.GetMessagesAsync(end, Direction.After, int.MaxValue).FlattenAsync();
                accept = accept.Except(reject);
                foreach (IMessage m in accept) Debug.Log(m.Content);

            }
            */

            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(accept);
            IUserMessage delMessage = await ReplyAsync($"Deleted messages.");
            await Task.Delay(1000 * 5);
            await delMessage.DeleteAsync();
        }
    }
}
