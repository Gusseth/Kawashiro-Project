using Discord;
using Kawashiro_Project.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kawashiro_Project.util
{
    public static class KappaIO
    {
        /// <summary>
        /// Makes sure that the file exists in the given path. If not, then generate one from an internal version of the file.
        /// </summary>
        /// <param name="path">Path to desired file.</param>
        /// <param name="resourceFile">Source file from resources </param>
        /// <param name="msg">Message to log when the file is not found.</param>
        /// <returns></returns>
        public static string EnsureFileExists(string path, byte[] resourceFile, string msg = null)
        {
            if (!File.Exists(path))
            {
                Debug.Log(msg, LogSeverity.Warning);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllBytesAsync(path, resourceFile);
            }
            return path;
        }
    }
}
