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
    public class LineManager
    {
        public const string LINES_PATH = "data\\lines.json";

        private static Dictionary<string, List<string>> lines;

        public LineManager(string path)
        {
            path = KappaIO.EnsureFileExists(path, Resources.lines, "lines.json is not found in the data folder! Building lines.json...");
            lines = new Dictionary<string, List<string>>();

            JObject json = JObject.Parse(File.ReadAllTextAsync(path).Result);
            LoadLines(json);
        }

        /// <summary>
        /// Gets one line from lines.json
        /// </summary>
        /// <param name="key">Key associated with the line</param>
        /// <returns>A line from lines.json</returns>
        public static string GetLine(string key)
        {
            List<string> validLines;
            lines.TryGetValue(key, out validLines);
            if (validLines == null) return $"Line [{key}] not found! "; // When the line doesn't exist
            if (validLines.Count == 1) return validLines[0];            // When only one entry exists, return that one entry
            return validLines[Nitori.random.Next(0, validLines.Count)]; // When multiple entries exist, pick one random entry 
        }

        /// <summary>
        /// Loads a json into the lines dictionary
        /// </summary>
        /// <param name="json">JSON object to be parsed</param>
        private void LoadLines(JObject json)
        {
            foreach (JProperty jobject in json.Properties())
            {
                string key = jobject.Name;
                List<string> values = new List<string>();
                foreach (JValue value in jobject.Values())
                {
                    values.Add(value.Value<string>());
                }
                lines.Add(key, values);
            }
        }
    }
}
