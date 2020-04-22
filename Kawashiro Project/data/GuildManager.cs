using Kawashiro_Project.data.entities;
using Kawashiro_Project.util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kawashiro_Project.data
{
    public class GuildManager
    {
        public const string GUILDS_PATH = "data\\guilds.json"; // Hardcoded guilds.json path.

        public Dictionary<ulong, KappaGuild> guilds;    // Guild dictionary, <guild id, guild persistency data>

        /// <summary>
        /// Creates an object based off of data from the guilds.json.
        /// </summary>
        /// <param name="path">The path to the guilds.json file</param>
        public GuildManager(string path)
        {
            path = KappaIO.EnsureFileExists(path, "guilds.json is not found in the data folder! Building an empty guilds.json...");
            guilds = new Dictionary<ulong, KappaGuild>();
            string jsonData = File.ReadAllTextAsync(path).Result;
            if (!string.IsNullOrEmpty(jsonData)) ReadGuilds(JObject.Parse(jsonData));
        }

        /// <summary>
        /// Loads or reloads the guilds.json.
        /// </summary>
        /// <param name="path">The path to the guilds.json file</param>
        public void ReadGuilds(JObject jsonFile)
        {
            foreach (JProperty guildData in jsonFile.Properties())
            {
                KappaGuild guild = guildData.Value.ToObject<KappaGuild>();
                guilds.Add(ulong.Parse(guildData.Name), guild);
            }
        }
    }
}
