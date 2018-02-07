using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord.WebSocket;
using System.Threading;

namespace Lomztein.AdvDiscordCommands.Misc {

    public static class Logging {

        public static float minLogSaveTime = 5f;
        public static string logFileDirectory = AppContext.BaseDirectory + "/Logs/";
        private static DateTime lastLogTime = DateTime.Now;

        public enum LogType {
            CHAT, SYSTEM, WARNING, CRITICAL, CONFIG, EXCEPTION, BOT
        }

        public static Queue<string> loggingQueue = new Queue<string> ();

        public static void Log (LogType logType, string message) {
            if (message == null || message.Length == 0)
                return;
            string combine = $"[{logType}][{DateTime.Now}] - {message}";

            Console.WriteLine (combine);
            loggingQueue.Enqueue (combine);

            if (logType == LogType.CRITICAL) {
                Console.WriteLine ($"SYSTEM HALTED DUE TO CRITICAL ERROR: {message}");
                while (true) {
                    Thread.Sleep (1000);
                }
            }

            if (lastLogTime.AddSeconds (minLogSaveTime) < DateTime.Now) {
                File.AppendAllLines ($"{logFileDirectory}{DateTime.Now.Year}-{DateTime.Now.Month}", loggingQueue);
                loggingQueue.Clear ();
            }
        }

        public static void Log(Exception exception) {
            Log (LogType.EXCEPTION, $"{exception.Message} - {exception.StackTrace}");
        }
    }
}
