using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kawashiro_Project.data;
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
        protected int argPosition;   // Which 'word' is the start of the command

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

        /// <summary>
        /// Initializes this class.
        /// </summary>
        /// <returns></returns>
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
            if (msg == null) return;    // If the message is a system message, reject immediately

            bool prefixGuard = Nitori.Config.separatePrefix ? 
                (msg.HasStringPrefix(Nitori.Config.prefix, ref argPosition)     // Confirms that the prefix is spearated as otherwise <prefix>ogpes is accepted.
                    && (msg.Content.StartsWith(Nitori.Config.prefix + " ")))    // i.e. prefix = !n and separate, !nhelp is accepted instead of !n help
                : msg.HasStringPrefix(Nitori.Config.prefix, ref argPosition);   // If the prefix is not separated, only check for the presence of the prefix

            // Great filter below
            if (!prefixGuard ||                         // If the message doesn't pass the prefix check
                msg.Content == Nitori.Config.prefix ||  // If the message itself is empty and only contains the prefix
                msg.Author.IsBot)                       // or the author is a bot
                
                return;                                 // Reject the message

            SocketCommandContext context = new SocketCommandContext(client, msg);

            int commandStart = Nitori.Config.separatePrefix ? 
                argPosition + 1 : argPosition;          // argPos + 1 so that the command and the prefix 
                                                        // are separate if separartePrefix is true
            
            _ = commandService.ExecuteAsync(context, commandStart, serviceProvider);  // Parses the command in a separate thread, unawaited.
                                                                                      // This means that each running command are handled in separate
                                                                                      // threads, not gridlocking the main thread of the bot.

            await Task.CompletedTask;
        }

        /// <summary>
        /// Fires when the command is done executing.
        /// </summary>
        /// <param name="result">Result of the command</param>
        /// <param name="context">The message behind the command</param>
        /// <returns></returns>
        public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!string.IsNullOrEmpty(result?.ErrorReason))
            {
                int prefixLength = Nitori.Config.separatePrefix ?
                    Nitori.Config.prefix.Length + 1 : Nitori.Config.prefix.Length;  // Adjust for the additional whitespace

                string commandName = command.IsSpecified ? // Checks if the command that errored out is a valid command
                    command.Value.Name :                   // If it is a valid command, use the name of said command
                    context.Message.Content.Substring      // If not, then use the "command" that was attempted as the name
                    (prefixLength);
                await ParseCommandErrors(context, commandName, result.Error, result.ErrorReason);
            }
        }
        

        /// <summary>
        /// Parses command errors from the given CommandError
        /// </summary>
        /// <param name="context">The message behind the command</param>
        /// <param name="error">The type of error</param>
        /// <param name="reason">The message if and error arises</param>
        /// <returns></returns>
        private async Task ParseCommandErrors(ICommandContext context, string command, CommandError? error, string reason)
        {
            string author = context.Message.Author.Username;
            string prefix = Nitori.Config.separatePrefix ? Nitori.Config.prefix + " " : Nitori.Config.prefix;  // {3} argument is prefix, compensate for having a spearated prefix
            switch (error)
            {
                case (CommandError.Exception | CommandError.Unsuccessful):
                    await CommandFailed(context, command, reason);
                    break;
                case (CommandError.UnknownCommand):
                    await Nitori.Say(context.Channel, ResponseManager.GetLine("UnknownCommandError"), command, reason, author, prefix);
                    break;
                case (CommandError.UnmetPrecondition):
                    if (reason == "") await Nitori.Say(context.Channel, ResponseManager.GetLine("CommandPermissionsError"), command, reason, author);
                    break;
                default:
                    await CommandFailed(context, command, reason);
                    break;
            }
        }

        /// <summary>
        /// Fire when a command fails to execute due to an exception
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="command"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        private async Task CommandFailed(ICommandContext context, string command = "", string reason = "")
        {
            IMessageChannel channel = context.Channel;
            string author = context.Message.Author.Username;
            if (!string.IsNullOrEmpty(command)) command = "'" + command + "' ";
            string reasonSentence = ResponseManager.GetLine("NoReasonGiven");
            if (!string.IsNullOrEmpty(reason)) reasonSentence = ResponseManager.GetLine("ReasonGiven");
            await Nitori.Say(channel, ResponseManager.GetLine("CommandExecutionError") + reasonSentence, command, reason, author);
        }
    }
}
