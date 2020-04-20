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

        public static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public static Task Log(string msg, LogSeverity logSeverity = LogSeverity.Info, string source = debugSource)
        {
            if (msg == null) return Task.CompletedTask; // Do nothing if the message is null.    
            LogMessage logMessage = new LogMessage(logSeverity, source, msg);
            Console.WriteLine(logMessage.ToString());
            return Task.CompletedTask;
        }

        public static Task Log(object msg, LogSeverity logSeverity = LogSeverity.Info, string source = debugSource)
        {
            if (msg == null) return Task.CompletedTask; // Do nothing if the message is null.  
            LogMessage logMessage = new LogMessage(logSeverity, source, msg.ToString());
            Console.WriteLine(logMessage.ToString());
            return Task.CompletedTask;
        }

        public static Task ThrowError(Exception e)
        {
            throw e;
        }

        public static Task ThrowError()
        {
            return ThrowError(new Exception());
        }
    }
}
