using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kawashiro_Project.util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands
{
    public class CommandHandler
    {
        protected int argPosition;                  // 

        protected readonly CommandService commandService;       // CommandService singleton
        protected readonly DiscordSocketClient client;          // Client singleton
        protected readonly IServiceProvider serviceProvider;    // IServiceProvider singleton

        public CommandHandler(CommandService commandService, DiscordSocketClient client, IServiceProvider serviceProvider, int argPosition = 0)
        {
            this.commandService = commandService;
            this.client = client;
            this.serviceProvider = serviceProvider;
            this.argPosition = argPosition;
        }

        public async Task Initialize()
        {
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
            client.MessageReceived += HandleCommandAsync;
        }

        /// <summary>
        /// Command observer. This checks every message and filters out irrelevant messages.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task HandleCommandAsync(SocketMessage message)
        {
            SocketUserMessage msg = message as SocketUserMessage;
            //if (msg == null) return;    // Don't process system messages

            // Great filter below
            if (msg == null ||
                !msg.HasStringPrefix(Nitori.config.prefix, ref argPosition) ||  // If the message doesn't contain the prefix
                msg.Author.IsBot)                                               // or the author is a bot
            {     
                return;         // Reject the message
            }

            SocketCommandContext context = new SocketCommandContext(client, msg);

            IResult result = await commandService.ExecuteAsync(context, argPosition + 1, serviceProvider);  // Parses the command
                                                                                                            // argPos + 1 so that the command and the prefix are separate
            
        }

        private async Task CommandFailed()
        {

        }
    }
}
