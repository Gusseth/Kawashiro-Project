using Discord;
using Kawashiro_Project.util;
using Kawashiro_Project.Properties;
using System.IO;
using Newtonsoft.Json.Linq;
using Kawashiro_Project.exceptions;

namespace Kawashiro_Project.data
{
    class Config
    {
        public const string CONFIG_PATH = "data\\config.json"; // Hardcoded config path.
        public const uint CURRENT_CONFIG_VERSION = 0;          // Used to determine if a config is outdated.

        public string token { get; private set; }   // Current token the bot uses.
        private uint configVersion { get; set; }    // The version of the config to determine if a rewrite is necessary.

        private JObject config;                     // The entire config as a json object.

        /// <summary>
        /// Creates an object based off of data from the config.json.
        /// </summary>
        /// <param name="path">The path to the config.json file</param>
        public Config(string path)
        {
            LoadConfig(FindConfig(path));
        }

        /// <summary>
        /// Loads or reloads the config.
        /// </summary>
        /// <param name="path">The path to the config.json file</param>
        public void LoadConfig(string path)
        {
            config = JObject.Parse(File.ReadAllTextAsync(path).Result);

            token = (string)config.GetValue("token");
            configVersion = uint.Parse((string)config.GetValue("configVersion"));
            
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
        /// Checks if data\config.json exists. Also makes the data directory if it doesn't already exists.
        /// </summary>
        /// <param name="path">The path to the config.json file</param>
        /// <returns></returns>
        private string FindConfig(string path)
        {
            if (File.Exists(path))
            {
                return path;
            } else
            {
                Debug.Log("config.json is not found in the data folder! Building the config...", LogSeverity.Warning, this.ToString());
                RewriteConfig(path);
                return path;
            }
        }

        private void RewriteConfig(string path)
        {
            Directory.CreateDirectory(DataManager.PATH_TO_DATA_FOLDER);
            File.WriteAllBytesAsync(path, Resources.config);
        }
    }
}
