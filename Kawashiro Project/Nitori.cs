using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kawashiro_Project.commands;
using Kawashiro_Project.data;
using Kawashiro_Project.exceptions;
using Kawashiro_Project.util;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Kawashiro_Project
{
    class Nitori
    {
        public const string version = "0.0.1a";

        public static Config config;                // Singleton Config class

        protected DiscordSocketClient client;       // Client that is used to connect to Discord
        protected CommandService commandService;    // Base for all commands
        protected IServiceProvider serviceProvider; // Message I/O service for the bot
        protected CommandHandler commandHandler;    // CommandHandler singleton

        private string token { get; set; }      // Bot token as given from the Discord Developer site

        public static void Main(string[] args) => new Nitori().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            if (Initialize().Result)
            {
                LogIn();
            }

            await Task.Delay(-1);
        }

        private async Task<bool> Initialize()
        {
            try
            {
                config = new Config(Config.CONFIG_PATH);
                token = config.token;

                client = new DiscordSocketClient(config.GetDiscordSocketConfig());
                commandService = new CommandService(config.GetCommandServiceConfig());
                serviceProvider = new ServiceCollection().AddSingleton(client).AddSingleton(commandService).BuildServiceProvider();

                commandHandler = new CommandHandler(commandService, client, serviceProvider);
                await commandHandler.Initialize();

                client.Log += Debug.Log;
                return true;
            }
            catch (OutdatedConfigException)
            {
                await Debug.Log(new LogMessage(LogSeverity.Critical, "init", "Your config has been rebuilt. Please fill in."));
            }
            catch (MissingTokenException)
            {
                await Debug.Log(new LogMessage(LogSeverity.Critical, "init", "The token that you have given in the config is missing. Please fill it in."));
            } 
            catch (Exception e)
            {
                await Debug.Log(new LogMessage(LogSeverity.Critical, "init", "This program has spectacularly failed on startup:", e));
            }
            return false;
        }

        /// <summary>
        /// Logs the bot in to the given bot token.
        /// </summary>
        private async void LogIn()
        {
            try {             
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
            } 
            catch (ArgumentException)
            {
                await Debug.Log(new LogMessage(LogSeverity.Critical, "login", "The bot has failed to login due to a malformed token."));
            }
            catch (Exception e)
            {
                await Debug.Log(new LogMessage(LogSeverity.Critical, "login", "This program has spectacularly failed on startup:", e));
            }
        }
    }
}
