using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Kawashiro_Project.data;
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
        /// <summary>
        /// Enumerated Message deletion. Deletes amount number of messages.
        /// </summary>
        /// <param name="amount">n posts above the command call</param>
        /// <returns></returns>
        [Command("Delete")]
        [Alias("Del")]
        [Summary("Enumerated Message deletion. Deletes amount number of messages.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        public async Task Delete(int amount)
        {
            if (amount <= 0)
            {
                await ReplyAsync(string.Format(LineManager.GetLine("InvalidArgument"), amount, "greater than 0."));
                return;
            }
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await DeleteMessages(messages);
            await DeleteSuccess(messages.Count());
        }

        /// <summary>
        /// Message deletion via messageID. Deletes all the messages below the given ID. 
        /// Exclusive, meaning the given message ID's message won't be deleted.
        /// </summary>
        /// <param name="destinationID">Upper bound of the deletion</param>
        /// <param name="startID">Lower bound of the deletion</param>
        /// <returns></returns>
        [Command("Delete")]
        [Alias("Del")]
        [Summary("Message deletion via messageID. Deletes all the messages below the given ID. Exclusive, meaning the destinationID message won't be deleted.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        public async Task Delete(ulong destinationID, ulong startID = 0)
        {
            IMessage destination = Context.Channel.GetMessageAsync(destinationID).Result;
            // IMessage end = null;
            if (destination == null) 
            { 
                await ReplyAsync("No such message exists."); 
                return; 
            }

            
            if (startID != 0) {
                //end = Context.Channel.GetMessageAsync(startID).Result;
                //if (end == null) await ReplyAsync("The start message does not exist.");
                throw new NotImplementedException();
            }

            //int deletedMessages = int.MaxValue;

            IEnumerable<IMessage> accept = await Context.Channel.GetMessagesAsync(destinationID, Direction.After, int.MaxValue).FlattenAsync();

            /*
            if (end != null)
            {
                IEnumerable<IMessage> reject = await Context.Channel.GetMessagesAsync(end, Direction.After, int.MaxValue).FlattenAsync();
                accept = accept.Except(reject);
                foreach (IMessage m in accept) Debug.Log(m.Content);

            }
            */
            await DeleteMessages(accept);
            await DeleteSuccess(accept.Count());
        }

        /// <summary>
        /// Precision delete, give an array of message IDs to delete.
        /// </summary>
        /// <param name="ids">Message IDs to delete</param>
        /// <returns></returns>
        [Command("PDelete")]
        [Alias("pd")]
        [Summary("Precision delete, put in a bunch of message IDs to delete.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        public async Task PreciseDelete(params ulong?[] ids)
        {
            ulong count = 0;
            foreach (ulong id in ids)
            {
                IMessage msg = Context.Channel.GetMessageAsync(id).Result;
                if (msg == null) continue;
                await msg.DeleteAsync();
                await Task.Delay(Nitori.config.rateDelayInMs);
                count++;
            }
            await DeleteSuccess(count);

        }

        /// <summary>
        /// Deletes the given messages no matter what
        /// </summary>
        /// <param name="messages">Messages to delete</param>
        /// <returns></returns>
        private async Task DeleteMessages(IEnumerable<IMessage> messages)
        {
            try
            {
                // Use official API for bulk deletion for newer messages
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
            }
            catch (ArgumentException)
            {
                // We've reached messages that are older than 2 weeks
                foreach (IMessage msg in messages)
                {
                    await Context.Channel.DeleteMessageAsync(msg.Id);
                    await Task.Delay(Nitori.config.rateDelayInMs);
                }
            }
        }

        /// <summary>
        /// Deletes the command, sends a message to the channel about the deletion, 
        /// then deletes it in Nitori.config.deleteMessageInMs milliseconds
        /// </summary>
        /// <param name="args">Arguments for the compounded string</param>
        /// <returns></returns>
        private async Task DeleteSuccess(params object[] args)
        {
            IUserMessage delMessage = await ReplyAsync(
                string.Format(LineManager.GetLine("DeleteMessageNumeric"), args));
            await Task.Delay(Nitori.config.deleteMessageInMs);
            await delMessage.DeleteAsync();
        }

    }
}
