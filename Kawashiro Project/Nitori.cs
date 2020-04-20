using Discord;
using Discord.WebSocket;
using Kawashiro_Project.data;
using Kawashiro_Project.exceptions;
using Kawashiro_Project.util;
using System;
using System.Threading.Tasks;

namespace Kawashiro_Project
{
    class Nitori
    {
        public const string version = "0.0.1a";

        public static Config config;            // Singleton Config class

        protected DiscordSocketClient client;   // Client that is used to connect to Discord

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
                client = new DiscordSocketClient();
                client.Log += Debug.Log;

                config = new Config(Config.CONFIG_PATH);
                token = config.token;
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
