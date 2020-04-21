using Discord;
using Kawashiro_Project.util;
using Kawashiro_Project.Properties;
using System.IO;
using Newtonsoft.Json.Linq;
using Kawashiro_Project.exceptions;
using Discord.Commands;
using Discord.WebSocket;
using System;

namespace Kawashiro_Project.data
{
    public class Config
    {
        public const string CONFIG_PATH = "data\\config.json"; // Hardcoded config path.
        public const uint CURRENT_CONFIG_VERSION = 0;          // Used to determine if a config is outdated.

        public ulong authorID { get; private set; }         // Current token the bot uses.
        public string token { get; private set; }           // Current token the bot uses.
        public string prefix { get; private set; }          // Bot prefix
        public bool separatePrefix { get; private set; }    // Should bot prefix be separate
        public int deleteMessageInMs { get; private set; }  // Deletes temporary messages in x milliseconds
        public int rateDelayInMs { get; private set; }      // Delay to avoid rate limit

        private uint configVersion { get; set; }    // The version of the config to determine if a rewrite is necessary.

        private JObject config;                     // The entire config as a json object.

        /// <summary>
        /// Creates an object based off of data from the config.json.
        /// </summary>
        /// <param name="path">The path to the config.json file</param>
        public Config(string path)
        {
            path = RewriteConfig(path);
            ReadConfig(path);
        }

        /// <summary>
        /// Loads or reloads the config.
        /// </summary>
        /// <param name="path">The path to the config.json file</param>
        public void ReadConfig(string path)
        {
            config = JObject.Parse(File.ReadAllTextAsync(path).Result);

            token = config.Value<string>("token");
            configVersion = config.Value<uint>("configVersion");
            prefix = config.Value<string>("prefix");
            separatePrefix = config.Value<bool>("separatePrefix");
            authorID = config.Value<ulong>("authorID");
            deleteMessageInMs = config.Value<int>("deleteMessageInMs");
            rateDelayInMs = config.Value<int>("rateDelayInMs");

            if (configVersion != CURRENT_CONFIG_VERSION)
            {
                Debug.Log("The current config is outdated. Rebuilding config...");
                RewriteConfig(path);
                throw new OutdatedConfigException();
            }
            if (token == "")
            {
                throw new MissingTokenException();
            }
        }

        /// <summary>
        /// Returns a CommandServiceConfig as specified in config.json or a dummy config if one doesn't exist
        /// </summary>
        /// <returns>CommandServiceConfig as specified in config.json or an empty config</returns>
        public CommandServiceConfig GetCommandServiceConfig()
        {
            return config.Value<CommandServiceConfig>("CommandServiceConfig") ?? new CommandServiceConfig();
        }

        /// <summary>
        /// Returns a DiscordSocketConfig as specified in config.json or a dummy config if one doesn't exist
        /// </summary>
        /// <returns>DiscordSocketConfig as specified in config.json or an empty config</returns>
        public DiscordSocketConfig GetDiscordSocketConfig()
        {
            return config.Value<DiscordSocketConfig>("DiscordSocketConfig") ?? new DiscordSocketConfig();
        }

        /// <summary>
        /// Rebuilds/Generates a config.json from Resources
        /// </summary>
        /// <param name="path">Path to config.json</param>
        /// <returns>path</returns>
        private string RewriteConfig(string path)
        {
            KappaIO.EnsureFileExists(path, Resources.config, "config.json is not found in the data folder! Building the config...");
            return path;
        }
    }
}
