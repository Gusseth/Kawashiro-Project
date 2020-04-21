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

        public static string GetLine(string key)
        {
            List<string> validLines;
            lines.TryGetValue(key, out validLines);
            if (validLines == null) return $"Line [{key}] not found! ";
            if (validLines.Count == 1) return validLines[0];
            return validLines[Nitori.random.Next(validLines.Count)];
        }

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
