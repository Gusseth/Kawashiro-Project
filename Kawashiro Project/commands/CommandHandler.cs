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
            commandService.CommandExecuted += OnCommandExecutedAsync;
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
            if (msg == null ||                                                  // If the message is a system message
                !msg.HasStringPrefix(Nitori.config.prefix, ref argPosition) ||  // If the message doesn't contain the prefix
                msg.Content == Nitori.config.prefix ||                          // If the message itself is empty and only contains the prefix
                msg.Author.IsBot)                                               // or the author is a bot
                
                return;                                                         // Reject the message

            SocketCommandContext context = new SocketCommandContext(client, msg);

            IResult result = await commandService.ExecuteAsync(context, argPosition + 1, serviceProvider);  // Parses the command
                                                                                                            // argPos + 1 so that the command and the prefix are separate
            //await PostCommandExecution(result, context);
        }

        /*
        /// <summary>
        /// Fires when the command is done executing.
        /// </summary>
        /// <param name="result">Result of the command</param>
        /// <param name="context">The message behind the command</param>
        /// <returns></returns>
        private async Task PostCommandExecution(IResult result, SocketCommandContext context)
        {
            if (result.Error != null) await ParseCommandErrors(context, result.Error, result.ErrorReason);
        }
        */
        public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!string.IsNullOrEmpty(result?.ErrorReason))
            {
                string commandName = command.IsSpecified ? command.Value.Name : context.Message.Content.Substring(Nitori.config.prefix.Length + 1);
                await ParseCommandErrors(context, commandName, result.Error, result.ErrorReason);
            }
        }
        

        /// <summary>
        /// Parses command errors from the given CommandError
        /// </summary>
        /// <param name="context">The message behind the command</param>
        /// <param name="error">The type of error</param>
        /// <param name="reason">The message if</param>
        /// <returns></returns>
        private async Task ParseCommandErrors(ICommandContext context, string command, CommandError? error, string reason)
        {
            switch (error)
            {
                case (CommandError.Exception | CommandError.Unsuccessful):
                    await CommandFailed(context.Channel, reason);
                    break;
                case (CommandError.UnknownCommand):
                    await Nitori.Say(context.Channel, $"I don't know what '{command}' is.");
                    break;
                case (CommandError.UnmetPrecondition):
                    if (reason == "") await Nitori.Say(context.Channel, $"Sorry, but you are not a true administrator to use '{command}'.");
                    break;
                default:
                    await CommandFailed(context.Channel, reason);
                    break;
            }
        }

        /// <summary>
        /// Fires when a command fails to execute due to an exception
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        private async Task CommandFailed(IMessageChannel channel, string reason = "")
        {
            string reasonSentence = "No reason given.";
            if (reason != "")
            {
                reasonSentence = $"Reason: {reason}";
            }
            await Nitori.Say(channel, "The command failed to execute.\n" + reasonSentence);
        }
    }
}
