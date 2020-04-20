using Discord;
using Discord.Commands;
using Discord.WebSocket;
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

        public CommandHandler(CommandService commandService, DiscordSocketClient client, IServiceProvider serviceProvider, int argPosition = 1)
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

        private async Task HandleCommandAsync(SocketMessage message)
        {
            var msg = message as SocketUserMessage;
            if (msg == null) return;    // Don't process system messages

            if (msg == null ||
                !msg.HasStringPrefix(Nitori.config.prefix, ref argPosition) ||
                msg.HasMentionPrefix(client.CurrentUser, ref argPosition) ||
                msg.Author.IsBot) return;

            var context = new SocketCommandContext(client, msg);

            var result = await commandService.ExecuteAsync(context, argPosition, serviceProvider);
        }
    }
}
