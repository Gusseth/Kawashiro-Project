using Discord;
using Discord.WebSocket;
using Kawashiro_Project.data.entities;
using Kawashiro_Project.Properties;
using Kawashiro_Project.util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kawashiro_Project.data
{
    public class ResponseManager
    {
        public const string LINES_PATH = "data\\lines.json";
        public const string EMBEDS_PATH = "data\\embeds.json";
        private static readonly EmbedBuilder EmptyEmbed = 
            new EmbedBuilder().WithTitle("Missing embed!");

        private static Dictionary<string, List<string>> lines;
        private static Dictionary<string, List<EmbedBuilder>> embeds;

        public ResponseManager(string linesPath, string embedsPath)
        {
            linesPath = KappaIO.EnsureFileExists(linesPath, Resources.lines, "lines.json is not found in the data folder! Building lines.json...");
            lines = new Dictionary<string, List<string>>();
            embeds = new Dictionary<string, List<EmbedBuilder>>();

            // Might make these two be laoded in separate threads in the future
            JObject linesJson = JObject.Parse(File.ReadAllTextAsync(linesPath).Result);
            JObject embedsJson = JObject.Parse(File.ReadAllTextAsync(embedsPath).Result);
            LoadLines(linesJson);
            LoadEmbeds(embedsJson);
        }

        /// <summary>
        /// Gets one line from lines.json with the current key
        /// </summary>
        /// <param name="key">Key associated with the line</param>
        /// <returns>A line from lines.json</returns>
        public static string GetLine(string key)
        {
            List<string> validLines;
            lines.TryGetValue(key, out validLines);
            if (validLines == null) return $"Line [{key}] not found! "; // When the line doesn't exist
            if (validLines.Count == 1) return validLines[0];            // When only one entry exists, return that one entry
            return validLines[Nitori.Random.Next(0, validLines.Count)]; // When multiple entries exist, pick one random entry 
        }

        /// <summary>
        /// Gets one EmbedBuilder from embeds.json with the current key
        /// </summary>
        /// <param name="key">Key associated with the embed</param>
        /// <returns></returns>
        public static EmbedBuilder GetEmbed(string key)
        {
            List<EmbedBuilder> validEmbeds;
            embeds.TryGetValue(key, out validEmbeds);
            if (validEmbeds == null) return EmptyEmbed
                    .WithColor(13576232)
                    .WithDescription($"Embed [{key}] not found!");          // When the embed doesn't exist
            if (validEmbeds.Count == 1) return validEmbeds[0];              // When only one entry exists, return that one entry
            return validEmbeds[Nitori.Random.Next(0, validEmbeds.Count)];   // When multiple entries exist, pick one random entry 
        }

        /// <summary>
        /// Builds an EmbedBuilder with the given json data
        /// </summary>
        /// <param name="embed">Embed JSON data</param>
        /// <returns></returns>
        public static EmbedBuilder BuildEmbedBuilder(JObject embed, params object[] args)
        {
            string title = embed.Value<string>("title");
            string description = embed.Value<string>("description");
            string url = embed.Value<string>("url");
            uint color = embed.Value<uint?>("color") ?? 6798951;
            bool timestamp = embed.Value<bool?>("timestamp") ?? false;

            JObject footer = embed.Value<JObject>("footer");

            string thumbnail = embed.Value<string>("thumbnail");
            string image = embed.Value<string>("image");

            JObject author = embed.Value<JObject>("author");

            JToken fields = embed["fields"];

            EmbedBuilder embedBuilder = new EmbedBuilder();

            embedBuilder
                .WithTitle(title)               // Adds title
                .WithDescription(description)   // Adds description
                .WithUrl(url)                   // Adds a link with the title
                .WithColor(color)               // Adds colour to the side of the embed
                .WithThumbnailUrl(thumbnail)    // Adds the image to the top-right side
                .WithImageUrl(image);           // Adds the larger image at the bottom of the embed

            if (timestamp) embedBuilder.WithCurrentTimestamp(); // Adds timestamp with the current time and date

            // Adding footer, if available
            if (footer != null)
            {
                bool icon = footer.Value<bool?>("icon") ?? true;
                string iconUrl = footer.Value<string>("iconUrl") ?? footer.Value<string>("icon_url") ?? Nitori.User.GetAvatarUrl(); // Defaults to user avatar if not provided
                string text = footer.Value<string>("text");
                _ = !icon ? embedBuilder.WithFooter(tempFooter => tempFooter.WithText(text)) :              // Without an icon
                    embedBuilder.WithFooter(tempFooter => tempFooter.WithIconUrl(iconUrl).WithText(text));  // With an icon
            }

            // Adding the author header, if available
            if (author != null)
            {
                string name = author.Value<string>("name");
                string nameUrl = author.Value<string>("url");
                bool icon = author.Value<bool?>("icon") ?? true;
                string iconUrl = author.Value<string>("iconUrl") ?? author.Value<string>("icon_url") ?? Nitori.User.GetAvatarUrl();
                _ = !icon ? embedBuilder.WithAuthor(tempAuthor => tempAuthor.WithName(name).WithUrl(nameUrl)) :  // Without an icon
                    embedBuilder.WithAuthor(tempAuthor =>
                    tempAuthor.WithIconUrl(iconUrl).WithName(name).WithUrl(nameUrl));                           // With an icon
            }

            // Adding fields that are defined in the template, if available
            if (fields != null)
            {
                foreach (JObject field in fields)
                {
                    string name = field.Value<string>("name");
                    string value = field.Value<string>("value");
                    bool inline = field.Value<bool?>("inline") ?? false;
                    embedBuilder.AddField(name, value, inline);
                }
            }

            return embedBuilder;
        }

        /// <summary>
        /// Builds an EmbedBuilder with the given json data
        /// </summary>
        /// <param name="embed">Embed JSON data as a string</param>
        /// <returns></returns>
        public static EmbedBuilder BuildEmbed(string json) => BuildEmbedBuilder(JObject.Parse(json));

        /// <summary>
        /// Loads a json and parses strings into the lines dictionary
        /// </summary>
        /// <param name="json">JSON object for lines.json</param>
        private void LoadLines(JObject json)
        {
            foreach (JProperty entry in json.Properties())
            {
                string key = entry.Name;
                List<string> addLines = new List<string>();
                foreach (JValue value in entry.Values())
                {
                    addLines.Add(value.Value<string>());
                }
                lines.Add(key, addLines);
            }
        }

        /// <summary>
        /// Loads a json and parses an embed into the embeds dictionary
        /// </summary>
        /// <param name="json">JSON object for embeds.json</param>
        private void LoadEmbeds(JObject json, params object[] args)
        {
            foreach (JProperty entry in json.Properties())
            {
                string key = entry.Name;
                List<EmbedBuilder> addEmbeds = new List<EmbedBuilder>();
                foreach (JObject embed in entry.Values<JObject>())
                {
                    addEmbeds.Add(BuildEmbedBuilder(embed));
                }
                embeds.Add(key, addEmbeds);
            }
        }
    }
}
