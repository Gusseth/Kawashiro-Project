using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kawashiro_Project.commands;
using Kawashiro_Project.data;
using Kawashiro_Project.data.entities;
using Kawashiro_Project.exceptions;
using Kawashiro_Project.util;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kawashiro_Project
{
    class Nitori
    {
        public const string version = "0.3rc1";

        public static Config Config { get; private set; }                   // Singleton Config class
        public static ResponseManager ResponseManager { get; private set; }  // Singleton LineManager class
        public static GuildManager GuildManager { get; private set; }       // Singleton GuildManager class
        public static Random Random { get; private set; }                   // Singleton Random class
        public static IUser User { get; private set; }                      // The bot Discord user
        public static DiscordSocketClient Client { get; private set; }      // The Discord client

        protected DiscordSocketClient client;       // Client that is used to connect to Discord
        protected CommandService commandService;    // Base for all commands
        protected IServiceProvider serviceProvider; // Used for dependency injection. Future feature?
        protected CommandHandler commandHandler;    // CommandHandler singleton

        private string token { get; set; }      // Bot token as given from the Discord Developer site

        public static void Main(string[] args) => new Nitori().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// The main async method of the bot. The console.
        /// </summary>
        /// <returns></returns>
        public async Task MainAsync()
        {
            if (await Initialize())
            {
                LogIn();
            }

            await Task.Delay(-1);
        }

        /// <summary>
        /// Initializes the bot.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> Initialize()
        {
            try
            {
                Random = new Random();
                Config = new Config(Config.CONFIG_PATH);
                GuildManager = new GuildManager(GuildManager.GUILDS_PATH);
                ResponseManager = new ResponseManager(ResponseManager.LINES_PATH, ResponseManager.EMBEDS_PATH); // One hell of a hackjob since client.CurrentUser is only updated
                token = Config.token;

                client = new DiscordSocketClient(Config.GetDiscordSocketConfig());
                commandService = new CommandService(Config.GetCommandServiceConfig());
                serviceProvider = new ServiceCollection().AddSingleton(client).AddSingleton(commandService).BuildServiceProvider();

                commandHandler = new CommandHandler(commandService, client, serviceProvider);
                await commandHandler.Initialize();

                client.Log += Debug.Log;
                client.UserVoiceStateUpdated += OnUserVoiceStateUpdated;
                client.Ready += OnReady;
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

        /// <summary>
        /// Logs the bot out.
        /// </summary>
        private async void LogOut()
        {
            await client.LogoutAsync();
            await client.StopAsync();
            await Debug.Log("Successfully logged out");
        }

        /// <summary>
        /// Says something in the given text channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task Say(IMessageChannel channel, string msg)
        {
            await channel.SendMessageAsync(msg);
        }

        /// <summary>
        /// Generic-fied version of the Say function for compounded strings.
        /// </summary>
        /// <param name="channel">The channel this message is posted</param>
        /// <param name="msg">The messsage content</param>
        /// <param name="args">Compounded string arguments</param>
        /// <returns></returns>
        public static async Task Say(IMessageChannel channel, string msg, params object[] args)
        {
            await channel.SendMessageAsync(string.Format(msg, args));
        }

        /// <summary>
        /// Reloads all configs. Need I say more?
        /// </summary>
        public static void ReloadConfig()
        {
            string oldToken = Config.token;
            Config = new Config(Config.CONFIG_PATH);
            ResponseManager = new ResponseManager(ResponseManager.LINES_PATH, ResponseManager.EMBEDS_PATH);
            ResponseManager.LoadEmbeds();
            GuildManager = new GuildManager(GuildManager.GUILDS_PATH);
        }

        /// <summary>
        /// Fires when a user joins/leaves a voice channel
        /// </summary>
        /// <param name="user">User that is responsible for this event being fired</param>
        /// <param name="before">The user's previous state</param>
        /// <param name="after">The user's current state</param>
        /// <returns></returns>
        public async Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            if (after.VoiceChannel != null) return; // Do nothing if no one left

            SocketGuild guild = before.VoiceChannel.Guild;

            if (GuildManager.guilds.TryGetValue(guild.Id, out KappaGuild kGuild))  // Gets the guild persistent data
            {
                foreach (SocketVoiceChannel voiceChannel in guild.VoiceChannels)
                {
                    if (voiceChannel.Users.Count > 0) return;  // Do nothing if there is a channel with people connected
                }

                foreach (ulong channelID in kGuild.clearedChannels)
                {
                    SocketTextChannel channel = guild.GetTextChannel(channelID);
                    _ = channel.DeleteMessagesAsync(await channel.GetMessagesAsync(int.MaxValue).FlattenAsync());
                }
            }
        }

        /// <summary>
        /// Fires when the client is done connecting to the API
        /// </summary>
        /// <returns></returns>
        public async Task OnReady()
        {
            User = client.CurrentUser;
            ResponseManager.LoadEmbeds();   // Loads the embeds late because it depends on grabbing the current user's avatar
            await Task.CompletedTask;
        }
    }
}
