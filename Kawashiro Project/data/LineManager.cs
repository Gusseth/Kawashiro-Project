using Kawashiro_Project.Properties;
using Kawashiro_Project.util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kawashiro_Project.data
{
    public class LineManager
    {
        public const string LINES_PATH = "data\\lines.json";

        private Dictionary<Lines, List<string>> lines;
        public LineManager(string path)
        {
            path = KappaIO.EnsureFileExists(path, Resources.lines, "lines.json is not found in the data folder! Building lines.json...");

            JObject json = JObject.Parse(File.ReadAllTextAsync(path).Result);
        }
    }
}
