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
    class Config
    {
        public const string CONFIG_PATH = "data\\config.json"; // Hardcoded config path.
        public const uint CURRENT_CONFIG_VERSION = 0;          // Used to determine if a config is outdated.

        public string token { get; private set; }   // Current token the bot uses.
        public string prefix { get; private set; }  // Bot prefix
        private uint configVersion { get; set; }    // The version of the config to determine if a rewrite is necessary.

        private JObject config;                     // The entire config as a json object.

        /// <summary>
        /// Creates an object based off of data from the config.json.
        /// </summary>
        /// <param name="path">The path to the config.json file</param>
        public Config(string path)
        {
            path = FindConfig(path); // path will always be path
            ReadConfig(path);
            LoadConfigVariables(path);
        }

        /// <summary>
        /// Loads or reloads the config.
        /// </summary>
        /// <param name="path">The path to the config.json file</param>
        public void ReadConfig(string path)
        {
            config = JObject.Parse(File.ReadAllTextAsync(path).Result);
        }

        /// <summary>
        /// Loads all base variables in the config such as token, etc.
        /// </summary>
        public void LoadConfigVariables(string path)
        {
            token = config.Value<string>("token");
            configVersion = config.Value<uint>("configVersion");

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
        /// Checks if data\config.json exists. Also makes the data directory if it doesn't already exists.
        /// </summary>
        /// <param name="path">The path to the config.json file</param>
        /// <returns>path</returns>
        private string FindConfig(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("config.json is not found in the data folder! Building the config...", LogSeverity.Warning);
                RewriteConfig(path);
            }
            return path;
        }

        /// <summary>
        /// Overwrites/Creates a config based on the internal one found in Resources
        /// </summary>
        /// <param name="path"></param>
        private void RewriteConfig(string path)
        {
            Directory.CreateDirectory(DataManager.PATH_TO_DATA_FOLDER);
            File.WriteAllBytesAsync(path, Resources.config);
        }
    }
}
