using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kawashiro_Project.util
{
    public static class Debug
    {
        private const string debugSource = "Debug.Log";

        /// <summary>
        /// Logs a message to console.
        /// </summary>
        /// <param name="msg">Message to be displayed</param>
        /// <returns></returns>
        public static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs a message to console.
        /// </summary>
        /// <param name="msg">Message to be displayed</param>
        /// <param name="logSeverity">How important is this message</param>
        /// <param name="source">Name of the source of this log</param>
        /// <returns></returns>
        public static Task Log(string msg, LogSeverity logSeverity = LogSeverity.Info, string source = debugSource)
        {
            if (msg == null) return Task.CompletedTask; // Do nothing if the message is null.    
            LogMessage logMessage = new LogMessage(logSeverity, source, msg);
            Console.WriteLine(logMessage.ToString());
            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs a message to console through an object. Uses ToString().
        /// </summary>
        /// <param name="obj">Message to be displayed using ToString()</param>
        /// <param name="logSeverity">How important is this message</param>
        /// <param name="source">Name of the source of this log</param>
        /// <returns></returns>
        public static Task Log(object obj, LogSeverity logSeverity = LogSeverity.Info, string source = debugSource)
        {
            if (obj == null) return Task.CompletedTask; // Do nothing if the message is null.  
            LogMessage logMessage = new LogMessage(logSeverity, source, obj.ToString());
            Console.WriteLine(logMessage.ToString());
            return Task.CompletedTask;
        }

        /// <summary>
        /// Throws the specified error.
        /// </summary>
        /// <param name="e">Error that should be thrown</param>
        /// <returns></returns>
        public static Task ThrowError(Exception e)
        {
            throw e;
        }

        /// <summary>
        /// Throws a very generic error.
        /// </summary>
        /// <returns></returns>
        public static Task ThrowError()
        {
            return ThrowError(new Exception());
        }
    }
}
